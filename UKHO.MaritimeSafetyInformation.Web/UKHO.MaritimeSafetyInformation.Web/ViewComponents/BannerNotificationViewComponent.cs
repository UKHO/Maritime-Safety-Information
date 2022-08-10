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
        private readonly IUserService _userService;

        public BannerNotificationViewComponent(IHttpContextAccessor contextAccessor, ILogger<BannerNotificationViewComponent> logger, IMSIBannerNotificationService mSIBannerNotificationService, IUserService userService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _mSIBannerNotificationService = mSIBannerNotificationService;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                _logger.LogError(EventIds.BannerNotificationRequestStarted.ToEventId(), "Maritime safety information request to get banner notification message started for User:{SignInName} and IsDistributor:{IsDistributorUser} with _X-Correlation-ID:{CorrelationId}", _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());

                ViewBag.BannerNotificationMessage = await _mSIBannerNotificationService.GetBannerNotification();

                _logger.LogError(EventIds.BannerNotificationRequestCompleted.ToEventId(), "Maritime safety information request to get banner notification message completed for User:{SignInName} and IsDistributor:{IsDistributorUser} with _X-Correlation-ID:{CorrelationId}", _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());

                return View("~/Views/BannerNotification/index.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.BannerNotificationRequestFailed.ToEventId(), "Maritime safety information request to get banner notification message failed to return data with exception:{exceptionMessage} for User:{SignInName} and IsDistributor:{IsDistributorUser} with _X-Correlation-ID:{CorrelationId}", ex.Message, _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());
                throw;
            }
        }
    }
}
