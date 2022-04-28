using Microsoft.Extensions.Options;
using System.Globalization;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common;
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
        private readonly IAuthFssTokenProvider _authFssTokenProvider;
        public NMDataService(IFileShareService fileShareService, ILogger<NMDataService> logger, IAuthFssTokenProvider authFssTokenProvider)
        {
            this.fileShareService = fileShareService;
            _logger = logger;
            _authFssTokenProvider = authFssTokenProvider;
        }

        public async Task<List<ShowFilesResponseModel>> GetNMBatchFiles(int year, int week, string correlationId)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new();
            try
            {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);               

                _logger.LogInformation(EventIds.GetNMBatchFilesRequestStarted.ToEventId(), "Maritime safety information request for show weekly files response started for _X-Correlation-ID:{correlationId}", correlationId);

                string searchText = $" and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";
                IResult<BatchSearchResponse> result = await fileShareService.FssBatchSearchAsync(searchText, accessToken, correlationId);

                BatchSearchResponse SearchResult = result.Data;
                if (SearchResult != null && SearchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.GetNMBatchFilesRequestDataFound.ToEventId(), "Maritime safety information request for show weekly files response data found for _X-Correlation-ID:{correlationId}", correlationId);

                    ListshowFilesResponseModels = NMHelper.GetShowFilesResponses(SearchResult);
                    ListshowFilesResponseModels = ListshowFilesResponseModels.OrderBy(e => e.FileDescription).ToList();
                }
                else
                {
                    _logger.LogInformation(EventIds.GetNMBatchFilesRequestDataFoundNotFound.ToEventId(), "Maritime safety information request for show weekly files response data found", correlationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetNMBatchFilesResponseFailed.ToEventId(), "Failed to get MSI response data {exceptionMessage} {exceptionTrace} for _X-Correlation-ID:{CorrelationId}", ex.Message, ex.StackTrace, correlationId);
                throw;
            }
            return ListshowFilesResponseModels;

        }

        public List<KeyValuePair<string, string>> GetPastYears(string correlationId)
        {
            List<KeyValuePair<string, string>> years = new();

            _logger.LogInformation(EventIds.GetPastYearsStarted.ToEventId(), "Maritime safety information request get past years started", correlationId);

            years.Add(new KeyValuePair<string, string>("Year", ""));
            for (int i = 0; i < 3; i++)
            {
                string year = (DateTime.Now.Year - i).ToString();
                years.Add(new KeyValuePair<string, string>(year, year));
            }

            return years;
        }
        public List<KeyValuePair<string, string>> GetAllWeeksofYear(int year, string correlationId)
        {
            List<KeyValuePair<string, string>> weeks = new();

            _logger.LogInformation(EventIds.GetAllWeeksofYearStarted.ToEventId(), "Maritime safety information request get all weeks of year started", correlationId);

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
