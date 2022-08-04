using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.ViewComponents
{
    [ViewComponent(Name = "BannerNotification")]
    public class BannerNotificationViewComponent : ViewComponent
    {
        private readonly IMSIBannerNotificationService _mSIBannerNotificationService;

        public BannerNotificationViewComponent(IMSIBannerNotificationService mSIBannerNotificationService)
        {
            _mSIBannerNotificationService = mSIBannerNotificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await _mSIBannerNotificationService.GetBannerNotification();

            return View("~/Views/BannerNotification/index.cshtml"); 
        }
    }
}
