using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        public Task<List<ShowFilesResponseModel>> GetBatchDetailsFiles(int year, int week);
    }
}
