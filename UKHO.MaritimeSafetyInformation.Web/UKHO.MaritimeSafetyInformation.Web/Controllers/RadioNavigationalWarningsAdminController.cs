using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsAdminController : Controller
    {
        private readonly IRnwRepository _iRnwRepository;

        public RadioNavigationalWarningsAdminController(IRnwRepository iRnwRepository)
        {
            _iRnwRepository = iRnwRepository;
        }

        public async Task<IActionResult> Index(int warningType =0,string year= "", int pageIndex = 1)
        {
            RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminFilter = _iRnwRepository.GetRadioNavigationForAdmin(pageIndex);
            ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminFilter.WarningTypes, "Id", "Name");
            ViewBag.Years = new SelectList(radioNavigationalWarningsAdminFilter.Years);
            return View(radioNavigationalWarningsAdminFilter);
        }
    }
}
