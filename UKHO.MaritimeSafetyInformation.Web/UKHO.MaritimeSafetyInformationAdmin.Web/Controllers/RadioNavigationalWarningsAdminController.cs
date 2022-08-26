using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformationAdmin.Web.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize(Roles = "rnw-admin")]
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

            return View("~/Views/RadioNavigationalWarningsAdmin/Create.cshtml");
        }

        // POST: RadioNavigationalWarnings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RadioNavigationalWarning radioNavigationalWarning)
        {
            _logger.LogInformation(EventIds.CreateNewRNWRecordStart.ToEventId(), "Create RNW request started for _X-Correlation-ID:{correlationId}. Requested by user: {user}", GetCurrentCorrelationId(),User.Identity.Name);

            bool skipDuplicateReferenceCheck = false;
            if (!string.IsNullOrWhiteSpace(Request.Form["SkipDuplicateReferenceCheck"]))
            {
                skipDuplicateReferenceCheck = Request.Form["SkipCheckDuplicate"] == "Yes";
            }

            if (ModelState.IsValid)
            {
                bool isNewRecordCreated = await _rnwService.CreateNewRadioNavigationWarningsRecord(radioNavigationalWarning, GetCurrentCorrelationId(), skipDuplicateReferenceCheck, User.Identity.Name);

                if (isNewRecordCreated)
                {
                    TempData["message"] = "Record created successfully!";
                    _logger.LogInformation(EventIds.CreateNewRNWRecordCompleted.ToEventId(), "Create RNW request completed successfully with following values WarningType:{WarningType}, Reference:{Reference}, DateTime:{DateTime}, Description:{Description}, Text:{Text}, Expiry Date:{ExpiryDate} for _X-Correlation-ID:{correlationId}. Requested by user: {user}", radioNavigationalWarning.WarningType, radioNavigationalWarning.Reference, radioNavigationalWarning.DateTimeGroup, radioNavigationalWarning.Summary, RnwHelper.FormatContent(radioNavigationalWarning.Content), radioNavigationalWarning.ExpiryDate, GetCurrentCorrelationId(), User.Identity.Name);

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.WarningType = await _rnwService.GetWarningTypes();
                    TempData["message"] = "A warning record with this reference number already exists. Would you like to add another record with the same reference?";

                    return View("~/Views/RadioNavigationalWarningsAdmin/Create.cshtml", radioNavigationalWarning);
                }
            }

            return View("~/Views/RadioNavigationalWarningsAdmin/Create.cshtml", radioNavigationalWarning);
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int? warningType = null, int? year = null)
        {
            _logger.LogInformation(EventIds.RNWAdminListStarted.ToEventId(), "RNW get request started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            RadioNavigationalWarningsAdminFilter radioNavigationalWarningsAdminFilter = await _rnwService.GetRadioNavigationWarningsForAdmin(pageIndex, warningType, year, GetCurrentCorrelationId());
            ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminFilter.WarningTypes, "Id", "Name");
            ViewBag.Years = new SelectList(radioNavigationalWarningsAdminFilter.Years);

            _logger.LogInformation(EventIds.RNWAdminListCompleted.ToEventId(), "RNW get request completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            return View(radioNavigationalWarningsAdminFilter);
        }

        // GET: RadioNavigationalWarningsAdmin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            RadioNavigationalWarning radioNavigationalWarning = _rnwService.GetRadioNavigationalWarningById(id, GetCurrentCorrelationId());
            if (radioNavigationalWarning != null)
            {
                ViewBag.WarningType = await _rnwService.GetWarningTypes();
                return View("~/Views/RadioNavigationalWarningsAdmin/Edit.cshtml", radioNavigationalWarning);
            }
            _logger.LogInformation(EventIds.EditRNWRecordNotFound.ToEventId(), "RNW record not found for Id:{id} with _X-Correlation-ID:{correlationId}", id, GetCurrentCorrelationId());
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RadioNavigationalWarning radioNavigationalWarning)
        {
            _logger.LogInformation(EventIds.EditRNWRecordStarted.ToEventId(), "RNW update request started for Id:{id} with _X-Correlation-ID:{correlationId}. Requested by user: {user}", radioNavigationalWarning.Id, GetCurrentCorrelationId(), User.Identity.Name);

            if (ModelState.IsValid)
            {
                bool result = await _rnwService.EditRadioNavigationalWarningsRecord(radioNavigationalWarning, GetCurrentCorrelationId());
                if (result)
                {
                    TempData["message"] = "Record updated successfully!";
                    _logger.LogInformation(EventIds.EditRNWRecordCompleted.ToEventId(), "RNW record updated successfully with following values Id:{id}, WarningType:{WarningType}, Reference:{Reference}, DateTime:{DateTime}, Description:{Description}, Text:{Text}, Expiry Date:{ExpiryDate}, Deleted:{IsDeleted} for _X-Correlation-ID:{correlationId}. Requested by user: {user}", radioNavigationalWarning.Id, radioNavigationalWarning.WarningType, radioNavigationalWarning.Reference, radioNavigationalWarning.DateTimeGroup, radioNavigationalWarning.Summary, RnwHelper.FormatContent(radioNavigationalWarning.Content), radioNavigationalWarning.ExpiryDate, radioNavigationalWarning.IsDeleted, GetCurrentCorrelationId(), User.Identity.Name);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(radioNavigationalWarning);
        }
    }
}
