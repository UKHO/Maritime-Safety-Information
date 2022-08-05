
namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowNMFilesResponseModel
    {
        public List<ShowFilesResponseModel> ShowFilesResponseModel { get; set; }
        public bool IsBatchResponseCached { get; set; }

        public static implicit operator List<object>(ShowNMFilesResponseModel v) => throw new NotImplementedException();
    }
}
