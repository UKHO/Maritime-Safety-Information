
namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowDailyFilesResponseListModel
    {
        public List<ShowDailyFilesResponseModel> ShowDailyFilesResponseModel { get; set; }

        public bool IsDailyFilesResponseCached { get; set; }
    }
}
