using Ganss.XSS;
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
        private readonly IOptions<BannerNotificationConfiguration> _bannerNotificationConfiguration;
        
        private string ConnectionString => _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

        public MSIBannerNotificationService(IOptions<CacheConfiguration> cacheConfiguration, IAzureStorageService azureStorageService, IAzureTableStorageClient azureTableStorageClient, IOptions<BannerNotificationConfiguration> bannerNotificationConfiguration)
        {
            _cacheConfiguration = cacheConfiguration;
            _azureStorageService = azureStorageService;
            _azureTableStorageClient = azureTableStorageClient;
            _bannerNotificationConfiguration = bannerNotificationConfiguration; 
        }

        public async Task<string> GetBannerNotification()
        {
            string bannerNotificationMessage = null;

            if (_bannerNotificationConfiguration.Value.IsBannerNotificationEnabled)
            {
                MsiBannerNotificationEntity msiBannerNotificationEntity = await _azureTableStorageClient.GetSingleEntityAsync(_cacheConfiguration.Value.MsiBannerNotificationTableName, ConnectionString);
                if (msiBannerNotificationEntity != null)
                {
                    HtmlSanitizer sanitizer = new ();
                    bannerNotificationMessage = sanitizer.Sanitize(msiBannerNotificationEntity.Message);
                }
            }

            return bannerNotificationMessage;
        }
    }
}
