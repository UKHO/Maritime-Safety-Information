using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class BatchSearchResponseModel
    {
        public BatchSearchResponse batchSearchResponse { get; set; }
        public bool WeeklyNMFilesIsCache { get; set; }
    }
}
