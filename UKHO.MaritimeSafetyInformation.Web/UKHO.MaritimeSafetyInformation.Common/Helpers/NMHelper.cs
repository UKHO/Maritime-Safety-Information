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
            if (SearchResult.Entries.Count > 1)
            {
                List<BatchDetails> batchDetailsList = new();

                BatchDetails batchDetail = SearchResult.Entries.OrderByDescending(t => t.BatchPublishedDate).FirstOrDefault();
                batchDetailsList.Add(batchDetail);
                SearchResult.Entries = batchDetailsList;
                SearchResult.Count = batchDetailsList.Count;
                SearchResult.Total = batchDetailsList.Count;
            }
            return GetShowFilesResponseModel(SearchResult.Entries);
        }

        public static List<ShowFilesResponseModel> GetShowFilesResponseModel(List<BatchDetails> batchDetails)
        {
            List<ShowFilesResponseModel> listShowFilesResponseModels = new();
            foreach (BatchDetails item in batchDetails)
            {
                foreach (BatchDetailsFiles file in item.Files)
                {
                    item.Attributes.Add(new BatchDetailsAttributes { Key = "BatchPublishedDate", Value = item.BatchPublishedDate.ToString() });
                    listShowFilesResponseModels.Add(new ShowFilesResponseModel
                    {
                        Attributes = item.Attributes,
                        BatchId = item.BatchId,
                        Filename = file.Filename,
                        FileDescription = Path.GetFileNameWithoutExtension(file.Filename),
                        FileExtension = Path.GetExtension(file.Filename),
                        FileSize = file.FileSize,
                        FileSizeinKB = FileHelper.FormatSize((long)file.FileSize),
                        MimeType = file.MimeType,
                        Links = file.Links,
                    });
                }
            }
            return listShowFilesResponseModels;
        }

        public static List<ShowFilesResponseModel> ListFilesResponseLeisure(BatchSearchResponse searchResult)
        {

            List<ShowFilesResponseModel> listShowFilesResponseModels = GetShowFilesResponseModel(searchResult.Entries);

            return listShowFilesResponseModels
                .OrderByDescending(x => Convert.ToDateTime(x.Attributes.FirstOrDefault(y => y.Key == "BatchPublishedDate")?.Value))
                .GroupBy(x => x.Attributes.FirstOrDefault(y => y.Key == "Chart")?.Value)
                .Select(grp => grp.First())
                .OrderBy(x => x.Attributes.FirstOrDefault(y => y.Key == "Chart")?.Value)
                .ToList();
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
                        EventIds.DownloadSingleNMFileInvalidParameter.ToEventId(),
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

        public static List<ShowFilesResponseModel> ListFilesResponseCumulative(List<BatchDetails> batchDetails)
        {
            return GetShowFilesResponseModel(batchDetails)
                .OrderByDescending(x => Convert.ToDateTime(x.Attributes.FirstOrDefault(y => y.Key == "BatchPublishedDate")?.Value))
                .GroupBy(x => x.Attributes.FirstOrDefault(y => y.Key == "Data Date")?.Value)
                .Select(grp => grp.First())
                .OrderByDescending(x => Convert.ToDateTime(x.Attributes.FirstOrDefault(y => y.Key == "Data Date")?.Value))
                .ToList();
        }

        public static List<ShowFilesResponseModel> GetShowAnnualFilesResponseModel(List<BatchDetails> batchDetails)
        {
            List<ShowFilesResponseModel> listShowFilesResponseModels = new();
            foreach (BatchDetails item in batchDetails)
            {
                foreach (BatchDetailsFiles file in item.Files)
                {
                    switch (Path.GetFileNameWithoutExtension(file.Filename))
                    {
                        case "An overview of each of the 26 sections":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 1, "--"));
                            break;

                        case "ADMIRALTY Tide Tables 2022 — General Information":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 2, "1"));
                            break;
                        case "Suppliers of ADMIRALTY Charts and Publications":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 3, "2"));
                            break;
                        case "Safety of British merchant ships in periods of peace, tension or conflict":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 4, "3"));
                            break;
                        case "Firing Practice and Exercise Areas":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 5, "5"));
                            break;
                        case "Mine-Laying and Mine Countermeasures Exercises - Waters around the British Isles":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 6, "10"));
                            break;
                        case "National Claims to Maritime Jurisdiction":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 7, "12"));
                            break;
                        case "Global Navigational Satellite System Positions, Horizontal Datums and Position Shifts":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 8, "19"));
                            break;
                        case "Mandatory Expanded Inspections - EU Directive 2009.16.EC":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 9, "20"));
                            break;
                        case "Canadian Charts and Nautical Publications Regulations":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 10, "21"));
                            break;
                        case "US Navigation Safety Regulations Relating to Navigation, Charts and Publications":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 11, "22"));
                            break;
                        case "High Speed Craft":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 12, "23"));
                            break;
                        case "Marine Environmental High Risk Areas":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 13, "26"));
                            break;
                        case "T&P 2022":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 14, "--"));
                            break;
                        case "Amends to SDs":
                            listShowFilesResponseModels.Add(SetShowFilesForAnnualResponseModel(item, file, 15, "--"));
                            break;
                    }
                }
            }
            return listShowFilesResponseModels.OrderBy(x=>x.DisplayOrder).ToList();
        }

        public static ShowFilesResponseModel SetShowFilesForAnnualResponseModel(BatchDetails batchDetails, BatchDetailsFiles file, int displayOrder, string secton)
        {
            ShowFilesResponseModel showFilesResponse = new ShowFilesResponseModel();
            showFilesResponse.DisplayOrder = displayOrder;
            showFilesResponse.Secton = secton;
            showFilesResponse.Attributes = batchDetails.Attributes;
            showFilesResponse.BatchId = batchDetails.BatchId;
            showFilesResponse.Filename = file.Filename;
            showFilesResponse.FileDescription = Path.GetFileNameWithoutExtension(file.Filename);
            showFilesResponse.FileExtension = Path.GetExtension(file.Filename);
            showFilesResponse.FileSize = file.FileSize;
            showFilesResponse.FileSizeinKB = FileHelper.FormatSize((long)file.FileSize);
            showFilesResponse.MimeType = file.MimeType;
            showFilesResponse.Links = file.Links;
            return showFilesResponse;
        }


    }
}
