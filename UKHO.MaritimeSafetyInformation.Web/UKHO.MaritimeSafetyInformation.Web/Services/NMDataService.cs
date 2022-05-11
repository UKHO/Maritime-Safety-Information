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

        public async Task<List<YearWeekModel>> GetAllYearWeek(string correlationId)
        {
            List<YearWeekModel> yearWeekModelList = new();
            try
                {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataStarted.ToEventId(), "Request Search Attribute Year and week data from File Share Service started for _X-Correlation-ID:{correlationId}", correlationId);

                IResult<BatchAttributesSearchResponse> searchAttributes = await _fileShareService.FssSearchAttributeAsync(accessToken, correlationId);

                if (searchAttributes.Data != null)
                {
                    for (int i = 0; i < searchAttributes.Data.BatchAttributes.Count; i++)
                    {
                        if (searchAttributes.Data.BatchAttributes[i].Key == "YEAR / WEEK")
                        {
                            List<string> yearWeekList = searchAttributes.Data.BatchAttributes[i].Values;

                            if (yearWeekList != null && yearWeekList.Count != 0)
                            {
                                foreach (string yw in yearWeekList)
                                {
                                    string[] yearWeek = yw.Split('/');
                                    yearWeekModelList.Add(new YearWeekModel { Year = yearWeek[0].Trim(), Week = yearWeek[1].Trim() });
                                }
                                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataFound.ToEventId(), "Request Search Attribute Year and week data recieved successfully from File Share Service for BatchSearchAttribute with _X-Correlation-ID:{correlationId}" , correlationId);
                            }
                            else
                            {
                                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No Data recieved from File Share Service for Request Search Attribute Year and week for _X-Correlation-ID:{correlationId}", correlationId);
                            }
                        }
                    }
                }
                else
                    _logger.LogInformation(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No Data recieved from File Share Service for Request Search Attribute Year and week for _X-Correlation-ID:{correlationId}", correlationId);         
            }
            catch (Exception ex) {
                _logger.LogError(EventIds.GetSearchAttributeRequestDataFailed.ToEventId(), "Request Search Attribute Year and week data failed with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId );
                throw;
            }
            return yearWeekModelList;
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
