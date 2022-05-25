using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class NoticesToMarinersController : BaseController<NoticesToMarinersController>
    {

        private readonly ILogger<NoticesToMarinersController> _logger;
        private readonly INMDataService _nMDataService;
        private readonly IHttpContextAccessor _contextAccessor;
        public NoticesToMarinersController(INMDataService nMDataService, IHttpContextAccessor contextAccessor, ILogger<NoticesToMarinersController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
            _nMDataService = nMDataService;
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            ShowWeeklyFilesResponseModel showWeeklyFiles = new();
            try
            {
                _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request to get weekly NM files started for correlationId:{correlationId}", GetCurrentCorrelationId());

                showWeeklyFiles = await _nMDataService.GetWeeklyFilesResponseModelsAsync(0, 0, GetCurrentCorrelationId());
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowWeeklyFilesIndexGetFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }

            _logger.LogInformation(EventIds.ShowWeeklyFilesResponseIndexGetCompleted.ToEventId(), "Maritime safety information request for weekly NM file response for index get completed for correlationId:{correlationId}", GetCurrentCorrelationId());

            return View("~/Views/NoticesToMariners/Index.cshtml", showWeeklyFiles);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int year, int week)
        {
            ShowWeeklyFilesResponseModel showWeeklyFiles = new();
            try
            {
                _logger.LogInformation(EventIds.ShowWeeklyFilesResponseStartIndexPost.ToEventId(), "Maritime safety information request for weekly NM file response for index post started for correlationId:{correlationId}", GetCurrentCorrelationId());

                showWeeklyFiles = await _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, GetCurrentCorrelationId());

                ViewData["Year"] = year;
                ViewData["Week"] = week;

            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowWeeklyFilesIndexPostFailed.ToEventId(), "Maritime safety information request to get daily NM weekly files index post failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }
            _logger.LogInformation(EventIds.ShowWeeklyFilesResponseIndexPostCompleted.ToEventId(), "Maritime safety information request for weekly NM file response for index post completed for correlationId:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/Index.cshtml", showWeeklyFiles);
        }

        public IActionResult DailyFiles()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request to get daily NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/ShowDailyFiles.cshtml");
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

            return View("~/Views/NoticesToMariners/ShowDailyFiles.cshtml", showDailyFilesResponseModels);

        }

        public async Task<FileResult> DownloadWeeklyFile(string batchId, string fileName, string mimeType)
        {
            NMHelper.ValidateParametersForDownloadSingleFile(new()
            {
                new KeyValuePair<string, string>("BatchId", batchId),
                new KeyValuePair<string, string>("FileName", fileName),
                new KeyValuePair<string, string>("MimeType", mimeType)
            }, GetCurrentCorrelationId(), _logger);

            try
            {
                _logger.LogInformation(EventIds.DownloadSingleWeeklyNMFileStarted.ToEventId(), "Maritime safety information request to download single weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                byte[] fileBytes = await _nMDataService.DownloadFssFileAsync(batchId, fileName, GetCurrentCorrelationId());

                _logger.LogInformation(EventIds.DownloadSingleWeeklyNMFileCompleted.ToEventId(), "Maritime safety information request to download single weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                _contextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename=" + fileName);

                return File(fileBytes, mimeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.DownloadSingleWeeklyNMFileFailed.ToEventId(), "Maritime safety information request to download single weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
                throw;
            }
        }

        public async Task<ActionResult> DownloadDailyFile(string batchId, string mimeType)
        {
            try
            {
                _logger.LogInformation(EventIds.DownloadDailyNMFileStarted.ToEventId(), "Maritime safety information request to download daily NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                string fileName = batchId + ".zip";

                NMHelper.ValidateParametersForDownloadSingleFile(new()
                {
                    new KeyValuePair<string, string>("BatchId", batchId),
                    new KeyValuePair<string, string>("MimeType", mimeType)
                }, GetCurrentCorrelationId(), _logger);

                byte[] fileBytes = await _nMDataService.DownloadFssFileAsync(batchId, String.Empty, GetCurrentCorrelationId());

                _contextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename=" + fileName);

                _logger.LogInformation(EventIds.DownloadDailyNMFileCompleted.ToEventId(), "Maritime safety information request to download daily NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                return File(fileBytes, mimeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.DownloadDailyNMFileFailed.ToEventId(), "Maritime safety information request to download daily NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
                return RedirectToAction("ShowDailyFiles");
            }
        }

    }
}
