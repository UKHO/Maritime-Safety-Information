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

        [HttpGet]
        [Route("/RadioNavigationalWarnings")]
        public async Task<IActionResult> Index()
        {
            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = new();

            try
            {
                _logger.LogInformation(EventIds.RNWListDetailStarted.ToEventId(), "Maritime safety information request to get RNW details started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                ViewBag.HasError = false;

                radioNavigationalWarningsData = await _rnwService.GetRadioNavigationalWarningsData(GetCurrentCorrelationId());

                ViewBag.LastModifiedDateTime = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(GetCurrentCorrelationId());

                _logger.LogInformation(EventIds.RNWListDetailCompleted.ToEventId(), "Maritime safety information request to get RNW details completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                return View("~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml", radioNavigationalWarningsData);
            }
            catch (Exception ex)
            {
                ViewBag.LastModifiedDateTime = DateTime.MinValue;
                ViewBag.HasError = true;
                ViewData["CurrentCorrelationId"] = GetCurrentCorrelationId();

                _logger.LogError(EventIds.RNWListDetailFailed.ToEventId(), "Maritime safety information request to showget RNW details failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());

                return View("~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml", radioNavigationalWarningsData);
            }
        }

        [HttpGet]
        [Route("/RadioNavigationalWarnings/About")]
        public async Task<IActionResult> About()
        {
            try
            {
                _logger.LogInformation(EventIds.RNWAboutStarted.ToEventId(), "Maritime safety information request for About RNW started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                ViewBag.LastModifiedDateTime = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(GetCurrentCorrelationId());

                _logger.LogInformation(EventIds.RNWAboutCompleted.ToEventId(), "Maritime safety information request for About RNW completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            }
            catch (Exception ex)
            {
                ViewBag.LastModifiedDateTime = DateTime.MinValue;

                _logger.LogError(EventIds.RNWAboutFailed.ToEventId(), "Maritime safety information request to showget RNW details failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ShowSelection()
        {
            int[] selectedIds = Array.Empty<int>();
            string data = Request.Form["showSelectionId"];
            if (!string.IsNullOrWhiteSpace(data))
            {
                selectedIds = data.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            }

            ViewBag.LastModifiedDateTime = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(GetCurrentCorrelationId());

            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = await _rnwService.GetSelectedRadioNavigationalWarningsData(selectedIds, GetCurrentCorrelationId());

            return View("~/Views/RadioNavigationalWarnings/ShowSelection.cshtml", radioNavigationalWarningsData);
        }
    }
}
