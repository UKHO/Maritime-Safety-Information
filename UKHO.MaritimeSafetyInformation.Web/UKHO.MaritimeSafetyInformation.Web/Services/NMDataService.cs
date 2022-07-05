using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private readonly IAzureTableStorageClient _azureTableStorageClient;
        private readonly IOptions<CacheConfiguration> _cacheConfiguration;

        public NMDataService(IFileShareService fileShareService, ILogger<NMDataService> logger, IAuthFssTokenProvider authFssTokenProvider, IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig,
                             IAzureTableStorageClient azureTableStorageClient, IOptions<CacheConfiguration> cacheConfiguration)
        {
            _fileShareService = fileShareService;
            _logger = logger;
            _authFssTokenProvider = authFssTokenProvider;
            _fileShareServiceConfig = fileShareServiceConfig;
            _httpClientFactory = httpClientFactory;
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
        }

        public async Task<List<ShowFilesResponseModel>> GetWeeklyBatchFiles(int year, int week, string correlationId)
        {
            try
            {

                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                _logger.LogInformation(EventIds.GetWeeklyNMFilesRequestStarted.ToEventId(), "Maritime safety information request to get weekly NM files started for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);

                string searchText = $" and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";

                IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId, fileShareApiClient);

                BatchSearchResponse SearchResult = result.Data;

                if (SearchResult != null && SearchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.GetWeeklyNMFilesRequestDataFound.ToEventId(), "Maritime safety information request to get weekly NM files returned data for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);

                    List<ShowFilesResponseModel> ListshowFilesResponseModels = NMHelper.ListFilesResponse(SearchResult).OrderBy(e => e.FileDescription).ToList();
                    return ListshowFilesResponseModels;
                }
                else
                {
                    _logger.LogError(EventIds.GetWeeklyNMFilesRequestDataNotFound.ToEventId(), "Maritime safety information request to get weekly NM files returned no data for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);
                    throw new InvalidDataException("Invalid data received for weekly NM files");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetWeeklyNMFilesRequestFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}, year:{year} and week:{week}", ex.Message, correlationId, year, week);
                throw;
            }

        }

        public async Task<List<YearWeekModel>> GetAllYearWeek(string correlationId)
        {
            List<YearWeekModel> yearWeekModelList = new();
            IResult<BatchAttributesSearchResponse> searchAttributes;

            try
            {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataStarted.ToEventId(), "Request to search attribute year and week data from File Share Service started for _X-Correlation-ID:{correlationId}", correlationId);

                CustomTableEntity cacheInfo = await _azureTableStorageClient.GetEntityAsync("Public","", _cacheConfiguration.Value.FssWeeklyAttributeTableName, _cacheConfiguration.Value.ConnectionString);
                
                if (_cacheConfiguration.Value.IsFssCacheEnabled && cacheInfo != null && !string.IsNullOrEmpty(cacheInfo.Response))
                {
                    searchAttributes = JsonConvert.DeserializeObject<IResult<BatchAttributesSearchResponse>>(cacheInfo.Response);
                }
                else
                {
                    IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                    searchAttributes = await _fileShareService.FSSSearchAttributeAsync(accessToken, correlationId, fileShareApiClient);

                    if (_cacheConfiguration.Value.IsFssCacheEnabled)
                    {
                        CustomTableEntity customTableEntity = new()
                        {
                            PartitionKey = "Public",
                            RowKey = "",
                            Response = JsonConvert.SerializeObject(searchAttributes)
                        };

                        await _azureTableStorageClient.InsertEntityAsync(customTableEntity, _cacheConfiguration.Value.FssWeeklyAttributeTableName, _cacheConfiguration.Value.ConnectionString);
                    }
                }

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
                                _logger.LogError(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No data received from File Share Service for request to search attribute year and week for _X-Correlation-ID:{correlationId}", correlationId);
                                throw new InvalidDataException("No data received from File Share Service for request to search attribute year and week");
                            }
                            break;
                        }
                    }
                }
                else
                {
                    _logger.LogError(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No data received from File Share Service for request to search attribute year and week for _X-Correlation-ID:{correlationId}", correlationId);
                    throw new InvalidDataException("No Data received from File Share Service for request to search attribute year and week");
                }
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
            try
            {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                _logger.LogInformation(EventIds.ShowDailyFilesResponseStarted.ToEventId(), "Maritime safety information request to get daily NM files response started with _X-Correlation-ID:{correlationId}", correlationId);

                const string searchText = $" and $batch(Frequency) eq 'Daily'";

                IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId, fileShareApiClient);

                BatchSearchResponse searchResult = result.Data;

                if (searchResult != null && searchResult.Entries != null && searchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.ShowDailyFilesResponseDataFound.ToEventId(), "Maritime safety information request to get daily NM files response data found for _X-Correlation-ID:{correlationId}", correlationId);
                    List<ShowDailyFilesResponseModel> showDailyFilesResponses = NMHelper.GetDailyShowFilesResponse(searchResult);
                    return showDailyFilesResponses;
                }
                else
                {
                    _logger.LogError(EventIds.ShowDailyFilesResponseDataNotFound.ToEventId(), "Maritime safety information request to get daily NM files response data not found for _X-Correlation-ID:{correlationId}", correlationId);
                    throw new InvalidDataException("Invalid data received for daily NM files");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowDailyFilesResponseFailed.ToEventId(), "Maritime safety information request to get daily NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<ShowWeeklyFilesResponseModel> GetWeeklyFilesResponseModelsAsync(int year, int week, string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.GetWeeklyFilesResponseStarted.ToEventId(), "Maritime safety information request to get weekly NM files response started with _X-Correlation-ID:{correlationId}", correlationId);

                ShowWeeklyFilesResponseModel showWeeklyFilesResponses = new();

                showWeeklyFilesResponses.YearAndWeekList = await GetAllYearWeek(correlationId);
                if (year == 0 && week == 0)
                {
                    year = Convert.ToInt32(showWeeklyFilesResponses.YearAndWeekList.OrderByDescending(x => x.Year).Select(x => x.Year).FirstOrDefault());
                    week = Convert.ToInt32(showWeeklyFilesResponses.YearAndWeekList.OrderByDescending(x => x.Week).Where(x => x.Year == year).Select(x => x.Week).FirstOrDefault());
                }
                showWeeklyFilesResponses.ShowFilesResponseList = await GetWeeklyBatchFiles(year, week, correlationId);
                
                _logger.LogInformation(EventIds.GetWeeklyFilesResponseCompleted.ToEventId(), "Maritime safety information request to get weekly NM files response completed with _X-Correlation-ID:{correlationId}", correlationId);

                return showWeeklyFilesResponses;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetWeeklyFilesResponseFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<byte[]> DownloadFssFileAsync(string batchId, string fileName, string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.GetSingleWeeklyNMFileStarted.ToEventId(), "Maritime safety information request to get single weekly NM file started for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                Stream stream = await _fileShareService.FSSDownloadFileAsync(batchId, fileName, accessToken, correlationId, fileShareApiClient);

                byte[] fileBytes = await NMHelper.GetFileBytesFromStream(stream);

                _logger.LogInformation(EventIds.GetSingleWeeklyNMFileCompleted.ToEventId(), "Maritime safety information request to get single weekly NM file completed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                return fileBytes;
            }
            catch (Exception)
            {
                _logger.LogInformation(EventIds.GetSingleWeeklyNMFileFailed.ToEventId(), "Maritime safety information request to get single weekly NM file failed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);
                throw;
            }

        }

        public async Task<byte[]> DownloadFSSZipFileAsync(string batchId, string fileName, string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.GetDailyZipNMFileStarted.ToEventId(), "Maritime safety information request to get daily zip NM file started for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(correlationId);

                IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                Stream stream = await _fileShareService.FSSDownloadZipFileAsync(batchId, fileName, accessToken, correlationId, fileShareApiClient);

                byte[] fileBytes = await NMHelper.GetFileBytesFromStream(stream);

                _logger.LogInformation(EventIds.GetDailyZipNMFileCompleted.ToEventId(), "Maritime safety information request to get daily zip NM file completed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                return fileBytes;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(EventIds.GetDailyZipNMFileFailed.ToEventId(), "Maritime safety information request to get daily zip NM file failed for batchId:{batchId} and fileName:{fileName} with exception:{exceptionMessage} for _X-Correlation-ID:{correlationId}", batchId, fileName, ex.Message, correlationId);
                throw;
            }

        }
    }
}
