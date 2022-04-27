using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.DTO;
using UKHO.MaritimeSafetyInformation.Common.Models.RNW;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class RadioNavigationalWarningsAdminController : Controller
    {
        private readonly RadioNavigationalWarningsContext _context;
        private readonly IRnwRepository _iRnwRepository;

        public RadioNavigationalWarningsAdminController(RadioNavigationalWarningsContext context,
                                                   IRnwRepository iRnwRepository)
        {
            _context = context;
            _iRnwRepository = iRnwRepository;
        }

        // GET: RadioNavigationalWarnings
        public async Task<IActionResult> Index()
        {
            RadioNavigationalWarningsAdminList RadioNavigationalWarningsAdminList =_iRnwRepository.GetRadioNavigationForAdmin();
            return  View(RadioNavigationalWarningsAdminList);
        }

        // GET: RadioNavigationalWarnings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RadioNavigationalWarnings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WarningType,Reference,DateTimeGroup,Summary,Content,ExpiryDate,IsDeleted")] RadioNavigationalWarnings radioNavigationalWarnings)
        {
            if (ModelState.IsValid)
            {
                _iRnwRepository.AddRadioNavigation(radioNavigationalWarnings);

                return RedirectToAction(nameof(Index));
            }
            return View(radioNavigationalWarnings);
        }
    }
}
