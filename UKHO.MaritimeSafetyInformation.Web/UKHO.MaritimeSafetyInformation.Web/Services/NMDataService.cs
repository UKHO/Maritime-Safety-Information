using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class NMDataService : INMDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFileShareService fileShareService;
        public NMDataService(IFileShareService fileShareService, IHttpClientFactory httpClientFactory)
        {
            this.fileShareService = fileShareService;
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<BatchDetailsFiles>> GetBatchDetailsFiles(int year, int week, string accessToken)
        {
            string searchText = "$batch(Year) eq '" + year.ToString() + "' and $batch(Week Number) eq '" + week.ToString() + "'";
            var result = await fileShareService.FssWeeklySearchAsync(searchText, accessToken);

            BatchSearchResponse SearchResult = result.Data;
            IEnumerable<BatchDetailsFiles> listFiles = SearchResult.Entries.SelectMany(e => e.Files).ToList();

            return listFiles;

        }

        public async Task<byte[]> DownloadFssFileAsync(string batchId, string filename, string accessToken)
        {
            Stream stream = await fileShareService.GetFssFileStreamAsync(batchId, filename, accessToken);
            byte[] fileBytes = new byte[stream.Length];
            stream.Read(fileBytes, 0, fileBytes.Length);
            return fileBytes;
        }
    }
}
