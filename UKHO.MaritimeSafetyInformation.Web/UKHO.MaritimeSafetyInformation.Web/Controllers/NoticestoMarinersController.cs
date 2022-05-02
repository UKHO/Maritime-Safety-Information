using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class NoticesToMarinersController : BaseController<NoticesToMarinersController>
    {

        private readonly ILogger<NoticesToMarinersController> _logger;
        private readonly INMDataService nMDataService;
        public NoticesToMarinersController(INMDataService nMDataService, IHttpContextAccessor contextAccessor, ILogger<NoticesToMarinersController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
            this.nMDataService = nMDataService;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/ShowWeeklyFiles.cshtml");
        }

        public IActionResult LoadYears()
        {
            return Json(nMDataService.GetPastYears(GetCurrentCorrelationId()));
        }

        public IActionResult LoadWeeks(int year)
        {
            return Json(nMDataService.GetAllWeeksofYear(year, GetCurrentCorrelationId()));
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestStarted.ToEventId(), "Maritime safety information request for show weekly files requested for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await nMDataService.GetNMBatchFiles(year, week, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestCompleted.ToEventId(), "Maritime safety information request for show weekly files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/_WeeklyFilesList.cshtml",listFiles);
        }

        public async Task<IActionResult> ShowDailyFilesAsync()
        {
            _logger.LogInformation(EventIds.MSIShowDailyFilesRequest.ToEventId(), "Maritime safety information request for show daily files requested:{correlationId}", GetCurrentCorrelationId());

            List<ShowDailyFilesResponseModel> Entries = await nMDataService.GetDailyBatchDetailsFiles(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.MSIShowDailyFilesCompleted.ToEventId(), "Maritime safety information request for show daily files completed:{correlationId}", GetCurrentCorrelationId());

            return View(Entries);
        }


    }
}
