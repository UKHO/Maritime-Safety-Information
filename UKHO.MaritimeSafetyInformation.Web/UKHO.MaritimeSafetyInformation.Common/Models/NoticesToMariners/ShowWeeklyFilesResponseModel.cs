
namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowWeeklyFilesResponseModel
    {
        public List<ShowFilesResponseModel> ShowFilesResponseList { get; set; }
        public List<YearWeekModel> YearAndWeekList { get; set; }
    }
}
