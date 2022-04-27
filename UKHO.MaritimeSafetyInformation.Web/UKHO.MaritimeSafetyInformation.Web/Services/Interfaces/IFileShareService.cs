using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareService
    {
        public Task<IResult<BatchSearchResponse>> FssWeeklySearchAsync(string searchText,string accessToken);
    }
}
