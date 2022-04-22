using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class NoticestoMarinersController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly INMDataService nMDataService;
        public NoticestoMarinersController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, INMDataService nMDataService)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.nMDataService = nMDataService;
        }

        public IActionResult Index()
        {
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
            List<ShowFilesResponseModel> listFiles = await nMDataService.GetBatchDetailsFiles(year, week);
            return PartialView("~/Views/NoticestoMariners/_WeeklyFilesList.cshtml",listFiles);
        }

        
    }
}
