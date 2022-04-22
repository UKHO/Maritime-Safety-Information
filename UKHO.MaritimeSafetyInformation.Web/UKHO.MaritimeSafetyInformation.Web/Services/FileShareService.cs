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
        private readonly ILogger<FileShareService> _logger;
        public FileShareService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FileShareService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.fileShareServiceConfig = configuration.GetSection("FileShareService").Get<FileShareServiceConfiguration>();
            _logger = logger;

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
