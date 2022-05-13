using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class NoticesToMarinersController : BaseController<NoticesToMarinersController>
    {

        private readonly ILogger<NoticesToMarinersController> _logger;
        private readonly INMDataService _nMDataService;
        private ShowWeeklyFilesResponseModel showWeeklyFiles = new ShowWeeklyFilesResponseModel();
        public NoticesToMarinersController(INMDataService nMDataService, IHttpContextAccessor contextAccessor, ILogger<NoticesToMarinersController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
            _nMDataService = nMDataService;
        }

        public IActionResult Index()
        {
            //ShowWeeklyFilesResponseModel showWeeklyFiles = new ShowWeeklyFilesResponseModel();
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());
            //return View("~/Views/NoticesToMariners/FilterWeeklyFiles.cshtml");
            showWeeklyFiles.Years = _nMDataService.GetAllYearsSelectItem(GetCurrentCorrelationId());
            showWeeklyFiles.Weeks = _nMDataService.GetAllWeeksOfYearSelectItem(Convert.ToInt32(showWeeklyFiles.Years[1].Value), GetCurrentCorrelationId());
            return View(showWeeklyFiles);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(int? year, int? week)
        {
            List<ShowFilesResponseModel> listFiles = new List<ShowFilesResponseModel>();
            if (year.HasValue)
            {
                showWeeklyFiles.Years = _nMDataService.GetAllYearsSelectItem(GetCurrentCorrelationId());
                showWeeklyFiles.Weeks = _nMDataService.GetAllWeeksOfYearSelectItem((int)year, GetCurrentCorrelationId());
            }

            if (year.HasValue && week.HasValue)
            {
                showWeeklyFiles.Years = _nMDataService.GetAllYearsSelectItem(GetCurrentCorrelationId());
                showWeeklyFiles.Weeks = _nMDataService.GetAllWeeksOfYearSelectItem((int)year, GetCurrentCorrelationId());

                showWeeklyFiles.ShowFilesResponseModel = await _nMDataService.GetWeeklyBatchFiles((int)year, (int)week, GetCurrentCorrelationId());
            }
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());
            //return View("~/Views/NoticesToMariners/FilterWeeklyFiles.cshtml");

            return View(showWeeklyFiles);
        }

        public IActionResult DailyFiles()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request to get daily NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/ShowDailyFiles.cshtml");
        }

        public IActionResult LoadYears()
        {
            return Json(_nMDataService.GetAllYears(GetCurrentCorrelationId()));
        }

        public IActionResult LoadWeeks(int year)
        {
            ViewBag.Weeks = _nMDataService.GetAllWeeksOfYear(year, GetCurrentCorrelationId());
            //return Json(_nMDataService.GetAllWeeksOfYear(year, GetCurrentCorrelationId()));
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestStarted.ToEventId(), "Maritime safety information request to show weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await _nMDataService.GetWeeklyBatchFiles(year, week, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestCompleted.ToEventId(), "Maritime safety information request to show weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            //return PartialView("~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml", listFiles);
            return View("Index", listFiles);
        }

        public async Task<IActionResult> ShowDailyFilesAsync()
        {
            _logger.LogInformation(EventIds.ShowDailyFilesRequest.ToEventId(), "Maritime safety information request to show daily NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowDailyFilesResponseModel> showDailyFilesResponseModels = await _nMDataService.GetDailyBatchDetailsFiles(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.ShowDailyFilesCompleted.ToEventId(), "Maritime safety information request to show daily NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/ShowDailyFilesList.cshtml", showDailyFilesResponseModels);

        }
    }
}
