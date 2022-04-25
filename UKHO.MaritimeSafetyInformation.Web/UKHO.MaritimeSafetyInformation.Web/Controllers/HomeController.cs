using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpContextAccessor contextAccessor, ILogger<HomeController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
