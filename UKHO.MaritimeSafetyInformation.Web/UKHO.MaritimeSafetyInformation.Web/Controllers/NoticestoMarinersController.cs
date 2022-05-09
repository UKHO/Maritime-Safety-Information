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
            return View("~/Views/NoticesToMariners/FilterWeeklyFiles.cshtml");
        }

         public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestStarted.ToEventId(), "Maritime safety information request to show weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await _nMDataService.GetWeeklyBatchFiles(year, week, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestCompleted.ToEventId(), "Maritime safety information request to show weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml", listFiles);
        }

        public async Task<IActionResult> GetAllYearandWeeks()
        {
            _logger.LogInformation(EventIds.NoticesToMarinersGetAllYearsandWeeksStarted.ToEventId(), "Maritime safety information request to Search Year and Week for NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());             

            List<YearWeekModel> listYear = await _nMDataService.GetAllYearWeek(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.NoticesToMarinersGetAllYearsandWeeksCompleted.ToEventId(), "Maritime safety information request to Search Year and Week for NM files completed with Year/Week count as:{listYear} for _X-Correlation-ID:{correlationId}", listYear.Count, GetCurrentCorrelationId());

            return Json(listYear);               
        }           
    }
}
