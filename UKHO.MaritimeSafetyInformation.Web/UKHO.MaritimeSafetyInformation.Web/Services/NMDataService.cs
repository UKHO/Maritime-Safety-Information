using System.Globalization;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class NMDataService : INMDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFileShareService fileShareService;
        private readonly IConfiguration configuration;
        private readonly ILogger<NMDataService> _logger;
        private readonly FileShareServiceConfiguration fileShareServiceConfig;
        public NMDataService(IFileShareService fileShareService, IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<NMDataService> logger)
        {
            this.fileShareService = fileShareService;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.fileShareServiceConfig = configuration.GetSection("FileShareService").Get<FileShareServiceConfiguration>();
            _logger = logger;
        }
        public async Task<List<ShowFilesResponseModel>> GetBatchDetailsFiles(int year, int week)
        {
            AuthFssTokenProvider authFssTokenProvider = new AuthFssTokenProvider();
            AuthenticationResult authentication = await authFssTokenProvider.GetAuthTokenAsync();
            string accessToken = authentication.AccessToken;

            string searchText = $"BusinessUnit eq '{fileShareServiceConfig.BusinessUnit}' and $batch(Product Type) eq '{fileShareServiceConfig.ProductType}' and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";
            var result = await fileShareService.FssWeeklySearchAsync(searchText, accessToken);

            BatchSearchResponse SearchResult = result.Data;
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new List<ShowFilesResponseModel>();
            if (SearchResult.Entries.Count > 0)
            {
                ListshowFilesResponseModels = GetShowFilesResponses(SearchResult);
                ListshowFilesResponseModels = ListshowFilesResponseModels.OrderBy(e => e.FileDescription).ToList();
            }
            return ListshowFilesResponseModels;

        }

        private List<ShowFilesResponseModel> GetShowFilesResponses(BatchSearchResponse SearchResult)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new List<ShowFilesResponseModel>();
            foreach (var item in SearchResult.Entries)
            {
                foreach (var file in item.Files)
                {
                    ListshowFilesResponseModels.Add(new ShowFilesResponseModel
                    {
                        BatchId = item.BatchId,
                        Filename = file.Filename,
                        FileDescription = Path.GetFileNameWithoutExtension(file.Filename),
                        FileExtension = Path.GetExtension(file.Filename),
                        FileSize = file.FileSize,
                        FileSizeinKB = FormatSize((long)file.FileSize),
                        MimeType = file.MimeType,
                        Links = file.Links,
                    });
                }
            }
            return ListshowFilesResponseModels;
        }

        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        private async Task<AccessTokenItem> GetNewAuthToken(string resource)
        {
            try
            {
                var tokenCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = "9134ca48-663d-4a05-968a-31a42f0aed3e", ManagedIdentityClientId = "644e4406-4e92-4e5d-bdc5-3b233884f900" });
                var accessToken = await tokenCredential.GetTokenAsync(
                    new TokenRequestContext(scopes: new string[] { resource + "/.default" }) { }
                );
                return new AccessTokenItem
                {
                    ExpiresIn = DateTime.Now.AddHours(1)/*accessToken.ExpiresOn.UtcDateTime*/,
                    AccessToken = !String.IsNullOrEmpty(resource) ? "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjQ5MjM4OTIwLCJuYmYiOjE2NDkyMzg5MjAsImV4cCI6MTY0OTI0NDU2OSwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQW5YVFVTMWtDL3l4ZTJ5R1Nlc1JBVkk5NkJkNXFnTThYNDNkWlorQ1l4eGFGWFFySWpVSkVGVkJsVVMrZDJaUkJOQ3JrNXpMaEVXOW5XK2s4aElHSE9YckU1V1FsNnR1YlhDSURiUTZTZkpRPSIsImFtciI6WyJwd2QiLCJyc2EiXSwiYXBwaWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6Im1vaGFtbWUxNTMxNUBtYXN0ZWsuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYWRkMWM1MDAtYTZkNy00ZGJkLWI4OTAtN2Y4Y2I2ZjdkODYxLyIsImlwYWRkciI6IjIyMy4xODQuMjU0LjE1NCIsIm5hbWUiOiJNb2hhbW1lZCBLaGFuIiwib2lkIjoiMDEzMDU1Y2ItZWQ2Mi00NmQyLWFkZTgtMzhmY2NkNjQwYWE2IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FBNC4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiJnZHBHdUE3dVNmT0djRG5LZWZmTjUxdkFQNldraEo2V3Fsd05pWlIyT2o0IiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJtb2hhbW1lMTUzMTVAbWFzdGVrLmNvbSIsInV0aSI6IlcxeHZYbllfLVVpeS1POVZTTGU4QUEiLCJ2ZXIiOiIxLjAifQ.l0I0fST2hJoNKAZrNiCWINcCJf9E9odSTVPAegqF9ra2AHYS3Ba4WFHxP6KwT6KhreVc3nsRDQkASlmUOvqBxKhP0c5Xrl2l6w4I_6MmqqT81z1D3p9zbYKF7x4zUMfBlvzX6LW5czjTiocGC4iU42Mnil_H4ufVOPbXeu8dOfm05LZ2Rl8YKbyzRwg2V0l9XePXhWQpe9uFoKyDSfplmf2aeHETv1OwtY3sDVnjEXK5fuS5N9KsXM8eNfnq930IkszLAy11lj05yUXoQa7TTe8VZBN2mo9KTFGG6EYzDE4OFbGcRgQCORjT9ifr606p0Kc-fc2U9ayX4h0_Nvb9eg" : resource/*accessToken.Token*/
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<KeyValuePair<string, string>> GetPastYears()
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
        public List<KeyValuePair<string, string>> GetAllWeeksofYear(int year)
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
                string week =  (i+1).ToString();
                weeks.Add(new KeyValuePair<string, string>(week, week));
            }
            return weeks;
        }

    }
}
