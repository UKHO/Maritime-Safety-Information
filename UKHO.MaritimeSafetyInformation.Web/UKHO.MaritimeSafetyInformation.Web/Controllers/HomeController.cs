using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(IHttpContextAccessor contextAccessor, ILogger<HomeController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());

            return View();
        }

        [Route("/error")]
        public async Task<IActionResult> ErrorAsync()
        {
            string correlationId = GetCurrentCorrelationId();
            ViewData["CurrentCorrelationId"] = correlationId;
            IExceptionHandlerPathFeature exceptionDetails = _contextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionDetails != null && exceptionDetails.Error.InnerException is MsalUiRequiredException)
            {                
                string appLogInUrl = $"https://msi-dev.admiralty.co.uk/MicrosoftIdentity/Account/SignIn";
                await Request.HttpContext.SignOutAsync();
                _logger.LogError(EventIds.SystemError.ToEventId(), "User redirected to signin in case of MsalUiRequiredException exception:{ex} with correlationId:{correlationId}", exceptionDetails?.Error.InnerException.Message, correlationId);
                return Redirect(appLogInUrl);                
            }

            _logger.LogError(EventIds.SystemError.ToEventId(), "System error has occurred while processing request with exception:{ex}, at exception path:{path} for correlationId:{correlationId}", exceptionDetails?.Error.Message, exceptionDetails?.Path, correlationId);
            return View();
        }
    }
}
