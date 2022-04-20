using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Azure.Identity;
using System.Globalization;
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
        private readonly IFileShareService fileShareService;
        private readonly INMDataService nMDataService;
        public NoticestoMarinersController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IFileShareService fileShareService, INMDataService nMDataService)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.fileShareService = fileShareService;
            this.nMDataService = nMDataService;
        }

        public IActionResult Index()
        {
            return View("~/Views/NoticestoMariners/ShowWeeklyFiles.cshtml");
        }

        public IActionResult LoadYears()
        {

            return Json(GetPastYears());
        }

        public IActionResult LoadWeeks(int year)
        {

            return Json(GetAllWeeksofYear(year));
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync(int year, int week)
        {
            List<ShowFilesResponseModel> listFiles = await nMDataService.GetBatchDetailsFiles(year, week);
            return PartialView("~/Views/NoticestoMariners/_WeeklyFilesList.cshtml",listFiles);
        }

        private List<KeyValuePair<string, string>> GetPastYears()
        {
            List<KeyValuePair<string, string>> years = new List<KeyValuePair<string, string>>();
            years.Add(new KeyValuePair<string, string>("Year", ""));
            for (int i = 0; i < 3; i++)
            {
                string year = (DateTime.Now.Year - i).ToString();
                years.Add(new KeyValuePair<string, string>(year, year));
            }
            return years;
        }
        private List<KeyValuePair<string, string>> GetAllWeeksofYear(int year)
        {
            List<KeyValuePair<string, string>> weeks = new List<KeyValuePair<string, string>>();
            weeks.Add(new KeyValuePair<string, string>("Week Number", ""));

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime lastdate;
            if (DateTime.Now.Year == year)
            {
                lastdate = new DateTime(year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            {
                lastdate = new DateTime(year, 12, 31);
            }
            Calendar cal = dfi.Calendar;

            int totalWeeks = cal.GetWeekOfYear(lastdate, dfi.CalendarWeekRule,
                                                dfi.FirstDayOfWeek);

            for (int i = 0; i < totalWeeks; i++)
            {
                string week = (totalWeeks - i).ToString();
                weeks.Add(new KeyValuePair<string, string>(week, week));
            }
            return weeks;
        }
    }
}
