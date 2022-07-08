
namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class ShowNMFilesResponseModel
    {
        public List<ShowFilesResponseModel> ShowFilesResponseModel { get; set; }
        public bool NMFilesResponseIsCache { get; set; }
    }
}
