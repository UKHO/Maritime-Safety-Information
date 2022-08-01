using FluentValidation.Results;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.Validation;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class WebhookService : IWebhookService
    {

        private readonly IAzureTableStorageClient _azureTableStorageClient;
        private readonly IOptions<CacheConfiguration> _cacheConfiguration;
        private readonly IEnterpriseEventCacheDataRequestValidator _enterpriseEventCacheDataRequestValidator;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private readonly IAzureStorageService _azureStorageService;
        private readonly ILogger<WebhookService> _logger;


        public WebhookService(IAzureTableStorageClient azureTableStorageClient,
            IOptions<CacheConfiguration> cacheConfiguration,
            IEnterpriseEventCacheDataRequestValidator enterpriseEventCacheDataRequestValidator,
            IOptions<FileShareServiceConfiguration> fileShareServiceConfig,
            IAzureStorageService azureStorageService,
            ILogger<WebhookService> logger)
        {
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
            _enterpriseEventCacheDataRequestValidator = enterpriseEventCacheDataRequestValidator;
            _fileShareServiceConfig = fileShareServiceConfig;
            _azureStorageService = azureStorageService;
            _logger = logger;

        }
        public Task<ValidationResult> ValidateNewFilesPublishedEventData(FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest)
        {
            return _enterpriseEventCacheDataRequestValidator.Validate(enterpriseEventCacheDataRequest);
        }

        public async Task<bool> DeleteBatchSearchResponseCacheData(FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest, string correlationId)
        {
            string productType = enterpriseEventCacheDataRequest.Attributes.First(a => a.Key == "Product Type").Value;

            if (enterpriseEventCacheDataRequest.BusinessUnit == _fileShareServiceConfig.Value.BusinessUnit && productType == _fileShareServiceConfig.Value.ProductType)
            {
                _logger.LogInformation(EventIds.ClearFSSSearchCacheStarted.ToEventId(), "Clear FSS search cache started for _X-Correlation-ID:{correlationId}", correlationId);
                string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);
                List<string> tableNames = new() { _cacheConfiguration.Value.FssWeeklyBatchSearchTableName , _cacheConfiguration.Value.FssCacheResponseTableName };
                await _azureTableStorageClient.DeleteTablesAsync(tableNames, connectionString);
                _logger.LogInformation(EventIds.ClearFSSSearchCacheCompleted.ToEventId(), "Clear FSS search cache completed for _X-Correlation-ID:{correlationId}", correlationId);
                return true;
            }
            return false;
        }
    }
}
