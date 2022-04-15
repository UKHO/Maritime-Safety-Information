using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class FileShareService : IFileShareService
    {
        private readonly IHttpClientFactory httpClientFactory;
        public FileShareService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<IResult<BatchSearchResponse>> FssWeeklySearchAsync(string searchText, string accessToken)
        {
            try
            {
                FileShareApiClient fileShareApi = new FileShareApiClient(httpClientFactory, "https://filesqa.admiralty.co.uk", accessToken);
                IResult<BatchSearchResponse> result = await fileShareApi.Search(searchText, 25, 0, CancellationToken.None);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<Stream> GetFssFileStreamAsync(string batchId, string filename,string accessToken)
        {
            FileShareApiClient fileShareApi = new FileShareApiClient(httpClientFactory, "https://filesqa.admiralty.co.uk", accessToken);
            Stream stream = await fileShareApi.DownloadFileAsync(batchId, filename);
            return stream;
        }
    }
}
