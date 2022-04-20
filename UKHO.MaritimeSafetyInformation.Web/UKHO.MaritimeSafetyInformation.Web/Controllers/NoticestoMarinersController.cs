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
            //var access = await GetNewAuthToken("805be024-a208-40fb-ab6f-399c2647d334");

            //string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUwNDM3MzgzLCJuYmYiOjE2NTA0MzczODMsImV4cCI6MTY1MDQ0MjI1MiwiYWNyIjoiMSIsImFpbyI6IkFYUUFpLzhUQUFBQVR4aE9PM2FRUk9ReDZlaE45TDRvaTdkaFRhTytocFFVdVZMZVdGTnE5S2FGc1lFU3RyR05la2xSWjVLbXNpSTlHYUxsa0R1c3IrQW80bSs3YUZ4bmhoanZwRDNQWkdUbEE1T2d2OHZjZy8rdWRyZEI2eWdTd2lGZ2F2dXViZlEvb3JEdjVrS0hYNlBTUi9jZlNmS2dFUT09IiwiYW1yIjpbInB3ZCIsInJzYSJdLCJhcHBpZCI6IjgwNWJlMDI0LWEyMDgtNDBmYi1hYjZmLTM5OWMyNjQ3ZDMzNCIsImFwcGlkYWNyIjoiMCIsImVtYWlsIjoibW9oYW1tZTE1MzE1QG1hc3Rlay5jb20iLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9hZGQxYzUwMC1hNmQ3LTRkYmQtYjg5MC03ZjhjYjZmN2Q4NjEvIiwiaXBhZGRyIjoiMjIzLjE4OS41MS4yMSIsIm5hbWUiOiJNb2hhbW1lZCBLaGFuIiwib2lkIjoiMDEzMDU1Y2ItZWQ2Mi00NmQyLWFkZTgtMzhmY2NkNjQwYWE2IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FBNC4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiJnZHBHdUE3dVNmT0djRG5LZWZmTjUxdkFQNldraEo2V3Fsd05pWlIyT2o0IiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJtb2hhbW1lMTUzMTVAbWFzdGVrLmNvbSIsInV0aSI6Im5ZdXE4RUM3ZWtPOHFCNzh1aW9tQUEiLCJ2ZXIiOiIxLjAifQ.gYjKc9vjyeFjZGO061YMJc0hTSIF2crM5uLxaHr8b34M5EOT_cXQjU8VFM8eRsewMLZvkPGbja1bTLgUroHy_nDQSNU9wN_XnuOzedqiZdftYXApUR6bALgEyvxbRNruuad05SruwgqjKXFYqrwvH9ug1g6iEefWNoFNGl3Bx2DPxtbZzceX7ACsoM-UFBIULdJEnP2FYe9FWWjsJPLriBfXxfqJVaDDdm03PKh_DILSCzMoFdBTkAKuaclmLRejXNA0sORtr7WcvIh8he7iFr4Ri0G1rMjWV5cMQQ428CpCPPb0cDbR9FjmUX3i2izFt9IUPMa6ejxZ6NO7AjuilA";
            AuthFssTokenProvider authFssTokenProvider = new AuthFssTokenProvider();
            AuthenticationResult authentication = await authFssTokenProvider.GetAuthTokenAsync();
            string accessToken = authentication.AccessToken;
            List<ShowFilesResponseModel> listFiles = await nMDataService.GetBatchDetailsFiles(year, week, accessToken);
            return PartialView("~/Views/NoticestoMariners/_WeeklyFilesList.cshtml",listFiles);
        }

        private async Task<AccessTokenItem> GetNewAuthToken(string resource)
        {
            var tokenCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = "9134ca48-663d-4a05-968a-31a42f0aed3e" });
            var accessToken = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new string[] { resource + "/.default" }) { }
            );
            return new AccessTokenItem
            {
                ExpiresIn = DateTime.Now.AddHours(1)/*accessToken.ExpiresOn.UtcDateTime*/,
                AccessToken = !String.IsNullOrEmpty(resource) ? "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjQ5MjM4OTIwLCJuYmYiOjE2NDkyMzg5MjAsImV4cCI6MTY0OTI0NDU2OSwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQW5YVFVTMWtDL3l4ZTJ5R1Nlc1JBVkk5NkJkNXFnTThYNDNkWlorQ1l4eGFGWFFySWpVSkVGVkJsVVMrZDJaUkJOQ3JrNXpMaEVXOW5XK2s4aElHSE9YckU1V1FsNnR1YlhDSURiUTZTZkpRPSIsImFtciI6WyJwd2QiLCJyc2EiXSwiYXBwaWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6Im1vaGFtbWUxNTMxNUBtYXN0ZWsuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYWRkMWM1MDAtYTZkNy00ZGJkLWI4OTAtN2Y4Y2I2ZjdkODYxLyIsImlwYWRkciI6IjIyMy4xODQuMjU0LjE1NCIsIm5hbWUiOiJNb2hhbW1lZCBLaGFuIiwib2lkIjoiMDEzMDU1Y2ItZWQ2Mi00NmQyLWFkZTgtMzhmY2NkNjQwYWE2IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FBNC4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiJnZHBHdUE3dVNmT0djRG5LZWZmTjUxdkFQNldraEo2V3Fsd05pWlIyT2o0IiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJtb2hhbW1lMTUzMTVAbWFzdGVrLmNvbSIsInV0aSI6IlcxeHZYbllfLVVpeS1POVZTTGU4QUEiLCJ2ZXIiOiIxLjAifQ.l0I0fST2hJoNKAZrNiCWINcCJf9E9odSTVPAegqF9ra2AHYS3Ba4WFHxP6KwT6KhreVc3nsRDQkASlmUOvqBxKhP0c5Xrl2l6w4I_6MmqqT81z1D3p9zbYKF7x4zUMfBlvzX6LW5czjTiocGC4iU42Mnil_H4ufVOPbXeu8dOfm05LZ2Rl8YKbyzRwg2V0l9XePXhWQpe9uFoKyDSfplmf2aeHETv1OwtY3sDVnjEXK5fuS5N9KsXM8eNfnq930IkszLAy11lj05yUXoQa7TTe8VZBN2mo9KTFGG6EYzDE4OFbGcRgQCORjT9ifr606p0Kc-fc2U9ayX4h0_Nvb9eg" : resource/*accessToken.Token*/
            };
        }

        [HttpGet]
        [Route("/batch/{batchId}/files/{filename}")]
        public async Task<IActionResult> DownloadFssFilesAsync(string batchId, string filename)
        {
            //string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUwNDM3MzgzLCJuYmYiOjE2NTA0MzczODMsImV4cCI6MTY1MDQ0MjI1MiwiYWNyIjoiMSIsImFpbyI6IkFYUUFpLzhUQUFBQVR4aE9PM2FRUk9ReDZlaE45TDRvaTdkaFRhTytocFFVdVZMZVdGTnE5S2FGc1lFU3RyR05la2xSWjVLbXNpSTlHYUxsa0R1c3IrQW80bSs3YUZ4bmhoanZwRDNQWkdUbEE1T2d2OHZjZy8rdWRyZEI2eWdTd2lGZ2F2dXViZlEvb3JEdjVrS0hYNlBTUi9jZlNmS2dFUT09IiwiYW1yIjpbInB3ZCIsInJzYSJdLCJhcHBpZCI6IjgwNWJlMDI0LWEyMDgtNDBmYi1hYjZmLTM5OWMyNjQ3ZDMzNCIsImFwcGlkYWNyIjoiMCIsImVtYWlsIjoibW9oYW1tZTE1MzE1QG1hc3Rlay5jb20iLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9hZGQxYzUwMC1hNmQ3LTRkYmQtYjg5MC03ZjhjYjZmN2Q4NjEvIiwiaXBhZGRyIjoiMjIzLjE4OS41MS4yMSIsIm5hbWUiOiJNb2hhbW1lZCBLaGFuIiwib2lkIjoiMDEzMDU1Y2ItZWQ2Mi00NmQyLWFkZTgtMzhmY2NkNjQwYWE2IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FBNC4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiJnZHBHdUE3dVNmT0djRG5LZWZmTjUxdkFQNldraEo2V3Fsd05pWlIyT2o0IiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJtb2hhbW1lMTUzMTVAbWFzdGVrLmNvbSIsInV0aSI6Im5ZdXE4RUM3ZWtPOHFCNzh1aW9tQUEiLCJ2ZXIiOiIxLjAifQ.gYjKc9vjyeFjZGO061YMJc0hTSIF2crM5uLxaHr8b34M5EOT_cXQjU8VFM8eRsewMLZvkPGbja1bTLgUroHy_nDQSNU9wN_XnuOzedqiZdftYXApUR6bALgEyvxbRNruuad05SruwgqjKXFYqrwvH9ug1g6iEefWNoFNGl3Bx2DPxtbZzceX7ACsoM-UFBIULdJEnP2FYe9FWWjsJPLriBfXxfqJVaDDdm03PKh_DILSCzMoFdBTkAKuaclmLRejXNA0sORtr7WcvIh8he7iFr4Ri0G1rMjWV5cMQQ428CpCPPb0cDbR9FjmUX3i2izFt9IUPMa6ejxZ6NO7AjuilA";
            AuthFssTokenProvider authFssTokenProvider = new AuthFssTokenProvider();
            AuthenticationResult authentication = await authFssTokenProvider.GetAuthTokenAsync();
            string accessToken = authentication.AccessToken;
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
