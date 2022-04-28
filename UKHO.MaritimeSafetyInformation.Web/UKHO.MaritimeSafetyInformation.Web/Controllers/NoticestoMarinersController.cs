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
            return Json(nMDataService.GetPastYears());
        }

        public IActionResult LoadWeeks(int year)
        {
            return Json(nMDataService.GetAllWeeksofYear(year));
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.RetrievalOfMSIShowWeeklyFilesRequest.ToEventId(), "Maritime safety information request for show weekly files requested for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await nMDataService.GetBatchDetailsFiles(year, week, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.RetrievalOfMSIShowWeeklyFilesCompleted.ToEventId(), "Maritime safety information request for show weekly files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/_WeeklyFilesList.cshtml",listFiles);
        }
    }
}
