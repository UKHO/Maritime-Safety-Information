using Microsoft.AspNetCore.Mvc;
using UKHO.FileShareClient.Models;
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
        private readonly IFileShareService _fileShareService;
        private readonly IAuthFssTokenProvider _authFssTokenProvider;

        public NoticesToMarinersController(INMDataService nMDataService, IHttpContextAccessor contextAccessor, ILogger<NoticesToMarinersController> logger, IFileShareService fileShareService, IAuthFssTokenProvider authFssTokenProvider) : base(contextAccessor, logger)
        {
            _logger = logger;
            _nMDataService = nMDataService;
            _fileShareService = fileShareService;
            _authFssTokenProvider = authFssTokenProvider;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());
            return View("~/Views/NoticesToMariners/FilterWeeklyFiles.cshtml");
        }

        public IActionResult LoadYears()
        {
            return Json(  _nMDataService.GetAllYearsandWeek(GetCurrentCorrelationId()));
        }

        public IActionResult LoadWeeks(int year)
        {
            return Json(_nMDataService.GetAllWeeksofYear(year, GetCurrentCorrelationId()));
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestStarted.ToEventId(), "Maritime safety information request to show weekly NM files started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await _nMDataService.GetWeeklyBatchFiles(year, week, GetCurrentCorrelationId());

            _logger.LogInformation(EventIds.NoticesToMarinersWeeklyFilesRequestCompleted.ToEventId(), "Maritime safety information request to show weekly NM files completed for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml", listFiles);
        }

        public async Task<IActionResult> YearWeek()
        {
            IResult<BatchAttributesSearchResponse> result = await GetSearchAttributeData();
            return Json(result);                     

        }

        public async Task<IResult<BatchAttributesSearchResponse>> GetSearchAttributeData()
        {
            string accessToken = await _authFssTokenProvider.GenerateADAccessToken(GetCurrentCorrelationId());

            IResult <BatchAttributesSearchResponse> searchAttributes = await _fileShareService.FssSearchAttributeAsync(accessToken, "");
            return searchAttributes;
        }
    }
}
