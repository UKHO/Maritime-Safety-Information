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
        private const string YearAndWeek = "YEAR/WEEK";

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
                IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId);

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

                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataStarted.ToEventId(), "Request to search attribute year and week data from File Share Service started for _X-Correlation-ID:{correlationId}", correlationId);

                IResult<BatchAttributesSearchResponse> searchAttributes = await _fileShareService.FSSSearchAttributeAsync(accessToken, correlationId);

                if (searchAttributes.Data != null)
                {
                    foreach (var attribute in searchAttributes.Data.BatchAttributes)
                    {
                        if (attribute.Key.Replace(" ", string.Empty) == YearAndWeek)
                        {
                            List<string> yearWeekList = attribute.Values;

                            if (yearWeekList != null && yearWeekList.Count != 0)
                            {
                                foreach (string yw in yearWeekList)
                                {
                                    string[] yearWeek = yw.Contains('/') ? yw.Split('/') : null;

                                    if (yearWeek != null && yearWeek.Length >= 2)
                                    {
                                        yearWeekModelList.Add(new YearWeekModel { Year = Convert.ToInt32(yearWeek[0].Trim()), Week = Convert.ToInt32(yearWeek[1].Trim()) });
                                    }
                                }
                                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataFound.ToEventId(), "Request to search attribute year and week data completed successfully from File Share Service for BatchSearchAttribute with _X-Correlation-ID:{correlationId}", correlationId);
                            }
                            else
                            {
                                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No Data received from File Share Service for request to search attribute year and week for _X-Correlation-ID:{correlationId}", correlationId);
                            }
                            break;
                        }
                    }
                }
                else
                    _logger.LogInformation(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No Data received from File Share Service for request to search attribute year and week for _X-Correlation-ID:{correlationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetSearchAttributeRequestDataFailed.ToEventId(), "Request to search attribute year and week data failed with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
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
                IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId);

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

        public async Task<ShowWeeklyFilesResponseModel> GetWeeklyFilesResponseModelsAsync(int year, int week, string correlationId)
        {
            _logger.LogInformation(EventIds.GetWeeklyFilesResponseStarted.ToEventId(), "Maritime safety information request to get weekly NM files response started with _X-Correlation-ID:{correlationId}", correlationId);
            ShowWeeklyFilesResponseModel showWeeklyFilesResponses = new();
            try
            {
                showWeeklyFilesResponses.YearAndWeekList = await GetAllYearWeek(correlationId);
                if (year == 0 && week == 0)
                {
                    year = Convert.ToInt32(showWeeklyFilesResponses.YearAndWeekList.OrderByDescending(x => x.Year).Select(x => x.Year).FirstOrDefault());
                    week = Convert.ToInt32(showWeeklyFilesResponses.YearAndWeekList.OrderByDescending(x => x.Week).Where(x => x.Year == year).Select(x => x.Week).FirstOrDefault());
                }
                showWeeklyFilesResponses.ShowFilesResponseList = await GetWeeklyBatchFiles(year, week, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetWeeklyFilesResponseFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }

            _logger.LogInformation(EventIds.GetWeeklyFilesResponseCompleted.ToEventId(), "Maritime safety information request to get weekly NM files response completed with _X-Correlation-ID:{correlationId}", correlationId);

            return showWeeklyFilesResponses;
        }
        public async Task<byte[]> DownloadFssFileAsync(string batchId, string fileName, string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.GetSingleWeeklyNMFileStarted.ToEventId(), "Maritime safety information request to get single weekly NM file started for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                Stream stream = await _fileShareService.FSSDownloadFileAsync(batchId, fileName, accessToken, correlationId);

                byte[] fileBytes = new byte[stream.Length + 10];

                int numBytesToRead = (int)stream.Length;
                int numBytesRead = 0;
                do
                {
                    int n = await stream.ReadAsync(fileBytes, numBytesRead, numBytesToRead);
                    numBytesRead += n;
                    numBytesToRead -= n;
                } while (numBytesToRead > 0);
                stream.Close();

                _logger.LogInformation(EventIds.GetSingleWeeklyNMFileCompleted.ToEventId(), "Maritime safety information request to get single weekly NM file completed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                return fileBytes;
            }
            catch (Exception)
            {
                _logger.LogInformation(EventIds.GetSingleWeeklyNMFileFailed.ToEventId(), "Maritime safety information request to get single weekly NM file failed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);
                throw;
            }

        }
    }
}
