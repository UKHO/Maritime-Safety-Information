using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

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

        public async Task<IActionResult> Index(int pageIndex = 1, int? warningType = null, int? year = null)
        {
            _logger.LogInformation(EventIds.MSIRnwAdminListStarted.ToEventId(), "Maritime safety information request to get RNW records for Admin started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminFilter = await _rnwService.GetRadioNavigationWarningsForAdmin(pageIndex, warningType, year, GetCurrentCorrelationId());
            ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminFilter.WarningTypes, "Id", "Name");
            ViewBag.Years = new SelectList(radioNavigationalWarningsAdminFilter.Years);

            _logger.LogInformation(EventIds.MSIRnwAdminListCompleted.ToEventId(), "Maritime safety information request to get RNW records for Admin completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            return View(radioNavigationalWarningsAdminFilter);
        }

        // GET: RadioNavigationalWarnings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            
            if (id == null)
            {
                _logger.LogInformation(EventIds.EditRNWRecordIDNotFound.ToEventId(), "Maritime safety information edit RNW record id not found for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return NotFound();
            }

            RadioNavigationalWarningsAdminList radioNavigationalWarningsAdminList = _rnwService.EditRadioNavigationWarningListForAdmin(id, GetCurrentCorrelationId());
            if (radioNavigationalWarningsAdminList == null)
            {
                _logger.LogInformation(EventIds.EditRNWListIsNull.ToEventId(), "Maritime safety information edit RNW list is null for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return NotFound();
            }
            return View(radioNavigationalWarningsAdminList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RadioNavigationalWarningsAdminList radioNavigationalWarningsAdminList)
        {
            _logger.LogInformation(EventIds.EditRNWRecordStart.ToEventId(), "Maritime safety information create new RNW record request started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            if (id != radioNavigationalWarningsAdminList.Id)
            {
                _logger.LogInformation(EventIds.EditRNWRecordIdMismatch.ToEventId(), "Maritime safety information edit RNW record id mismatched for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return NotFound();
            }
            
            else if (ModelState.IsValid)
            {
               bool result = await _rnwService.EditRadioNavigationWarningsRecord(radioNavigationalWarningsAdminList, GetCurrentCorrelationId());
               if (result)
                {
                    TempData["message"] = "Record updated successfully!";
                    _logger.LogInformation(EventIds.EditRNWRecordCompleted.ToEventId(), "Maritime safety information edit RNW record request updated successfully for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                    return RedirectToAction(nameof(Index), new { reLoadData = true });
                }
            }
            return View(radioNavigationalWarningsAdminList);
        }
    }
}
