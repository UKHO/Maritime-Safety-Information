using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsController : BaseController<RadioNavigationalWarningsController>
    {
        private readonly IRNWService _rnwService;
        private readonly ILogger<RadioNavigationalWarningsController> _logger;

        public RadioNavigationalWarningsController(IHttpContextAccessor contextAccessor,
                                                   ILogger<RadioNavigationalWarningsController> logger,
                                                   IRNWService rnwService) : base(contextAccessor, logger)
        {
            _rnwService = rnwService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(EventIds.RNWListDetailStarted.ToEventId(), "Maritime safety information request to get RNW details started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = await _rnwService.GetRadioNavigationalWarningsData(GetCurrentCorrelationId());

            ViewBag.LastModifiedDateTime = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.RNWListDetailCompleted.ToEventId(), "Maritime safety information request to get RNW details completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return View("~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml", radioNavigationalWarningsData);
        }

        [HttpGet]
        public IActionResult About()
        {
            ViewBag.LastModifiedDateTime = "261613 UTC May 22";
            //TODO
            //ViewBag.LastModifiedDateTime = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(GetCurrentCorrelationId());

            return View();
        }
    }
}
