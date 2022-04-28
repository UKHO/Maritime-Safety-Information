using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Globalization;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helper;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class NMDataService : INMDataService
    {
        private readonly IFileShareService fileShareService;
        private readonly ILogger<NMDataService> _logger;
        private readonly IOptions<FileShareServiceConfiguration> fileShareServiceConfig;
        private readonly IAuthFssTokenProvider _authFssTokenProvider;
        public NMDataService(IFileShareService fileShareService, IOptions<FileShareServiceConfiguration> fileShareServiceConfig, ILogger<NMDataService> logger, IAuthFssTokenProvider authFssTokenProvider)
        {
            this.fileShareService = fileShareService;
            this.fileShareServiceConfig = fileShareServiceConfig;
            _logger = logger;
            _authFssTokenProvider = authFssTokenProvider;
        }

        public async Task<List<ShowFilesResponseModel>> GetBatchDetailsFiles(int year, int week)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new();
            try
            {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken();               

                _logger.LogInformation(EventIds.RetrievalOfMSIShowFilesResponseStarted.ToEventId(), "Maritime safety information request for show weekly files response started");

                string searchText = $"BusinessUnit eq '{fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{fileShareServiceConfig.Value.ProductType}' and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";
                IResult<BatchSearchResponse> result = await fileShareService.FssWeeklySearchAsync(searchText, accessToken);

                BatchSearchResponse SearchResult = result.Data;
                if (SearchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.RetrievalOfMSIShowFilesResponseDataFound.ToEventId(), "Maritime safety information request for show weekly files response data found");

                    ListshowFilesResponseModels = NMHelper.GetShowFilesResponses(SearchResult);
                    ListshowFilesResponseModels = ListshowFilesResponseModels.OrderBy(e => e.FileDescription).ToList();
                }
                else
                {
                    _logger.LogInformation(EventIds.RetrievalOfMSIShowFilesResponseDataFoundNotFound.ToEventId(), "Maritime safety information request for show weekly files response data found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.RetrievalOfMSIShowFilesResponseFailed.ToEventId(), "Failed to get MSI response data {exceptionMessage} {exceptionTrace}", ex.Message, ex.StackTrace);
                throw;
            }
            return ListshowFilesResponseModels;

        }

        public List<KeyValuePair<string, string>> GetPastYears()
        {
            List<KeyValuePair<string, string>> years = new();

            _logger.LogInformation(EventIds.RetrievalOfMSIGetPastYearsStart.ToEventId(), "Maritime safety information request get past years started");

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
            List<KeyValuePair<string, string>> weeks = new();

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

            return weeks;
        }

    }
}
