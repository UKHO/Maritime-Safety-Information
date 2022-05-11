using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsAdminController : BaseController<RadioNavigationalWarningsAdminController>
    {
        private readonly IRnwService _rnwService;
        private readonly ILogger<RadioNavigationalWarningsAdminController> _logger;

        public RadioNavigationalWarningsAdminController(IHttpContextAccessor contextAccessor,
                                                        ILogger<RadioNavigationalWarningsAdminController> logger,
                                                        IRnwService rnwService) : base(contextAccessor, logger)
        {
            _rnwService = rnwService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int warningType = 0, string year = "", bool reLoadData = true)
        {
            _logger.LogInformation(EventIds.MSIGetRnwForAdminStarted.ToEventId(), "Maritime safety information request to get RNW records for Admin started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminFilter = await _rnwService.GetRadioNavigationWarningsForAdmin(pageIndex, warningType, year, reLoadData, GetCurrentCorrelationId());
            ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminFilter.WarningTypes, "Id", "Name");
            ViewBag.Years = new SelectList(radioNavigationalWarningsAdminFilter.Years);

            _logger.LogInformation(EventIds.MSIGetRnwForAdminCompleted.ToEventId(), "Maritime safety information request to get RNW records for Admin completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            return View(radioNavigationalWarningsAdminFilter);
                    return RedirectToAction(nameof(Index), new { reLoadData = true });
                }
            }

            return View(radioNavigationalWarnings);
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
        else
        {
            TempData["message"] = "Failed to create record.";
            _logger.LogInformation(EventIds.MSICreateNewRNWRecordFailed.ToEventId(), "Maritime safety information create new RNW record request failed for _X-Correlation-ID", GetCurrentCorrelationId());

            return RedirectToAction(nameof(Index), new { reLoadData = true });
        }
    }

    return View(radioNavigationalWarnings);
}
    }
}
