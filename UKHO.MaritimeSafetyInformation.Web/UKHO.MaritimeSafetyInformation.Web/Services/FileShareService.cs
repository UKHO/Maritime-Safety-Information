using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class FileShareService : IFileShareService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly FileShareServiceConfiguration fileShareServiceConfig;
        public FileShareService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.fileShareServiceConfig = configuration.GetSection("FileShareService").Get<FileShareServiceConfiguration>();

        }
        public async Task<IResult<BatchSearchResponse>> FssWeeklySearchAsync(string searchText, string accessToken)
        {
            try
            {
                FileShareApiClient fileShareApi = new FileShareApiClient(httpClientFactory, fileShareServiceConfig.BaseUrl, accessToken);
                IResult<BatchSearchResponse> result = await fileShareApi.Search(searchText, 25, 0, CancellationToken.None);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
