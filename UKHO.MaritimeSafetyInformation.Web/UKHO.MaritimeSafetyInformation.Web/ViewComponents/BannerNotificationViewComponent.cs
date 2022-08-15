using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.ViewComponents
{
    [ViewComponent(Name = "BannerNotification")]
    public class BannerNotificationViewComponent : BaseViewComponent<BannerNotificationViewComponent>
    {
        private readonly IMSIBannerNotificationService _mSIBannerNotificationService;
        private readonly ILogger<BannerNotificationViewComponent> _logger;

        public BannerNotificationViewComponent(IHttpContextAccessor contextAccessor, ILogger<BannerNotificationViewComponent> logger, IMSIBannerNotificationService mSIBannerNotificationService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _mSIBannerNotificationService = mSIBannerNotificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            _logger.LogInformation(EventIds.BannerNotificationRequestStarted.ToEventId(), "Maritime safety information request to get banner notification message started for _X-Correlation-ID:{CorrelationId}", GetCurrentCorrelationId());

            ViewBag.BannerNotificationMessage = await _mSIBannerNotificationService.GetBannerNotification(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.BannerNotificationRequestCompleted.ToEventId(), "Maritime safety information request to get banner notification message completed for _X-Correlation-ID:{CorrelationId}", GetCurrentCorrelationId());

            return View("~/Views/BannerNotification/index.cshtml");
        }
    }
}
