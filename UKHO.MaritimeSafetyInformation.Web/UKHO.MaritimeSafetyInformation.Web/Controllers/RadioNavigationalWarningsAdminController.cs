﻿using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index(int pageIndex = 1, int warningType = 0, string year = "", bool reLoadData = true)
        {
            _logger.LogInformation(EventIds.MSIGetRnwForAdminStarted.ToEventId(), "Maritime safety information request to get RNW records for Admin started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminFilter = await _rnwService.GetRadioNavigationWarningsForAdmin(pageIndex, warningType, year, reLoadData, GetCurrentCorrelationId());
            ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminFilter.WarningTypes, "Id", "Name");
            ViewBag.Years = new SelectList(radioNavigationalWarningsAdminFilter.Years);

            _logger.LogInformation(EventIds.MSIGetRnwForAdminCompleted.ToEventId(), "Maritime safety information request to get RNW records for Admin completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            return View(radioNavigationalWarningsAdminFilter);
        }
    }
}