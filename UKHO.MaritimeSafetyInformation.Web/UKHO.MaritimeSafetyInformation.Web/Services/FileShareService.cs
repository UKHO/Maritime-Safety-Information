using Microsoft.Extensions.Options;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class FileShareService : IFileShareService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptions<FileShareServiceConfiguration> fileShareServiceConfig;
        private readonly ILogger<FileShareService> _logger;

        public FileShareService(IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig, ILogger<FileShareService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.fileShareServiceConfig = fileShareServiceConfig;
            _logger = logger;

        }

        public async Task<IResult<BatchSearchResponse>> FssWeeklySearchAsync(string searchText, string accessToken)
        {
            try
            {
                _logger.LogInformation(EventIds.RetrievalOfMSIBatchSearchResponse.ToEventId(), "Maritime safety information request batch search response started");

                FileShareApiClient fileShareApi = new FileShareApiClient(httpClientFactory, fileShareServiceConfig.Value.BaseUrl, accessToken);
                IResult<BatchSearchResponse> result = await fileShareApi.Search(searchText, 100, 0, CancellationToken.None);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.RetrievalOfMSIBatchSearchResponseFailed.ToEventId(), "Failed to get batch search response data {exceptionMessage} {exceptionTrace}", ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
