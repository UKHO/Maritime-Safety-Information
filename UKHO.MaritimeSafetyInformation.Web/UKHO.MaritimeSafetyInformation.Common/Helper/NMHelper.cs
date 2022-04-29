using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models;

namespace UKHO.MaritimeSafetyInformation.Common.Helper
{
    public static class NMHelper
    {
        public static List<ShowFilesResponseModel> GetShowFilesResponses(BatchSearchResponse SearchResult)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new();
            foreach (BatchDetails item in SearchResult.Entries)
            {
                foreach (BatchDetailsFiles file in item.Files)
                {
                    ListshowFilesResponseModels.Add(new ShowFilesResponseModel
                    {
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
            return ListshowFilesResponseModels;
        }

        public static List<ShowDailyFilesResponseModel> GetDailyShowFilesResponse(BatchSearchResponse SearchResult)
        {
            List<ShowDailyFilesResponseModel> showDailyFilesResponses = new List<ShowDailyFilesResponseModel>();
            List<AttributesModel> lstattributes = (SearchResult.Entries.Where(x=>x.AllFilesZipSize.HasValue).Select(item => new AttributesModel
            {
                BatchId = item.BatchId,
                DataDate = item.Attributes.Where(x => x.Key.Equals("Data Date")).Select(x => x.Value).FirstOrDefault(),
                WeekNumber = item.Attributes.Where(x => x.Key.Equals("Week Number")).Select(x => x.Value).FirstOrDefault(),
                Year = item.Attributes.Where(x => x.Key.Equals("Year")).Select(x => x.Value).FirstOrDefault(),
                YearWeek = item.Attributes.Where(x => x.Key.Equals("Year / Week")).Select(x => x.Value).FirstOrDefault(),
                AllFilesZipSize = (long)item.AllFilesZipSize
            })).ToList();

            var groupped = lstattributes.GroupBy(x => x.YearWeek);
            foreach (var group in groupped)
            {
                List<DailyFilesDataModel> lstDataDate = (group.Select(item => new DailyFilesDataModel
                {
                    BatchId = item.BatchId,
                    DataDate = item.DataDate,
                    Filename = "Daily " + item.DataDate + ".zip",
                    FileExtension = ".zip",
                    FileDescription = "Daily " + item.DataDate + ".zip",
                    AllFilesZipSize = item.AllFilesZipSize,
                    FileSizeinKB = FileHelper.FormatSize(item.AllFilesZipSize),
                    MimeType = "application/gzip"
                })).ToList();

                lstDataDate.Distinct();
                lstDataDate = lstDataDate.OrderBy(x => Convert.ToDateTime(x.DataDate)).ToList();

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
    }
}
