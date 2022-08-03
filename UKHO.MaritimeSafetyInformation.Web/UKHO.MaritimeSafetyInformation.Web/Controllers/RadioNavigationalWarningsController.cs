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
        private readonly IMSIBannerNotificationService _mSIBannerNotificationService;

        public RadioNavigationalWarningsController(IHttpContextAccessor contextAccessor,
                                                   ILogger<RadioNavigationalWarningsController> logger,
                                                   IRNWService rnwService,
                                                   IMSIBannerNotificationService mSIBannerNotificationService) : base(contextAccessor, logger)
        {
            _rnwService = rnwService;
            _logger = logger;
            _mSIBannerNotificationService = mSIBannerNotificationService;
        }

        [HttpGet]
        [Route("/RadioNavigationalWarnings")]
        public async Task<IActionResult> Index()
        {
            await _mSIBannerNotificationService.GetBannerNotification();

            _logger.LogInformation(EventIds.RNWListDetailStarted.ToEventId(), "Maritime safety information request to get RNW details started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = await _rnwService.GetRadioNavigationalWarningsData(GetCurrentCorrelationId());

            ViewBag.LastModifiedDateTime = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.RNWListDetailCompleted.ToEventId(), "Maritime safety information request to get RNW details completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return View("~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml", radioNavigationalWarningsData);
        }

        [HttpGet]
        [Route("/RadioNavigationalWarnings/About")]
        public async Task<IActionResult>  About()
        {
            ViewBag.LastModifiedDateTime = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(GetCurrentCorrelationId());

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
