using Ganss.Xss;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Logging;
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
        private readonly ILogger<MSIBannerNotificationService> _logger;

        private string ConnectionString => _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

        public MSIBannerNotificationService(IOptions<CacheConfiguration> cacheConfiguration, IAzureStorageService azureStorageService, IAzureTableStorageClient azureTableStorageClient, IOptions<BannerNotificationConfiguration> bannerNotificationConfiguration, ILogger<MSIBannerNotificationService> logger)
        {
            _cacheConfiguration = cacheConfiguration;
            _azureStorageService = azureStorageService;
            _azureTableStorageClient = azureTableStorageClient;
            _bannerNotificationConfiguration = bannerNotificationConfiguration;
            _logger = logger;
        }

        public async Task<string> GetBannerNotification(string correlationId)
        {
            try
            {
                string bannerNotificationMessage = null;
                _logger.LogInformation(EventIds.GetBannerNotificationMessageFromTableStarted.ToEventId(), "Maritime safety information request to get banner notification message from azure table started for _X-Correlation-ID:{CorrelationId}", correlationId);

                if (_bannerNotificationConfiguration.Value.IsBannerNotificationEnabled)
                {
                    MsiBannerNotificationEntity msiBannerNotificationEntity = await _azureTableStorageClient.GetSingleEntityAsync(_bannerNotificationConfiguration.Value.MsiBannerNotificationTableName, ConnectionString);
                    if (msiBannerNotificationEntity != null)
                    {
                        HtmlSanitizer sanitizer = new();
                        bannerNotificationMessage = sanitizer.Sanitize(msiBannerNotificationEntity.Message);
                    }
                }

                _logger.LogInformation(EventIds.GetBannerNotificationMessageFromTableCompleted.ToEventId(), "Maritime safety information request to get banner notification message from azure table completed for _X-Correlation-ID:{CorrelationId}", correlationId);
                return bannerNotificationMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetBannerNotificationMessageFromTableFailed.ToEventId(), "Maritime safety information request to get banner notification message from azure table failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                return null ;
            }
        }
    }
}
