using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class NMDataService : INMDataService
    {
        private readonly IFileShareService _fileShareService;
        private readonly ILogger<NMDataService> _logger;
        private readonly IAuthFssTokenProvider _authFssTokenProvider;
        public NMDataService(IFileShareService fileShareService, ILogger<NMDataService> logger, IAuthFssTokenProvider authFssTokenProvider)
        {
            _fileShareService = fileShareService;
            _logger = logger;
            _authFssTokenProvider = authFssTokenProvider;
        }

        public async Task<List<ShowFilesResponseModel>> GetWeeklyBatchFiles(int year, int week, string correlationId)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new();
            try
            {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                _logger.LogInformation(EventIds.GetWeeklyNMFilesRequestStarted.ToEventId(), "Maritime safety information request to get weekly NM files started for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);

                string searchText = $" and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";
                IResult<BatchSearchResponse> result = await _fileShareService.FssBatchSearchAsync(searchText, accessToken, correlationId);

                BatchSearchResponse SearchResult = result.Data;
                if (SearchResult != null && SearchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.GetWeeklyNMFilesRequestDataFound.ToEventId(), "Maritime safety information request to get weekly NM files returned data for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);

                    ListshowFilesResponseModels = NMHelper.ListFilesResponse(SearchResult).OrderBy(e => e.FileDescription).ToList();
                }
                else
                {
                    _logger.LogInformation(EventIds.GetWeeklyNMFilesRequestDataNotFound.ToEventId(), "Maritime safety information request to get weekly NM files returned no data for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetWeeklyNMFilesRequestFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}, year:{year} and week:{week}", ex.Message, correlationId, year, week);
                throw;
            }
            return ListshowFilesResponseModels;

        }

        public List<KeyValuePair<string, string>> GetAllYears(string correlationId)
        {
            List<KeyValuePair<string, string>> years = new();

            _logger.LogInformation(EventIds.GetAllYearsStarted.ToEventId(), "Maritime safety information request to get all years to populate year dropdown started", correlationId);

            years.Add(new KeyValuePair<string, string>("Year", ""));
            for (int i = 0; i < 3; i++)
            {
                string year = (DateTime.Now.Year - i).ToString();
                years.Add(new KeyValuePair<string, string>(year, year));
            }

            return years;
        }

        public List<SelectListItem> GetAllYearsSelectItem(string correlationId)
        {
            List<SelectListItem> years = new();

            _logger.LogInformation(EventIds.GetAllYearsStarted.ToEventId(), "Maritime safety information request to get all years to populate year dropdown started", correlationId);

            years.Add(new SelectListItem("Year", "0"));
            for (int i = 0; i < 3; i++)
            {
                string year = (DateTime.Now.Year - i).ToString();
                if (i == 0)
                    years.Add(new SelectListItem(year, year, true));
                else
                    years.Add(new SelectListItem(year, year));
            }

            return years;
        }

        public List<KeyValuePair<string, string>> GetAllWeeksOfYear(int year, string correlationId)
        {
            List<KeyValuePair<string, string>> weeks = new();

            _logger.LogInformation(EventIds.GetAllWeeksOfYearStarted.ToEventId(), "Maritime safety information request to get all weeks of year to populate week dropdown started", correlationId);

            weeks.Add(new KeyValuePair<string, string>("Week Number", ""));

            DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
            DateTime lastdate;
            if (DateTime.Now.Year == year)
            {
                lastdate = new DateTime(year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            {
                lastdate = new DateTime(year, 12, 31);
            }
            Calendar calender = dateTimeFormatInfo.Calendar;

            int totalWeeks = calender.GetWeekOfYear(lastdate, dateTimeFormatInfo.CalendarWeekRule,
                                                dateTimeFormatInfo.FirstDayOfWeek);

            for (int i = 0; i < totalWeeks; i++)
            {
                string week = (i + 1).ToString();
                weeks.Add(new KeyValuePair<string, string>(week, week));
            }

            return weeks;
        }

        public List<SelectListItem> GetAllWeeksOfYearSelectItem(int year, string correlationId)
        {
            List<SelectListItem> weeks = new();

            _logger.LogInformation(EventIds.GetAllWeeksOfYearStarted.ToEventId(), "Maritime safety information request to get all weeks of year to populate week dropdown started", correlationId);

            weeks.Add(new SelectListItem("Week Number", ""));

            DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
            DateTime lastdate;
            if (DateTime.Now.Year == year)
            {
                lastdate = new DateTime(year, DateTime.Now.Month, DateTime.Now.Day);
            }
            else
            {
                lastdate = new DateTime(year, 12, 31);
            }
            Calendar calender = dateTimeFormatInfo.Calendar;

            int totalWeeks = calender.GetWeekOfYear(lastdate, dateTimeFormatInfo.CalendarWeekRule,
                                                dateTimeFormatInfo.FirstDayOfWeek);

            for (int i = 0; i < totalWeeks; i++)
            {
                string week = (i + 1).ToString();
                weeks.Add(new SelectListItem(week, week));
            }

            return weeks;
        }

        public async Task<List<ShowDailyFilesResponseModel>> GetDailyBatchDetailsFiles(string correlationId)
        {
            List<ShowDailyFilesResponseModel> showDailyFilesResponses = new();
            try
            {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);
                _logger.LogInformation(EventIds.ShowDailyFilesResponseStarted.ToEventId(), "Maritime safety information request to get daily NM files response started with _X-Correlation-ID:{correlationId}", correlationId);

                const string searchText = $" and $batch(Frequency) eq 'Daily'";
                IResult<BatchSearchResponse> result = await _fileShareService.FssBatchSearchAsync(searchText, accessToken, correlationId);

                BatchSearchResponse searchResult = result.Data;

                if (searchResult != null && searchResult.Entries != null && searchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.ShowDailyFilesResponseDataFound.ToEventId(), "Maritime safety information request to get daily NM files response data found for _X-Correlation-ID:{correlationId}", correlationId);
                    showDailyFilesResponses = NMHelper.GetDailyShowFilesResponse(searchResult);
                }
                else
                {
                    _logger.LogInformation(EventIds.ShowDailyFilesResponseDataNotFound.ToEventId(), "Maritime safety information request to get daily NM files response data not found for _X-Correlation-ID:{correlationId}", correlationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowDailyFilesResponseFailed.ToEventId(), "Maritime safety information request to get daily NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }

            return showDailyFilesResponses;

        }

    }
}
