using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareService
    {
        Task<IResult<BatchSearchResponse>> FSSBatchSearchAsync(string searchText, string correlationId, IFileShareApiClient fileShareApiClient);
        Task<IResult<BatchAttributesSearchResponse>> FSSSearchAttributeAsync(string correlationId, IFileShareApiClient fileShareApiClient);
        Task<Stream> FSSDownloadFileAsync(string batchId, string fileName, string correlationId, IFileShareApiClient fileShareApiClient, string frequency);
        Task<Stream> FSSDownloadZipFileAsync(string batchId, string fileName, string correlationId, IFileShareApiClient fileShareApiClient);
    }
}
