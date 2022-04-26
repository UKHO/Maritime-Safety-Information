using System.Globalization;
using Microsoft.Extensions.Options;
using Azure.Core;
using Microsoft.Identity.Client;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Helper;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models;
using UKHO.MaritimeSafetyInformation.Web.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class NMDataService : INMDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFileShareService fileShareService;
        private readonly IConfiguration configuration;
        private readonly ILogger<NMDataService> _logger;
        private readonly IOptions<FileShareServiceConfiguration> fileShareServiceConfig;
        private readonly NMHelper nMHelper;

        private readonly IAuthFssTokenProvider _authFssTokenProvider;
        public NMDataService(IFileShareService fileShareService, IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig, ILogger<NMDataService> logger, IAuthFssTokenProvider authFssTokenProvider)
        {
            this.fileShareService = fileShareService;
            this.httpClientFactory = httpClientFactory;
            this.fileShareServiceConfig = fileShareServiceConfig;
            _logger = logger;
            nMHelper = new NMHelper();
            _authFssTokenProvider = authFssTokenProvider;


        }
        public async Task<List<ShowFilesResponseModel>> GetBatchDetailsFiles(int year, int week)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new List<ShowFilesResponseModel>();
            try
            {
                AuthenticationResult authentication = await _authFssTokenProvider.GetAuthTokenAsync();
                string accessToken = authentication.AccessToken;

                _logger.LogInformation(EventIds.RetrievalOfMSIShowFilesResponseStarted.ToEventId(), "Maritime safety information request for show weekly files response started");

                string searchText = $"BusinessUnit eq '{fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{fileShareServiceConfig.Value.ProductType}' and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";
                var result = await fileShareService.FssWeeklySearchAsync(searchText, accessToken);

                BatchSearchResponse SearchResult = result.Data;
                if (SearchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.RetrievalOfMSIShowFilesResponseDataFound.ToEventId(), "Maritime safety information request for show weekly files response data found");

                    ListshowFilesResponseModels = nMHelper.GetShowFilesResponses(SearchResult);
                    ListshowFilesResponseModels = ListshowFilesResponseModels.OrderBy(e => e.FileDescription).ToList();
                }
                else {
                    _logger.LogInformation(EventIds.RetrievalOfMSIShowFilesResponseDataFoundNotFound.ToEventId(), "Maritime safety information request for show weekly files response data found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.RetrievalOfMSIShowFilesResponseFailed.ToEventId(), "Failed to get MSI response data {exceptionMessage} {exceptionTrace}", ex.Message, ex.StackTrace);
            }

            return ListshowFilesResponseModels;

        }


        public List<KeyValuePair<string, string>> GetPastYears()
        {
            List<KeyValuePair<string, string>> years = new List<KeyValuePair<string, string>>();
            try
            {
                _logger.LogInformation(EventIds.RetrievalOfMSIGetPastYearsStart.ToEventId(), "Maritime safety information request get past years started");

                years.Add(new KeyValuePair<string, string>("Year", ""));
                for (int i = 0; i < 3; i++)
                {
                    string year = (DateTime.Now.Year - i).ToString();
                    years.Add(new KeyValuePair<string, string>(year, year));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.RetrievalOfMSIGetPastYearsFailed.ToEventId(), "Failed to get past years data {exceptionMessage} {exceptionTrace}", ex.Message, ex.StackTrace);
            }
            return years;
        }
        public List<KeyValuePair<string, string>> GetAllWeeksofYear(int year)
        {
            List<KeyValuePair<string, string>> weeks = new List<KeyValuePair<string, string>>();
            try
            {
                _logger.LogInformation(EventIds.RetrievalOfMSIGetAllWeeksofYearStart.ToEventId(), "Maritime safety information request get all weeks of year started");

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
                    string week = (i + 1).ToString();
                    weeks.Add(new KeyValuePair<string, string>(week, week));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.RetrievalOfMSIGetAllWeeksofYearFailed.ToEventId(), "Failed to get all weeks of year data {exceptionMessage} {exceptionTrace}", ex.Message, ex.StackTrace);
            }
            return weeks;
        }

    }
}
