
namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowWeeklyFilesResponseModel
    {
        public List<ShowFilesResponseModel> ShowFilesResponseModel { get; set; }
        public List<YearWeekModel> YearAndWeek { get; set; }
    }
}
