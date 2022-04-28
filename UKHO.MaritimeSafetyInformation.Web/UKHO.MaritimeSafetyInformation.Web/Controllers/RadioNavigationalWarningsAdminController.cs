using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            RadioNavigationalWarningsAdminListPage radioNavigationalWarningsAdminList = _iRnwRepository.GetRadioNavigationForAdmin(pageIndex);
            // ViewBag.WarningTypes = new SelectList(radioNavigationalWarningsAdminList.WarningTypes, "Id", "Name");
            return View(radioNavigationalWarningsAdminList);
        }
    }
}
