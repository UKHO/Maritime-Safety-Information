using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<AzureAdB2C> _azureAdB2C;
        private readonly IMSIBannerNotificationService _mSIBannerNotificationService;

        public HomeController(IHttpContextAccessor contextAccessor, ILogger<HomeController> logger, IOptions<AzureAdB2C> azureAdB2C, IMSIBannerNotificationService mSIBannerNotificationService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _azureAdB2C = azureAdB2C;
            _mSIBannerNotificationService = mSIBannerNotificationService;
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());

            await _mSIBannerNotificationService.GetBannerNotification();

            return View();
        }

        [Route("/error")]
        public async Task<IActionResult> ErrorAsync()
        {
            string correlationId = GetCurrentCorrelationId();
            ViewData["CurrentCorrelationId"] = correlationId;
            IExceptionHandlerPathFeature exceptionDetails = _contextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // In case of MsalUiRequiredException redirect user to sign in to get the account/login hint
            if (exceptionDetails != null && exceptionDetails.Error.InnerException is MsalUiRequiredException)
            {                
                string appLogInUrl = $"{_azureAdB2C.Value.RedirectBaseUrl}/MicrosoftIdentity/Account/SignIn";
                await Request.HttpContext.SignOutAsync();                
                return Redirect(appLogInUrl);                
            }

            _logger.LogError(EventIds.SystemError.ToEventId(), "System error has occurred while processing request with exception:{ex}, at exception path:{path} for correlationId:{correlationId}", exceptionDetails?.Error.Message, exceptionDetails?.Path, correlationId);

            return View();
        }
    }
}
