using System.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformationAdmin.Web.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<AzureADConfiguration> _azureADConfiguration;

        public HomeController(IHttpContextAccessor contextAccessor, ILogger<HomeController> logger, IOptions<AzureADConfiguration> azureADConfiguration) : base(contextAccessor, logger)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _azureADConfiguration = azureADConfiguration;
        }

        [Route("/error")]
        public IActionResult Error()
        {
            string correlationId = GetCurrentCorrelationId();
            ViewData["CurrentCorrelationId"] = correlationId;
            IExceptionHandlerPathFeature exceptionDetails = _contextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError(EventIds.SystemError.ToEventId(), "System error has occurred while processing request with exception:{ex}, at exception path:{path} for correlationId:{correlationId}", exceptionDetails?.Error.Message, exceptionDetails?.Path, correlationId);
            return View();
        }

        [Route("/accessdenied")]
        public IActionResult AccessDenied()
        {
            string host = _contextAccessor.HttpContext.Request.Host.Value.ToString();
            _logger.LogError(EventIds.UnauthorizedAccess.ToEventId(), "Unauthorized page requested by user: {user}", User.Identity.Name);
            if (!string.IsNullOrEmpty(host) && "https://"+ host != _azureADConfiguration.Value.RedirectBaseUrl)
                return new RedirectResult(_azureADConfiguration.Value.RedirectBaseUrl + "/accessdenied");                
            else
                return View();
        }
    }
}
