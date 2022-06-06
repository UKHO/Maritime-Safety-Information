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
                    _logger.LogInformation(EventIds.CreateNewRNWRecordCompleted.ToEventId(), "Maritime safety information create new RNW record request completed successfully with record as WarningType:{WarningType}, Reference:{Reference}, DateTime:{DateTime}, Description:{Description}, Text:{Text}, Expiry Date:{ExpiryDate} for _X-Correlation-ID:{correlationId}", radioNavigationalWarning.WarningType, radioNavigationalWarning.Reference, radioNavigationalWarning.DateTimeGroup, radioNavigationalWarning.Summary, radioNavigationalWarning.Content, radioNavigationalWarning.ExpiryDate, GetCurrentCorrelationId());
                    return RedirectToAction(nameof(Index));
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

       
        // GET: RadioNavigationalWarnings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            EditRadioNavigationalWarningsAdmin radioNavigationalWarningsAdmin = _rnwService.GetRadioNavigationalWarningById(id, GetCurrentCorrelationId());
            if (radioNavigationalWarningsAdmin == null)
            {
                _logger.LogInformation(EventIds.EditRNWRecordNotFound.ToEventId(), "Maritime safety information edit RNW record not found for id:{id} with _X-Correlation-ID:{correlationId}", id, GetCurrentCorrelationId());
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
            ViewBag.WarningType = list;//await _rnwService.GetWarningTypes(); 
            return View(radioNavigationalWarningsAdmin);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRadioNavigationalWarningsAdmin radioNavigationalWarningsAdmin)
        {
            _logger.LogInformation(EventIds.EditRNWRecordStarted.ToEventId(), "Maritime safety information Edit RNW record request started for id:{id} with _X-Correlation-ID:{correlationId}", radioNavigationalWarningsAdmin.Id, GetCurrentCorrelationId());

            if (ModelState.IsValid)
            {
                bool result = await _rnwService.EditRadioNavigationalWarningsRecord(radioNavigationalWarningsAdmin, GetCurrentCorrelationId());
                if (result)
                {
                    TempData["message"] = "Record updated successfully!";
                    _logger.LogInformation(EventIds.EditRNWRecordCompleted.ToEventId(), "Maritime safety information edit RNW record request updated successfully with record as WarningType:{WarningType}, Reference:{Reference}, DateTime:{DateTime}, Description:{Description}, Text:{Text}, Expiry Date:{ExpiryDate}, Deleted:{IsDeleted} for _X-Correlation-ID:{correlationId}", radioNavigationalWarningsAdmin.WarningType, radioNavigationalWarningsAdmin.Reference, radioNavigationalWarningsAdmin.DateTimeGroup, radioNavigationalWarningsAdmin.Summary, radioNavigationalWarningsAdmin.Content, radioNavigationalWarningsAdmin.ExpiryDate, radioNavigationalWarningsAdmin.IsDeleted, GetCurrentCorrelationId());
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(radioNavigationalWarningsAdmin);
        }
       
    }
}
