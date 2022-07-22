using FluentValidation.Results;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.Validation;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class WebhookService : IWebhookService
    {

        private readonly IAzureTableStorageClient _azureTableStorageClient;
        private readonly ILogger<WebhookService> _logger;
        private readonly IOptions<CacheConfiguration> _cacheConfiguration;
        private readonly IEnterpriseEventCacheDataRequestValidator _enterpriseEventCacheDataRequestValidator;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private readonly IAzureStorageService _azureStorageService;

        public WebhookService(IAzureTableStorageClient azureTableStorageClient,
            IOptions<CacheConfiguration> cacheConfiguration,
            ILogger<WebhookService> logger,
            IEnterpriseEventCacheDataRequestValidator enterpriseEventCacheDataRequestValidator,
            IOptions<FileShareServiceConfiguration> fileShareServiceConfig,
            IAzureStorageService azureStorageService)
        {
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
            _logger = logger;
            _enterpriseEventCacheDataRequestValidator = enterpriseEventCacheDataRequestValidator;
            _fileShareServiceConfig = fileShareServiceConfig;
            _azureStorageService = azureStorageService;

        }
        public Task<ValidationResult> ValidateEventGridCacheDataRequest(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest)
        {
            return _enterpriseEventCacheDataRequestValidator.Validate(enterpriseEventCacheDataRequest);
        }

        public async Task DeleteSearchAndDownloadCacheData(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest, string correlationId)
        {
            var productCode = enterpriseEventCacheDataRequest.Attributes.Where(a => a.Key == "ProductCode").Select(a => a.Value).FirstOrDefault();
           
            if (ValidateCacheAttributeData(enterpriseEventCacheDataRequest.BusinessUnit, productCode))
            {
                string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

                await _azureTableStorageClient.DeleteTablesAsync(_cacheConfiguration.Value.FssWeeklyAttributeTableName, connectionString);
            }
        }

        private bool ValidateCacheAttributeData(string businessUnit, string productCode)
        {
            return (businessUnit == _fileShareServiceConfig.Value.BusinessUnit && productCode == _fileShareServiceConfig.Value.ProductType);
        }

    }
}
