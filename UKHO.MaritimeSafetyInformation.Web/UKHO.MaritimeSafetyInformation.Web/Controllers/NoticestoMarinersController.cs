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

        public async Task<IActionResult> IndexAsync()
        {
            ShowWeeklyFilesResponseModel showWeeklyFiles = new ShowWeeklyFilesResponseModel();
            try
            {
                _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());

                showWeeklyFiles = await _nMDataService.GetWeeklyFilesResponseModelsAsync(0, 0, GetCurrentCorrelationId());

                if (showWeeklyFiles.Years != null && showWeeklyFiles.Weeks != null)
                {
                    _logger.LogInformation(EventIds.ShowWeeklyFilesResponseForYearAndWeekNotNullForIndexGet.ToEventId(), "Maritime safety information request to get show weekly NM files response for year and week is not null with _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                    
                    TempData["Year"] = Convert.ToInt32(showWeeklyFiles.Years.Where(x => x.Selected).OrderByDescending(x => x.Value).Select(x => x.Value).FirstOrDefault());
                    TempData["Week"] = Convert.ToInt32(showWeeklyFiles.Weeks.Where(x => x.Selected).OrderByDescending(x => x.Value).Select(x => x.Value).FirstOrDefault());

                    TempData.Keep("Week");
                    TempData.Keep("Year");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowWeeklyFilesIndexGetFailed.ToEventId(), "Maritime safety information request to get daily NM weekly files index get failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }
            _logger.LogInformation(EventIds.ShowWeeklyFilesResponseIndexGetCompleted.ToEventId(), "Maritime safety information request for weekly NM file response for index get completed for correlationId:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/Index.cshtml", showWeeklyFiles);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(int year, int week)
        {
            ShowWeeklyFilesResponseModel showWeeklyFiles = new ShowWeeklyFilesResponseModel();
            try
            {
                _logger.LogInformation(EventIds.ShowWeeklyFilesResponseStartIndexPost.ToEventId(), "Maritime safety information request for weekly NM file response for index post started for correlationId:{correlationId}", GetCurrentCorrelationId());

                if (year != 0)
                {
                    _logger.LogInformation(EventIds.ShowWeeklyFilesResponseForYearNonZero.ToEventId(), "Maritime safety information request for weekly NM file response for year non zero for correlationId:{correlationId}", GetCurrentCorrelationId());
                    showWeeklyFiles = await _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, GetCurrentCorrelationId());
                }

                if (year != 0 && week != 0)
                {
                    _logger.LogInformation(EventIds.ShowWeeklyFilesResponseForYearAndWeekNonZero.ToEventId(), "Maritime safety information request for weekly NM file response for year and week non zero for correlationId:{correlationId}", GetCurrentCorrelationId());
                    showWeeklyFiles = await _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, GetCurrentCorrelationId());

                    TempData["Year"] = year;
                    TempData["Week"] = week;

                    TempData.Keep("Week");
                    TempData.Keep("Year");
                }
                else if (year != 0 && week == 0)
                {
                    _logger.LogInformation(EventIds.ShowWeeklyFilesResponseForYearNonZeroAndWeekWithZero.ToEventId(), "Maritime safety information request for weekly NM file response for year with non zero and week with zero for correlationId:{correlationId}", GetCurrentCorrelationId());
                    if (TempData["Year"] != null && TempData["Week"] != null)
                    {
                        _logger.LogInformation(EventIds.ShowWeeklyFilesResponseForTempDataYearAndWeekNotNull.ToEventId(), "Maritime safety information request for weekly NM file response for tempdata for year and week is not null for correlationId:{correlationId}", GetCurrentCorrelationId());

                        TempData.Keep("Week");
                        TempData.Keep("Year");

                        showWeeklyFiles.ShowFilesResponseModel = (await _nMDataService.GetWeeklyFilesResponseModelsAsync(Convert.ToInt32(TempData["Year"]), Convert.ToInt32(TempData["Week"]), GetCurrentCorrelationId())).ShowFilesResponseModel;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowWeeklyFilesIndexPostFailed.ToEventId(), "Maritime safety information request to get daily NM weekly files index post failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }
            _logger.LogInformation(EventIds.ShowWeeklyFilesResponsetIndexPostCompleted.ToEventId(), "Maritime safety information request for weekly NM file response for index post completed for correlationId:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/Index.cshtml", showWeeklyFiles);
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

        [HttpPost]
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
