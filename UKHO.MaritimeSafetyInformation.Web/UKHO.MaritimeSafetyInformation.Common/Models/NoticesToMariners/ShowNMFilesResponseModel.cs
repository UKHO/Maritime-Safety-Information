
namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowNMFilesResponseModel
    {
        public List<ShowFilesResponseModel> ShowFilesResponseModel { get; set; }
        public bool IsWeeklyBatchResponseCached { get; set; }
        public bool IsBatchResponseCached { get; set; }
    }
}
