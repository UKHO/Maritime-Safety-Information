using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models;

namespace UKHO.MaritimeSafetyInformation.Common.Helper
{
    public class NMHelper
    {
        public List<ShowFilesResponseModel> GetShowFilesResponses(BatchSearchResponse SearchResult)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new List<ShowFilesResponseModel>();
            foreach (var item in SearchResult.Entries)
            {
                foreach (var file in item.Files)
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

        public List<ShowDailyFilesResponseModel> GetDailyShowFilesResponse(BatchSearchResponse SearchResult)
        {
            List<ShowDailyFilesResponseModel> showDailyFilesResponses = new List<ShowDailyFilesResponseModel>();
            List<AttributesModel> lstattributes = (SearchResult.Entries.Select(item => new AttributesModel
            {
                DataDate = item.Attributes.Where(x => x.Key.Equals("Data Date")).Select(x => x.Value).FirstOrDefault(),
                WeekNumber = item.Attributes.Where(x => x.Key.Equals("Week Number")).Select(x => x.Value).FirstOrDefault(),
                Year = item.Attributes.Where(x => x.Key.Equals("Year")).Select(x => x.Value).FirstOrDefault(),
                YearWeek = item.Attributes.Where(x => x.Key.Equals("Year / Week")).Select(x => x.Value).FirstOrDefault()
            })).ToList();

            var groupped = lstattributes.GroupBy(x => x.YearWeek);
            foreach (var group in groupped)
            {
                List<DailyFilesDataModel> lstDataDate = (group.Select(item => new DailyFilesDataModel
                {
                    DataDate = item.DataDate,
                    Filename = "Daily " + item.DataDate + ".zip",
                    FileExtension = ".zip",
                    FileDescription = "Daily " + item.DataDate + ".zip",
                    FileSizeinKB = FileHelper.FormatSize(30000),
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
