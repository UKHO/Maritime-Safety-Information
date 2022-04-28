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

        public async Task<IResult<BatchSearchResponse>> FssBatchSearchAsync(string searchText, string accessToken, string correlationId)
        {            
            IResult<BatchSearchResponse> result = new Result<BatchSearchResponse>();
            try
            {
                _logger.LogInformation(EventIds.FSSBatchSearchResponseStarted.ToEventId(), "Maritime safety information request batch search response started", correlationId);
                
                string searchQuery = $"BusinessUnit eq '{fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{fileShareServiceConfig.Value.ProductType} " + searchText;
                FileShareApiClient fileShareApi = new(httpClientFactory, fileShareServiceConfig.Value.BaseUrl, accessToken);
                result = await fileShareApi.Search(searchQuery, fileShareServiceConfig.Value.PageSize, fileShareServiceConfig.Value.Start, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSBatchSearchResponseFailed.ToEventId(), "Failed to get batch search response data {exceptionMessage} {exceptionTrace} for _X-Correlation-ID:{CorrelationId}", ex.Message, ex.StackTrace, correlationId);
            }
            return result;
            
        }
    }
}
