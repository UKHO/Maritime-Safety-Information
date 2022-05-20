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
            _logger.LogInformation(EventIds.MSIGetRnwDetailStarted.ToEventId(), "Maritime safety information request to get RNW detail started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = await _rnwService.GetRadioNavigationalWarningsData(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.MSIGetRnwDetailCompleted.ToEventId(), "Maritime safety information request to get RNW detail completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return View("~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml", radioNavigationalWarningsData);
        }
    }
}
