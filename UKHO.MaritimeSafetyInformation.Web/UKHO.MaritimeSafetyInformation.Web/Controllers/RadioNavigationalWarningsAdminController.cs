using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
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

        // GET: RadioNavigationalWarnings/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.WarningType = await _rnwService.GetWarningTypes();

            return View();
        }

        // POST: RadioNavigationalWarnings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RadioNavigationalWarning radioNavigationalWarning)
        {
            _logger.LogInformation(EventIds.CreateNewRNWRecordStart.ToEventId(), "Maritime safety information create new RNW record request started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            if (ModelState.IsValid)
            {
                bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(radioNavigationalWarning, GetCurrentCorrelationId());

                if (result)
                {
                    TempData["message"] = "Record created successfully!";
                    _logger.LogInformation(EventIds.CreateNewRNWRecordCompleted.ToEventId(), "Maritime safety information create new RNW record request completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                    return RedirectToAction(nameof(Index), new { reLoadData = true });
                }
            }

            return View(radioNavigationalWarning);
        }


        public async Task<IActionResult> Index(int pageIndex = 1, int? warningType = null, int? year = null)
        {
            _logger.LogInformation(EventIds.RNWAdminListStarted.ToEventId(), "Maritime safety information request to get RNW records for Admin started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            RadioNavigationalWarningsAdminFilter radioNavigationalWarningsAdminFilter = await _rnwService.GetRadioNavigationWarningsForAdmin(pageIndex, warningType, year, GetCurrentCorrelationId());
            ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminFilter.WarningTypes, "Id", "Name");
            ViewBag.Years = new SelectList(radioNavigationalWarningsAdminFilter.Years);
                    
            _logger.LogInformation(EventIds.RNWAdminListCompleted.ToEventId(), "Maritime safety information request to get RNW records for Admin completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            return View(radioNavigationalWarningsAdminFilter);
        }

        #region Edit Radio Navigation Warning
        // GET: RadioNavigationalWarnings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                _logger.LogInformation(EventIds.EditRNWRecordIDNotFound.ToEventId(), "Maritime safety information edit RNW record id not found for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return NotFound();
            }

            EditRadioNavigationalWarningsAdmin radioNavigationalWarningsAdmin = _rnwService.EditRadioNavigationWarningListForAdmin(id, GetCurrentCorrelationId());
            if (radioNavigationalWarningsAdmin == null)
            {
                _logger.LogInformation(EventIds.EditRNWListIsNull.ToEventId(), "Maritime safety information edit RNW list is null for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return NotFound();
            }
            
            List<SelectListItem> list = new();

            foreach (WarningType item in await _rnwService.GetWarningTypes())
            {
                if (item.Name == radioNavigationalWarningsAdmin.WarningTypeName)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = Convert.ToString(item.Id),
                        Selected = true
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = Convert.ToString(item.Id),
                    });
                }
            }
            ViewBag.WarningType = list;
            return View(radioNavigationalWarningsAdmin);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRadioNavigationalWarningsAdmin radioNavigationalWarningsAdmin)
        {
            _logger.LogInformation(EventIds.EditRNWRecordStart.ToEventId(), "Maritime safety information Edit RNW record request started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            if (id != radioNavigationalWarningsAdmin.Id)
            {
                _logger.LogInformation(EventIds.EditRNWRecordIdMismatch.ToEventId(), "Maritime safety information edit RNW record id mismatched for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return NotFound();
            }

            else if (ModelState.IsValid)
            {
                bool result = await _rnwService.EditRadioNavigationWarningsRecord(radioNavigationalWarningsAdmin, GetCurrentCorrelationId());
                if (result)
                {
                    TempData["message"] = "Record updated successfully!";
                    _logger.LogInformation(EventIds.EditRNWRecordCompleted.ToEventId(), "Maritime safety information edit RNW record request updated successfully for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                    return RedirectToAction(nameof(Index), new { reLoadData = true });
                }
            }
            return View(radioNavigationalWarningsAdmin);
        }
        #endregion
    }
}
