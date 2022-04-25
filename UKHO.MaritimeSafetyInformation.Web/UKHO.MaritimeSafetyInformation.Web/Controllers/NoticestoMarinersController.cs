using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class NoticestoMarinersController : BaseController<NoticestoMarinersController>
    {

        private readonly ILogger<NoticestoMarinersController> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly INMDataService nMDataService;
        public NoticestoMarinersController(IHttpClientFactory httpClientFactory, INMDataService nMDataService, IHttpContextAccessor contextAccessor, ILogger<NoticestoMarinersController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.nMDataService = nMDataService;
        }

        public IActionResult Index()
        {
            _logger.LogInformation(EventIds.Start.ToEventId(), "Maritime safety information request started for correlationId:{correlationId}", GetCurrentCorrelationId());

            return View("~/Views/NoticestoMariners/ShowWeeklyFiles.cshtml");
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
            _logger.LogInformation(EventIds.RetrievalOfMSIShowWeeklyFilesRequest.ToEventId(), "Maritime safety information request for show weekly files requested:{correlationId}", GetCurrentCorrelationId());

            List<ShowFilesResponseModel> listFiles = await nMDataService.GetBatchDetailsFiles(year, week);

            _logger.LogInformation(EventIds.RetrievalOfMSIShowWeeklyFilesCompleted.ToEventId(), "Maritime safety information request for show weekly files completed:{correlationId}", GetCurrentCorrelationId());

            return PartialView("~/Views/NoticestoMariners/_WeeklyFilesList.cshtml",listFiles);
        }

        
    }
}
