using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
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
                _logger.LogInformation(EventIds.MSICreateNewRNWRecordStart.ToEventId(), "Maritime safety information create new RNW record request started for correlationId:{correlationId}", GetCurrentCorrelationId());

                await _iRnwRepository.AddRadioNavigationWarnings(radioNavigationalWarnings);

                _logger.LogInformation(EventIds.MSICreateNewRNWRecordCompleted.ToEventId(), "Maritime safety information create new RNW record request completed for correlationId:{correlationId}", GetCurrentCorrelationId());

                TempData["message"] = "Record created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(radioNavigationalWarnings);
        }
    }
}
