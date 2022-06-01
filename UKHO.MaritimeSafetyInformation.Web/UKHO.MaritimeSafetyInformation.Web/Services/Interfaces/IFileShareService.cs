using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareService
    {
        Task<IResult<BatchSearchResponse>> FSSBatchSearchAsync(string searchText, string accessToken, string correlationId);
        Task<IResult<BatchAttributesSearchResponse>> FSSSearchAttributeAsync(string accessToken, string correlationId);
        Task<Stream> FSSDownloadFileAsync(string batchId, string fileName, string accessToken, string correlationId);
        Task<Stream> FSSDownloadZipFileAsync(string batchId, string fileName, string accessToken, string correlationId);
    }
}
