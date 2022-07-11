using Microsoft.Extensions.Logging;
using System.Globalization;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public static class NMHelper
    {
        public static List<ShowFilesResponseModel> ListFilesResponse(BatchSearchResponse SearchResult)
        {
            List<BatchDetails> batchDetailsList = new();            
            if (SearchResult.Entries.Count > 1)
            {          

               batchDetailsList = SearchResult.Entries.OrderByDescending(t => t.BatchPublishedDate).ToList();

                SearchResult.Entries = batchDetailsList;
                SearchResult.Count = batchDetailsList.Count;
                SearchResult.Total = batchDetailsList.Count;
            }
            List<ShowFilesResponseModel> listshowFilesResponseModels = new();
            foreach (BatchDetails item in SearchResult.Entries)
            {
                foreach (BatchDetailsFiles file in item.Files)
                {
                    listshowFilesResponseModels.Add(new ShowFilesResponseModel
                    {
                        BatchId = item.BatchId,
                        Filename = file.Filename,
                        FileDescription = Path.GetFileNameWithoutExtension(file.Filename),
                        FileExtension = Path.GetExtension(file.Filename),
                        FileSize = file.FileSize,
                        FileSizeinKB = FileHelper.FormatSize((long)file.FileSize),
                        MimeType = file.MimeType,
                        Links = file.Links,
                        IsDistributorUser = item.Attributes.Where(x => x.Key.Equals("Content"))
                        .Select(x => x.Value).FirstOrDefault() == "tracings"
                    });
                }
            }
            return listshowFilesResponseModels;
        }

        public static List<ShowDailyFilesResponseModel> GetDailyShowFilesResponse(BatchSearchResponse searchResult)
        {
            List<ShowDailyFilesResponseModel> showDailyFilesResponses = new();
            List<AttributesModel> attributes = searchResult.Entries.Where(x => x.AllFilesZipSize.HasValue).Select(item => new AttributesModel
            {
                BatchId = item.BatchId,
                DataDate = item.Attributes.Where(x => x.Key.Equals("Data Date")).Select(x => x.Value).FirstOrDefault(),
                WeekNumber = item.Attributes.Where(x => x.Key.Equals("Week Number")).Select(x => x.Value).FirstOrDefault(),
                Year = item.Attributes.Where(x => x.Key.Equals("Year")).Select(x => x.Value).FirstOrDefault(),
                YearWeek = item.Attributes.Where(x => x.Key.Replace(" ", "").Equals("Year/Week")).Select(x => x.Value.Replace(" ", "")).FirstOrDefault(),
                AllFilesZipSize = (long)item.AllFilesZipSize,
                BatchPublishedDate = item.BatchPublishedDate,
            }).OrderByDescending(x => x.BatchPublishedDate)
            .GroupBy(x => Convert.ToDateTime(x.DataDate))
            .Select(x => x.First())
            .ToList();

            IEnumerable<IGrouping<string, AttributesModel>> grouped = attributes.GroupBy(x => x.YearWeek);
            foreach (IGrouping<string, AttributesModel> group in grouped)
            {
                List<DailyFilesDataModel> lstDataDate = (group.Select(item => new DailyFilesDataModel
                {
                    BatchId = item.BatchId,
                    DataDate = item.DataDate,
                    Filename = "Daily " + GetFormattedDate(item.DataDate) + ".zip",
                    FileExtension = ".zip",
                    FileDescription = "Daily " + GetFormattedDate(item.DataDate) + ".zip",
                    AllFilesZipSize = item.AllFilesZipSize,
                    FileSizeInKB = FileHelper.FormatSize(item.AllFilesZipSize),
                    MimeType = "application/gzip"
                })).Distinct().OrderBy(x => Convert.ToDateTime(x.DataDate)).ToList();

                showDailyFilesResponses.Add(new ShowDailyFilesResponseModel
                {
                    YearWeek = group.Key,
                    Year = group.Select(x => x.Year).FirstOrDefault(),
                    WeekNumber = group.Select(x => x.WeekNumber).FirstOrDefault(),
                    DailyFilesData = lstDataDate
                });

                showDailyFilesResponses = showDailyFilesResponses.OrderByDescending(x => x.Year).ThenByDescending(x => x.WeekNumber).ToList();
            }
            return showDailyFilesResponses;
        }

        public static string GetFormattedDate(string strDate)
        {
            string[] formats = { "M/d/yyyy", "d/M/yyyy", "M-d-yyyy", "d-M-yyyy", "d-MMM-yy", "d-MMMM-yyyy", "yyyy-MM-dd" };

            if (DateTime.TryParseExact(strDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return date.ToString("dd-MM-yy");

            return string.Empty;
        }

        public static void ValidateParametersForDownloadSingleFile(List<KeyValuePair<string, string>> parameters, string correlationId, ILogger logger)
        {
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter.Value))
                {
                    logger.LogInformation(
                        EventIds.DownloadSingleWeeklyNMFileInvalidParameter.ToEventId(),
                        "Maritime safety information download single NM files called with invalid argument " + parameter.Key + ":{" + parameter.Key + "} for _X-Correlation-ID:{correlationId}", parameter.Value, correlationId);
                    throw new ArgumentNullException("Invalid value received for parameter " + parameter.Key, new Exception());
                }
            }
        }

        public static async Task<byte[]> GetFileBytesFromStream(Stream stream)
        {
            byte[] fileBytes = new byte[stream.Length + 10];

            int numBytesToRead = (int)stream.Length;
            int numBytesRead = 0;
            do
            {
                int n = await stream.ReadAsync(fileBytes, numBytesRead, numBytesToRead);
                numBytesRead += n;
                numBytesToRead -= n;
            } while (numBytesToRead > 0);
            stream.Close();
            return fileBytes;
        }
    }
}
