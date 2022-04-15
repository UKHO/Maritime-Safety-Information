using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        public Task<IEnumerable<BatchDetailsFiles>> GetBatchDetailsFiles(int year, int week, string accessToken);
        public Task<byte[]> DownloadFssFileAsync(string batchId, string filename, string accessToken);
    }
}
