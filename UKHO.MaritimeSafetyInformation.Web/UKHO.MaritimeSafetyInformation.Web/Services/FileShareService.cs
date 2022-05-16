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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private readonly ILogger<FileShareService> _logger;

        public FileShareService(IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig, ILogger<FileShareService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _fileShareServiceConfig = fileShareServiceConfig;
            _logger = logger;
        }

        public async Task<IResult<BatchSearchResponse>> FssBatchSearchAsync(string searchText, string accessToken, string correlationId)
        {            
            IResult<BatchSearchResponse> result = new Result<BatchSearchResponse>();
            try
            {
                string searchQuery = $"BusinessUnit eq '{_fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{_fileShareServiceConfig.Value.ProductType}' " + searchText;

                _logger.LogInformation(EventIds.FSSBatchSearchResponseStarted.ToEventId(), "Maritime safety information request for FSS to get NM batch search response started for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);

                FileShareApiClient fileShareApi = new(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);
                result = await fileShareApi.Search(searchQuery, _fileShareServiceConfig.Value.PageSize, _fileShareServiceConfig.Value.Start, CancellationToken.None);
                _logger.LogInformation(EventIds.FSSBatchSearchResponseCompleted.ToEventId(), "Maritime safety information request for FSS to get NM batch search response completed for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSBatchSearchResponseFailed.ToEventId(), "Failed to get batch search response from FSS with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
            }
            return result;
            
        }

        public async Task<IResult<BatchAttributesSearchResponse>> FssSearchAttributeAsync(string accessToken, string correlationId)
        {
            IResult<BatchAttributesSearchResponse> result = new Result<BatchAttributesSearchResponse>();
            try
            {
                string searchQuery = $"BusinessUnit eq '{_fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{_fileShareServiceConfig.Value.ProductType}' and $batch(Frequency) eq 'Weekly'";

                _logger.LogInformation(EventIds.FSSSearchAttributeResponseStarted.ToEventId(), "Maritime safety information request for FSS to get NM batch search attribute response started for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);

                FileShareApiClient fileShareApi = new(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);
                result = await fileShareApi.BatchAttributeSearch(searchQuery, CancellationToken.None);

                _logger.LogInformation(EventIds.FSSSearchAttributeResponseCompleted.ToEventId(), "Maritime safety information request for FSS to get NM batch search attribute response completed for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSSearchAttributeResponseError.ToEventId(), "Failed to get NM batch search attribute with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
            return result;

        }
    }
}
