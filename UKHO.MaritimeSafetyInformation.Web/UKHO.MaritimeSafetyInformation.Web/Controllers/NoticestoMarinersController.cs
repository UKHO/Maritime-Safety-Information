using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public IActionResult DailyFiles()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/ShowDailyFiles.cshtml");
        }

        public IActionResult LoadYears()
        {
            return Json(_nMDataService.GetAllYears(GetCurrentCorrelationId()));
        }

        public IActionResult LoadWeeks(int year)
        {
            return Json(_nMDataService.GetAllWeeksofYear(year, GetCurrentCorrelationId()));
        }

        public async Task<JsonResult> GetWeeklyFilesResultAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.MSIGetWeeklyFilesResultRequest.ToEventId(), "Maritime safety information request to get weekly NM files result for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await _nMDataService.GetWeeklyBatchFiles(year, week, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.MSIGetWeeklyFilesResultRequestCompleted.ToEventId(), "Maritime safety information request to get weekly NM files result completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return Json(listFiles);
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestStarted.ToEventId(), "Maritime safety information request to show weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            JsonResult result = await GetWeeklyFilesResultAsync(year,week);
            string json = JsonConvert.SerializeObject(result.Value);

            List<ShowFilesResponseModel> listFiles = JsonConvert.DeserializeObject<List<ShowFilesResponseModel>>(json);

            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestCompleted.ToEventId(), "Maritime safety information request to show weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml", listFiles);
        }

        public async Task<JsonResult> GetDailyFilesResultAsync()
        {
            _logger.LogInformation(EventIds.MSIGetDailyFilesResultRequest.ToEventId(), "Maritime safety information request for get daily files result:{correlationId}", GetCurrentCorrelationId());

            List<ShowDailyFilesResponseModel> Entries = await _nMDataService.GetDailyBatchDetailsFiles(GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.MSIGetDailyFilesResultCompleted.ToEventId(), "Maritime safety information request for get daily files result completed:{correlationId}", GetCurrentCorrelationId());

            return Json(Entries);
        }

        public async Task<IActionResult> ShowDailyFilesAsync()
        {
            _logger.LogInformation(EventIds.MSIShowDailyFilesRequest.ToEventId(), "Maritime safety information request for show daily files requested:{correlationId}", GetCurrentCorrelationId());

            JsonResult result = await GetDailyFilesResultAsync();
            string json = JsonConvert.SerializeObject(result.Value);
            List<ShowDailyFilesResponseModel> Entries = JsonConvert.DeserializeObject<List<ShowDailyFilesResponseModel>>(json);
            

            _logger.LogInformation(EventIds.MSIShowDailyFilesCompleted.ToEventId(), "Maritime safety information request for show daily files completed:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/ShowDailyFilesList.cshtml", Entries);

        }
    }
}
