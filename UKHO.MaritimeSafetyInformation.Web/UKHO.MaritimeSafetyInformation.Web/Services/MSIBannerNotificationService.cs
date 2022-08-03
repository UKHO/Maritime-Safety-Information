using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class MSIBannerNotificationService : IMSIBannerNotificationService
    {
        private readonly IOptions<CacheConfiguration> _cacheConfiguration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IAzureTableStorageClient _azureTableStorageClient;
        private string ConnectionString => _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

        public MSIBannerNotificationService(IOptions<CacheConfiguration> cacheConfiguration, IAzureStorageService azureStorageService, IAzureTableStorageClient azureTableStorageClient)
        {
            _cacheConfiguration = cacheConfiguration;
            _azureStorageService = azureStorageService;
            _azureTableStorageClient = azureTableStorageClient;
        }

        public async Task GetBannerNotification()
        {
            MsiBannerNotificationEntity msiBannerNotificationEntity = await _azureTableStorageClient.GetAllEntityAsync(_cacheConfiguration.Value.MsiBannerNotificationTableName, ConnectionString);

            Global.Name = msiBannerNotificationEntity.Message;
        }
    }
}
