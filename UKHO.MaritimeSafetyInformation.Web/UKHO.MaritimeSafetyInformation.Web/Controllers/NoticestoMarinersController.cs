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
        private readonly IUserService _userService;

        public NoticesToMarinersController(INMDataService nMDataService, IHttpContextAccessor contextAccessor, ILogger<NoticesToMarinersController> logger, IUserService userService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _nMDataService = nMDataService;
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        [HttpGet]
        [Route("/NoticesToMariners/Weekly")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.IsDistributor = _userService.IsDistributorUser;
                ViewBag.HasError = false;

                _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request to get weekly NM files started for User:{SignInName} and IsDistributor:{IsDistributorUser} and correlationId:{correlationId}", _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());

                ShowWeeklyFilesResponseModel showWeeklyFiles = await _nMDataService.GetWeeklyFilesResponseModelsAsync(0, 0, GetCurrentCorrelationId());

                _logger.LogInformation(EventIds.ShowWeeklyFilesResponseIndexGetCompleted.ToEventId(), "Maritime safety information request for weekly NM file response for index get completed for User:{SignInName} and IsDistributor:{IsDistributorUser} and correlationId:{correlationId}", _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());

                if (showWeeklyFiles.ShowFilesResponseList.Count == 0)
                {
                    ViewBag.HasError = true;
                    ViewData["CurrentCorrelationId"] = GetCurrentCorrelationId();
                }

                ViewData["Year"] = showWeeklyFiles.YearAndWeekList.OrderByDescending(x => x.Year).Select(x => x.Year).FirstOrDefault();
                ViewData["Week"] = showWeeklyFiles.YearAndWeekList.OrderByDescending(x => x.Week).Where(x => x.Year == Convert.ToInt32(ViewData["Year"])).Select(x => x.Week).FirstOrDefault();

                return View("~/Views/NoticesToMariners/Index.cshtml", showWeeklyFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowWeeklyFilesIndexGetFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for User:{SignInName} and IsDistributor:{IsDistributorUser} with _X-Correlation-ID:{CorrelationId}", ex.Message, _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());
                throw;
            }
        }

        [HttpPost]
        [Route("/NoticesToMariners/Weekly")]
        public async Task<IActionResult> Index(int year, int week)
        {
            try
            {
                _logger.LogInformation(EventIds.ShowWeeklyFilesResponseStartIndexPost.ToEventId(), "Maritime safety information request for weekly NM file response for index post started for User:{SignInName} and IsDistributor:{IsDistributorUser} with correlationId:{correlationId}", _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());
                ViewBag.IsDistributor = _userService.IsDistributorUser;
                ViewBag.HasError = false;
                ShowWeeklyFilesResponseModel showWeeklyFiles = await _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, GetCurrentCorrelationId());

                ViewData["Year"] = year;
                ViewData["Week"] = week;

                _logger.LogInformation(EventIds.ShowWeeklyFilesResponseIndexPostCompleted.ToEventId(), "Maritime safety information request for weekly NM file response for index post completed for User:{SignInName} and IsDistributor:{IsDistributorUser} with correlationId:{correlationId}", _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());

                if (showWeeklyFiles.ShowFilesResponseList.Count == 0)
                {
                    ViewBag.HasError = true;
                    ViewData["CurrentCorrelationId"] = GetCurrentCorrelationId();
                }

                return View("~/Views/NoticesToMariners/Index.cshtml", showWeeklyFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowWeeklyFilesIndexPostFailed.ToEventId(), "Maritime safety information request to get daily NM weekly files index post failed to return data with exception:{exceptionMessage} for User:{SignInName} and IsDistributor:{IsDistributorUser} with _X-Correlation-ID:{CorrelationId}", ex.Message, _userService.SignInName ?? "Public", _userService.IsDistributorUser, GetCurrentCorrelationId());
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            try
            {
                _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestStarted.ToEventId(), "Maritime safety information request to show weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                ShowNMFilesResponseModel showNMFilesResponseModel = await _nMDataService.GetWeeklyBatchFiles(year, week, GetCurrentCorrelationId());
                List<ShowFilesResponseModel> listFiles = showNMFilesResponseModel.ShowFilesResponseModel;

                _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestCompleted.ToEventId(), "Maritime safety information request to show weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                return PartialView("~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml", listFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.NoticesToMarinersWeeklyFilesRequestFailed.ToEventId(), "Maritime safety information request to show weekly NM files failed with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
                throw;
            }
        }

        [HttpGet]
        [Route("/NoticesToMariners/Daily")]
        public async Task<IActionResult> ShowDailyFilesAsync()
        {
            ShowDailyFilesResponseListModel showDailyFilesResponseModels = new();

            try
            {
                _logger.LogInformation(EventIds.ShowDailyFilesRequest.ToEventId(), "Maritime safety information request to show daily NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

                ViewBag.HasError = false;

                showDailyFilesResponseModels = await _nMDataService.GetDailyBatchDetailsFiles(GetCurrentCorrelationId());
                ViewBag.IsDailyFilesResponseCached = showDailyFilesResponseModels.IsDailyFilesResponseCached;

                _logger.LogInformation(EventIds.ShowDailyFilesCompleted.ToEventId(), "Maritime safety information request to show daily NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
            }
            catch (Exception ex)
            {
                ViewBag.HasError = true;
                ViewData["CurrentCorrelationId"] = GetCurrentCorrelationId();

                _logger.LogError(EventIds.ShowDailyFilesFailed.ToEventId(), "Maritime safety information request to show daily NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }

            return View("~/Views/NoticesToMariners/ShowDailyFiles.cshtml", showDailyFilesResponseModels.ShowDailyFilesResponseModel);
        }

        [HttpGet]
        public async Task<FileResult> DownloadFile(string batchId, string fileName, string mimeType, string frequency)
        {
            try
            {
                _logger.LogInformation(EventIds.DownloadSingleNMFileStarted.ToEventId(), "Maritime safety information request to download single {frequency} NM files started for _X-Correlation-ID:{correlationId}", frequency, GetCurrentCorrelationId());

                NMHelper.ValidateParametersForDownloadSingleFile(new()
                {
                    new KeyValuePair<string, string>("BatchId", batchId),
                    new KeyValuePair<string, string>("FileName", fileName),
                    new KeyValuePair<string, string>("MimeType", mimeType)
                }, GetCurrentCorrelationId(), _logger);

                byte[] fileBytes = await _nMDataService.DownloadFssFileAsync(batchId, fileName, GetCurrentCorrelationId(), frequency);

                _logger.LogInformation(EventIds.DownloadSingleNMFileCompleted.ToEventId(), "Maritime safety information request to download single {frequency} NM files completed for _X-Correlation-ID:{correlationId}", frequency, GetCurrentCorrelationId());

                _contextAccessor.HttpContext.Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");

                if (mimeType != "application/pdf")
                    mimeType = "application/octet-stream";

                return File(fileBytes, mimeType);

            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.DownloadSingleNMFileFailed.ToEventId(), "Maritime safety information request to download single {frequency} NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, frequency, GetCurrentCorrelationId());
                throw;
            }
        }

        [HttpGet]
        [Route("/NoticesToMariners/Cumulative")]
        public async Task<IActionResult> Cumulative()
        {
            ShowNMFilesResponseModel showNMFilesResponseModel = new();
            try
            {
                _logger.LogInformation(EventIds.ShowCumulativeFilesRequestStarted.ToEventId(), "Maritime safety information request to show cumulative NM files started for correlationId:{correlationId}", GetCurrentCorrelationId());

                ViewBag.HasError = false;

                showNMFilesResponseModel = await _nMDataService.GetCumulativeBatchFiles(GetCurrentCorrelationId());

                _logger.LogInformation(EventIds.ShowCumulativeFilesRequestCompleted.ToEventId(), "Maritime safety information request for cumulative NM files completed for correlationId:{correlationId}", GetCurrentCorrelationId());
            }
            catch (Exception ex)
            {
                ViewBag.HasError = true;
                ViewData["CurrentCorrelationId"] = GetCurrentCorrelationId();

                _logger.LogError(EventIds.ShowCumulativeFilesFailed.ToEventId(), "Maritime safety information request to show cumulative NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }

            return View("~/Views/NoticesToMariners/Cumulative.cshtml", showNMFilesResponseModel);
        }

        [HttpGet]
        [Route("/NoticesToMariners/Annual")]
        public async Task<IActionResult> Annual()
        {
            ShowNMFilesResponseModel showFilesResponse = new();
            try
            {
                _logger.LogInformation(EventIds.ShowAnnualFilesRequestStarted.ToEventId(), "Maritime safety information request to show annual NM files started for correlationId:{correlationId}", GetCurrentCorrelationId());

                ViewBag.HasError = false;

                showFilesResponse = await _nMDataService.GetAnnualBatchFiles(GetCurrentCorrelationId());

                _logger.LogInformation(EventIds.ShowAnnualFilesRequestCompleted.ToEventId(), "Maritime safety information request for annual NM files completed for correlationId:{correlationId}", GetCurrentCorrelationId());
            }
            catch (Exception ex)
            {
                ViewBag.HasError = true;
                ViewData["CurrentCorrelationId"] = GetCurrentCorrelationId();

                _logger.LogError(EventIds.ShowAnnualFilesFailed.ToEventId(), "Maritime safety information request to show annual NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, GetCurrentCorrelationId());
            }

            return View("~/Views/NoticesToMariners/Annual.cshtml", showFilesResponse);
        }

        [HttpGet]
        [Route("/NoticesToMariners/Resellers")]
        public IActionResult Resellers()
        {
            return View();
        }

        [HttpGet]
        [Route("/NoticesToMariners/About")]
        public IActionResult About()
        {
            return View("~/Views/NoticesToMariners/About.cshtml");
        }

        [HttpGet]
        public async Task<FileResult> DownloadDailyFile(string batchId, string fileName, string mimeType)
        {
            try
            {
                _logger.LogInformation(EventIds.DownloadDailyNMFileStarted.ToEventId(), "Maritime safety information request to download daily NM files started with batchId:{batchId} and fileName:{fileName} for _X-Correlation-ID:{correlationId}", batchId, fileName, GetCurrentCorrelationId());

                NMHelper.ValidateParametersForDownloadSingleFile(new()
                {
                    new KeyValuePair<string, string>("BatchId", batchId),
                    new KeyValuePair<string, string>("FileName", fileName),
                    new KeyValuePair<string, string>("MimeType", mimeType)
                }, GetCurrentCorrelationId(), _logger);

                byte[] fileBytes = await _nMDataService.DownloadFSSZipFileAsync(batchId, fileName, GetCurrentCorrelationId());

                _contextAccessor.HttpContext.Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");

                _logger.LogInformation(EventIds.DownloadDailyNMFileCompleted.ToEventId(), "Maritime safety information request to download daily NM files with batchId:{batchId} and fileName:{fileName} is completed for _X-Correlation-ID:{correlationId}", batchId, fileName, GetCurrentCorrelationId());

                return File(fileBytes, mimeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.DownloadDailyNMFileFailed.ToEventId(), "Maritime safety information request to download daily NM files with batchId:{batchId} and fileName:{fileName} has failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", batchId, fileName, ex.Message, GetCurrentCorrelationId());
                throw;
            }
        }

        public async Task<FileResult> DownloadAllWeeklyZipFile(string batchId, string fileName, string mimeType, string type)
        {
            try
            {
                NMHelper.ValidateParametersForDownloadSingleFile(new()
                {
                    new KeyValuePair<string, string>("BatchId", batchId),
                    new KeyValuePair<string, string>("FileName", fileName),
                    new KeyValuePair<string, string>("MimeType", mimeType),
                    new KeyValuePair<string, string>("Type", type)
                }, GetCurrentCorrelationId(), _logger);

                _logger.LogInformation(EventIds.DownloadAllWeeklyNMFileStarted.ToEventId(), "Maritime safety information request to download all weekly NM files started for {type} with batchId:{batchId} and fileName:{fileName} for _X-Correlation-ID:{correlationId}", type, batchId, fileName, GetCurrentCorrelationId());

                byte[] fileBytes = await _nMDataService.DownloadFSSZipFileAsync(batchId, fileName, GetCurrentCorrelationId());

                _contextAccessor.HttpContext.Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");

                _logger.LogInformation(EventIds.DownloadAllWeeklyNMFileCompleted.ToEventId(), "Maritime safety information request to download all weekly NM files for {type} with batchId:{batchId} and fileName:{fileName} is completed for _X-Correlation-ID:{correlationId}", type, batchId, fileName, GetCurrentCorrelationId());

                return File(fileBytes, mimeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.DownloadAllWeeklyNMFileFailed.ToEventId(), "Maritime safety information request to download all weekly NM files for {type} with batchId:{batchId} and fileName:{fileName} has failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", type, batchId, fileName, ex.Message, GetCurrentCorrelationId());
                throw;
            }
        }

    }
}
