using FluentValidation.Results;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
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

        public WebhookService(IAzureTableStorageClient azureTableStorageClient,
            IOptions<CacheConfiguration> cacheConfiguration,
            IEnterpriseEventCacheDataRequestValidator enterpriseEventCacheDataRequestValidator,
            IOptions<FileShareServiceConfiguration> fileShareServiceConfig,
            IAzureStorageService azureStorageService)
        {
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
            _enterpriseEventCacheDataRequestValidator = enterpriseEventCacheDataRequestValidator;
            _fileShareServiceConfig = fileShareServiceConfig;
            _azureStorageService = azureStorageService;

        }
        public Task<ValidationResult> ValidateEventGridCacheDataRequest(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest)
        {
            return _enterpriseEventCacheDataRequestValidator.Validate(enterpriseEventCacheDataRequest);
        }

        public async Task<bool> DeleteSearchAndDownloadCacheData(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest, string correlationId)
        {
            var productCode = enterpriseEventCacheDataRequest.Attributes.First(a => a.Key == "ProductCode").Value;

            if (enterpriseEventCacheDataRequest.BusinessUnit == _fileShareServiceConfig.Value.BusinessUnit && productCode == _fileShareServiceConfig.Value.ProductType)
            {
                string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);
                List<string> tableNames = new() { _cacheConfiguration.Value.FssWeeklyBatchSearchTableName , _cacheConfiguration.Value.FssCacheResponseTableName };
                await _azureTableStorageClient.DeleteTablesAsync(tableNames, connectionString);
                return true;
            }
            return false;
        }
    }
}
