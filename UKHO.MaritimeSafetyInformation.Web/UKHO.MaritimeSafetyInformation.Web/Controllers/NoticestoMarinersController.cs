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

        public async Task<FileResult> DownloadWeeklyFile(string batchId, string fileName, string mimeType)
        {
            if (string.IsNullOrEmpty(batchId)) { 
                _logger.LogInformation(EventIds.DownloadSingleWeeklyNMFileInvalidParameter.ToEventId(), "Maritime safety information download single weekly NM files called with invalid argument batchId:{batchId} for _X-Correlation-ID:{correlationId}", batchId, GetCurrentCorrelationId());
                throw new ArgumentNullException("Invalid value recieved for parameter batchId", new Exception());
            }
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogInformation(EventIds.DownloadSingleWeeklyNMFileInvalidParameter.ToEventId(), "Maritime safety information download single weekly NM files called with invalid argument FileName:{fileName} for _X-Correlation-ID:{correlationId}", fileName, GetCurrentCorrelationId());
                throw new ArgumentNullException("Invalid value recieved for parameter fileName", new Exception());
            }
            if (string.IsNullOrEmpty(mimeType))
            {
                _logger.LogInformation(EventIds.DownloadSingleWeeklyNMFileInvalidParameter.ToEventId(), "Maritime safety information download single weekly NM files called with invalid argument MIME Type:{mimeType} for _X-Correlation-ID:{correlationId}", mimeType, GetCurrentCorrelationId());
                throw new ArgumentNullException("Invalid value recieved for parameter mimeType", new Exception());
            }
            byte[] fileBytes;
            try
            {
                _logger.LogInformation(EventIds.DownloadSingleWeeklyNMFileStarted.ToEventId(), "Maritime safety information request to download single weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                fileBytes = await _nMDataService.DownloadFssFileAsync(batchId, fileName, GetCurrentCorrelationId());

                _logger.LogInformation(EventIds.DownloadSingleWeeklyNMFileCompleted.ToEventId(), "Maritime safety information request to download single weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.DownloadSingleWeeklyNMFileFailed.ToEventId(), "Maritime safety information request to download single weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
                throw;
            }
            
            return File(fileBytes, mimeType);
        }
    }
}
