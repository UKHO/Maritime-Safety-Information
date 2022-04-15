using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareService
    {
        public Task<IResult<BatchSearchResponse>> FssWeeklySearchAsync(string searchText,string accessToken);
        public Task<Stream> GetFssFileStreamAsync(string batchId, string filename, string accessToken);
    }
}
