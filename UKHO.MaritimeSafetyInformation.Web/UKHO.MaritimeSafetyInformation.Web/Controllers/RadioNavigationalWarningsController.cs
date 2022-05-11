using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsController : BaseController<RadioNavigationalWarningsController>
    {
        private readonly IRnwService _rnwService;
        private readonly ILogger<RadioNavigationalWarningsController> _logger;

        public RadioNavigationalWarningsController(IHttpContextAccessor contextAccessor,
                                                   ILogger<RadioNavigationalWarningsController> logger,
                                                   IRnwService rnwService) : base(contextAccessor, logger)
        {
            _rnwService = rnwService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int warningType = 0, bool reLoadData = true)
        {
            _logger.LogInformation(EventIds.MSIGetRnwDetailStarted.ToEventId(), "Maritime safety information request to get RNW detail for public started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = await _rnwService.GetRadioNavigationalWarningsData(warningType, reLoadData, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.MSIGetRnwDetailCompleted.ToEventId(), "Maritime safety information request to get RNW detail for public completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return View("~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml", radioNavigationalWarningsData);
        }
    }
}
