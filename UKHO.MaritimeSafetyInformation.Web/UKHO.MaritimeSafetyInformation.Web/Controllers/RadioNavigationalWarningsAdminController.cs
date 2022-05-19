using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsAdminController : BaseController<RadioNavigationalWarningsAdminController>
    {
        private readonly IRNWService _rnwService;
        private readonly ILogger<RadioNavigationalWarningsAdminController> _logger;

        public RadioNavigationalWarningsAdminController(IHttpContextAccessor contextAccessor,
                                                        ILogger<RadioNavigationalWarningsAdminController> logger,
                                                        IRNWService rnwService) : base(contextAccessor, logger)
        {
            _rnwService = rnwService;
            _logger = logger;
        }

        // GET: RadioNavigationalWarnings
        public async Task<IActionResult> Index(int pageIndex = 1, int warningType = 0, string year = "", bool reLoadData = false)

        {
            return View();
        }

        // GET: RadioNavigationalWarnings/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.WarningType = await _rnwService.GetWarningTypes();
                    
            return View();
        }

        // POST: RadioNavigationalWarnings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RadioNavigationalWarnings radioNavigationalWarnings)
        {
            _logger.LogInformation(EventIds.MSICreateNewRNWRecordStart.ToEventId(), "Maritime safety information create new RNW record request started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            if (ModelState.IsValid)
            {
                bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(radioNavigationalWarnings, GetCurrentCorrelationId());

                if (result)
                {
                    TempData["message"] = "Record created successfully!";
                    _logger.LogInformation(EventIds.MSICreateNewRNWRecordCompleted.ToEventId(), "Maritime safety information create new RNW record request completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                    
                    return RedirectToAction(nameof(Index), new { reLoadData = true });
                }
            }

            return View(radioNavigationalWarnings);
        }
    }
}
