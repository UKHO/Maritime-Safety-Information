using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareService
    {
        Task<IResult<BatchSearchResponse>> FssBatchSearchAsync(string searchText, string accessToken, string correlationId);
        Task<byte[]> FSSDownloadFileAsync(string batchId, string filename, string accessToken, string correlationId);
    }
}
