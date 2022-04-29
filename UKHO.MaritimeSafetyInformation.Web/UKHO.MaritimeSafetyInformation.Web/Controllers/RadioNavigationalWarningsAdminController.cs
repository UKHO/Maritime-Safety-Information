using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
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

        // GET: RadioNavigationalWarnings
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: RadioNavigationalWarnings/Create
        public IActionResult Create()
        {
            ViewBag.WarningType = _iRnwRepository.GetWarningType();
         
            return View();
        }

        // POST: RadioNavigationalWarnings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WarningType,Reference,DateTimeGroup,Summary,Content,ExpiryDate")] RadioNavigationalWarnings radioNavigationalWarnings)
        {
            if (ModelState.IsValid)
            {
                await _iRnwRepository.AddRadioNavigation(radioNavigationalWarnings);

                TempData["message"] = "Record created successfully!";

                return RedirectToAction(nameof(Index));
            }

            return View(radioNavigationalWarnings);
        }
    }
}
