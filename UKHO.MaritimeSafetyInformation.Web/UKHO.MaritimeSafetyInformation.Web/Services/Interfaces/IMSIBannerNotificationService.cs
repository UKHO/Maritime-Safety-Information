using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IMSIBannerNotificationService
    {
        Task<MsiBannerNotificationEntity> GetBannerNotification();
    }
}
