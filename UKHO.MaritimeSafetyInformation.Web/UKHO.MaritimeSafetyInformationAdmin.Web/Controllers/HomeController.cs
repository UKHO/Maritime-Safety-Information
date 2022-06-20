using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformationAdmin.Web.Controllers
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

        [Route("/error")]
        public IActionResult Error()
        {
            string correlationId = GetCurrentCorrelationId();
            ViewData["CurrentCorrelationId"] = correlationId;
            IExceptionHandlerPathFeature exceptionDetails = _contextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError(EventIds.SystemError.ToEventId(), "System error has occurred while processing request with exception:{ex}, at exception path:{path} for correlationId:{correlationId}", exceptionDetails?.Error.Message, exceptionDetails?.Path, correlationId);
            return View();
        }
    }
}
