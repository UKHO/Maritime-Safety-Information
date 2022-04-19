using System.Globalization;
using Microsoft.AspNetCore.Mvc;
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
            string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUwMzY4NTg2LCJuYmYiOjE2NTAzNjg1ODYsImV4cCI6MTY1MDM3MzQxMCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQXVmOGdiOW9XL1hlUDFuQWZvLzM3d3ZNNDN0QW8yS3pYYlNWSENEVytZZ1pIeTArSVdwbE0wSDZXM005bzlLUUhXTERXMm9GdE5wMHAzblhPRHVSNjNCc2V5cDFwOFN2L3FBMnRpYUo5ZXpNPSIsImFtciI6WyJwd2QiLCJyc2EiXSwiYXBwaWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6Im1vaGFtbWUxNTMxNUBtYXN0ZWsuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYWRkMWM1MDAtYTZkNy00ZGJkLWI4OTAtN2Y4Y2I2ZjdkODYxLyIsImlwYWRkciI6IjIyMy4xODkuNTEuMjEiLCJuYW1lIjoiTW9oYW1tZWQgS2hhbiIsIm9pZCI6IjAxMzA1NWNiLWVkNjItNDZkMi1hZGU4LTM4ZmNjZDY0MGFhNiIsInJoIjoiMC5BVk1BU01vMGtUMW1CVXFXaWpHa0x3cnRQaVRnVzRBSW92dEFxMjg1bkNaSDB6UUNBQTQuIiwicm9sZXMiOlsiQmF0Y2hDcmVhdGUiXSwic2NwIjoiVXNlci5SZWFkIiwic3ViIjoiZ2RwR3VBN3VTZk9HY0RuS2VmZk41MXZBUDZXa2hKNldxbHdOaVpSMk9qNCIsInRpZCI6IjkxMzRjYTQ4LTY2M2QtNGEwNS05NjhhLTMxYTQyZjBhZWQzZSIsInVuaXF1ZV9uYW1lIjoibW9oYW1tZTE1MzE1QG1hc3Rlay5jb20iLCJ1dGkiOiJ0NkM1UVpJMVhFbW1seXlORU9NVEFBIiwidmVyIjoiMS4wIn0.e6Cl0Nk0pFogQtqIRygDrCpzYFWeRfF0aSOwWAxVGQUClfV5mYN34jGBR3fUt4flbVH3mYe4rovJ5e6Bjrxu5tpMRNH1vOS8CmCPlAp8NHn8MYncAcDyCSoGl-kqHyRYlZJPpw8TryCACKUKd7-46ZnILLdMiLaZdNyI9Q3ISfM8KEL5f_Sn56m_qktDh8kAtxOXZ_fuEhIpYf3B07RUDUE1wCzMIwc4FItZ20uN_3UptbwCNFkJPdWGeJGUc5sOyu4oeJOvp2XT5ndYBYfLgOfxajF_K1z0_0w7NwZeZpQ6E2a33mzll26QsBukOWIigJZbwUTTmMWexJWxr1Nwdg";
            List<ShowFilesResponseModel> listFiles = await nMDataService.GetBatchDetailsFiles(2022, 15, accessToken);
            return View(listFiles);
        }

        [HttpGet]
        [Route("/batch/{batchId}/files/{filename}")]
        public async Task<IActionResult> DownloadFssFilesAsync(string batchId, string filename)
        {
            string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUwMzY4NTg2LCJuYmYiOjE2NTAzNjg1ODYsImV4cCI6MTY1MDM3MzQxMCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQXVmOGdiOW9XL1hlUDFuQWZvLzM3d3ZNNDN0QW8yS3pYYlNWSENEVytZZ1pIeTArSVdwbE0wSDZXM005bzlLUUhXTERXMm9GdE5wMHAzblhPRHVSNjNCc2V5cDFwOFN2L3FBMnRpYUo5ZXpNPSIsImFtciI6WyJwd2QiLCJyc2EiXSwiYXBwaWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6Im1vaGFtbWUxNTMxNUBtYXN0ZWsuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYWRkMWM1MDAtYTZkNy00ZGJkLWI4OTAtN2Y4Y2I2ZjdkODYxLyIsImlwYWRkciI6IjIyMy4xODkuNTEuMjEiLCJuYW1lIjoiTW9oYW1tZWQgS2hhbiIsIm9pZCI6IjAxMzA1NWNiLWVkNjItNDZkMi1hZGU4LTM4ZmNjZDY0MGFhNiIsInJoIjoiMC5BVk1BU01vMGtUMW1CVXFXaWpHa0x3cnRQaVRnVzRBSW92dEFxMjg1bkNaSDB6UUNBQTQuIiwicm9sZXMiOlsiQmF0Y2hDcmVhdGUiXSwic2NwIjoiVXNlci5SZWFkIiwic3ViIjoiZ2RwR3VBN3VTZk9HY0RuS2VmZk41MXZBUDZXa2hKNldxbHdOaVpSMk9qNCIsInRpZCI6IjkxMzRjYTQ4LTY2M2QtNGEwNS05NjhhLTMxYTQyZjBhZWQzZSIsInVuaXF1ZV9uYW1lIjoibW9oYW1tZTE1MzE1QG1hc3Rlay5jb20iLCJ1dGkiOiJ0NkM1UVpJMVhFbW1seXlORU9NVEFBIiwidmVyIjoiMS4wIn0.e6Cl0Nk0pFogQtqIRygDrCpzYFWeRfF0aSOwWAxVGQUClfV5mYN34jGBR3fUt4flbVH3mYe4rovJ5e6Bjrxu5tpMRNH1vOS8CmCPlAp8NHn8MYncAcDyCSoGl-kqHyRYlZJPpw8TryCACKUKd7-46ZnILLdMiLaZdNyI9Q3ISfM8KEL5f_Sn56m_qktDh8kAtxOXZ_fuEhIpYf3B07RUDUE1wCzMIwc4FItZ20uN_3UptbwCNFkJPdWGeJGUc5sOyu4oeJOvp2XT5ndYBYfLgOfxajF_K1z0_0w7NwZeZpQ6E2a33mzll26QsBukOWIigJZbwUTTmMWexJWxr1Nwdg";
            byte[] fileBytes = await nMDataService.DownloadFssFileAsync(batchId, filename, accessToken);
            return File(fileBytes, "application/octet-stream", filename);
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
