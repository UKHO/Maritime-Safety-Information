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
        public NoticesToMarinersController(INMDataService nMDataService, IHttpContextAccessor contextAccessor, ILogger<NoticesToMarinersController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
            _nMDataService = nMDataService;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());
            //return View("~/Views/NoticesToMariners/FilterWeeklyFiles.cshtml");
            return View();
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
            return Json(_nMDataService.GetAllWeeksOfYear(year, GetCurrentCorrelationId()));
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestStarted.ToEventId(), "Maritime safety information request to show weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await _nMDataService.GetWeeklyBatchFiles(year, week, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestCompleted.ToEventId(), "Maritime safety information request to show weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml", listFiles);
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
