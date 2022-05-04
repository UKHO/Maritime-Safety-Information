using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsAdminController : BaseController<RadioNavigationalWarningsAdminController>
    {
        private readonly IRnwRepository _iRnwRepository;
        private readonly ILogger<RadioNavigationalWarningsAdminController> _logger;

        public RadioNavigationalWarningsAdminController(IHttpContextAccessor contextAccessor,
                                                        ILogger<RadioNavigationalWarningsAdminController> logger,
                                                        IRnwRepository iRnwRepository) : base(contextAccessor, logger)
        {
            _iRnwRepository = iRnwRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int warningType = 0, string year = "", bool reLoadData = false)
        {
            RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminFilter = _iRnwRepository.GetRadioNavigationWarningsForAdmin(pageIndex, warningType, year, reLoadData, GetCurrentCorrelationId());
            ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminFilter.WarningTypes, "Id", "Name");
            ViewBag.Years = new SelectList(radioNavigationalWarningsAdminFilter.Years);
            return View(radioNavigationalWarningsAdminFilter);
        }
    }
}
