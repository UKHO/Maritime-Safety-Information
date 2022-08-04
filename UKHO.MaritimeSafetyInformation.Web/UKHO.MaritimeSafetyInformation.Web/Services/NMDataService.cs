using Microsoft.Extensions.Options;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private readonly IFileShareServiceCache _fileShareServiceCache;
        private readonly IOptions<CacheConfiguration> _cacheConfiguration;
        private readonly IUserService _userService;
        private const string YearAndWeek = "YEAR/WEEK";
        private string PartitionKey => _userService.IsDistributorUser ? "Distributor" : "Public";

        public NMDataService(IFileShareService fileShareService, ILogger<NMDataService> logger, IAuthFssTokenProvider authFssTokenProvider, IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig,
                             IFileShareServiceCache fileShareServiceCache, IOptions<CacheConfiguration> cacheConfiguration, IUserService userService)
        {
            _fileShareService = fileShareService;
            _logger = logger;
            _authFssTokenProvider = authFssTokenProvider;
            _fileShareServiceConfig = fileShareServiceConfig;
            _httpClientFactory = httpClientFactory;
            _fileShareServiceCache = fileShareServiceCache;
            _cacheConfiguration = cacheConfiguration;
            _userService = userService;
        }

        public async Task<ShowNMFilesResponseModel> GetWeeklyBatchFiles(int year, int week, string correlationId)
        {
            try
            {
                BatchSearchResponse searchResult = new();
                bool isCached = false;
                const string frequency = "Weekly";

                _logger.LogInformation(EventIds.GetWeeklyNMFilesRequestStarted.ToEventId(), "Maritime safety information request to get weekly NM files started for year:{year} and week:{week} for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", year, week, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);

                if (_cacheConfiguration.Value.IsFssCacheEnabled)
                {
                    BatchSearchResponseModel batchSearchResponseModel = await _fileShareServiceCache.GetWeeklyBatchResponseFromCache(year, week, PartitionKey, correlationId);

                    if (batchSearchResponseModel.BatchSearchResponse != null)
                    {
                        searchResult = batchSearchResponseModel.BatchSearchResponse;
                        isCached = true;
                    }
                }

                if (searchResult.Entries == null)
                {
                    string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

                    string searchText = $" and $batch(Frequency) eq '{frequency}' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";

                    IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                    IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId, fileShareApiClient);
                    searchResult = result.Data;
                }

                if (searchResult != null && searchResult.Entries.Count > 0)
                {
                    if (_cacheConfiguration.Value.IsFssCacheEnabled && !isCached)
                    {
                        string rowKey = $"{year}|{week}";

                        _logger.LogInformation(EventIds.FSSSearchWeeklyBatchFilesResponseStoreToCacheStart.ToEventId(), "Request for storing file share service search weekly NM files response in azure table storage is started for year:{year} and week:{week} for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", year, week, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);

                        await _fileShareServiceCache.InsertCacheObject(searchResult, rowKey, _cacheConfiguration.Value.FssWeeklyBatchSearchTableName, frequency, PartitionKey, correlationId);

                        _logger.LogInformation(EventIds.FSSSearchWeeklyBatchFilesResponseStoreToCacheCompleted.ToEventId(), "Request for storing file share service search weekly NM files response in azure table storage is completed for year:{year} and week:{week} for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", year, week, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                    }
                    _logger.LogInformation(EventIds.GetWeeklyNMFilesRequestDataFound.ToEventId(), "Maritime safety information request to get weekly NM files returned from FSS for year:{year} and week:{week} for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", year, week, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);

                    List<ShowFilesResponseModel> listshowFilesResponseModels = NMHelper.ListFilesResponse(searchResult).OrderBy(e => e.FileDescription).ToList();
                    return new ShowNMFilesResponseModel() { ShowFilesResponseModel = listshowFilesResponseModels, IsBatchResponseCached = isCached };
                }
                else
                {
                    _logger.LogError(EventIds.GetWeeklyNMFilesRequestDataNotFound.ToEventId(), "Maritime safety information request to get weekly NM files returned no data for year:{year} and week:{week} for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", year, week, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                    return new ShowNMFilesResponseModel() { ShowFilesResponseModel = new List<ShowFilesResponseModel>(), IsBatchResponseCached = isCached };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetWeeklyNMFilesRequestFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for year:{year} and week:{week} for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{CorrelationId}", ex.Message, year, week, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                throw;
            }
        }

        public async Task<YearWeekResponseDataModel> GetAllYearWeek(string correlationId)
        {
            List<YearWeekModel> yearWeekModelList = new();
            BatchAttributesSearchModel searchAttributes = new();
            const string rowKey = "BatchAttributeKey";
            bool isCached = false;

            try
            {
                if (_cacheConfiguration.Value.IsFssCacheEnabled)
                {
                    searchAttributes = await _fileShareServiceCache.GetAllYearsAndWeeksFromCache(rowKey, PartitionKey, correlationId);
                    if (searchAttributes.Data != null)
                    {
                        isCached = true;
                    }
                }

                if (searchAttributes.Data == null)
                {
                    string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

                    _logger.LogInformation(EventIds.GetSearchAttributeRequestDataStarted.ToEventId(), "Request to search attribute year and week data from File Share Service started for _X-Correlation-ID:{correlationId}", correlationId);

                    IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                    IResult<BatchAttributesSearchResponse> attributes = await _fileShareService.FSSSearchAttributeAsync(accessToken, correlationId, fileShareApiClient);

                    searchAttributes = new()
                    {
                        Data = attributes.Data,
                        Errors = attributes.Errors,
                        IsSuccess = attributes.IsSuccess,
                        StatusCode = attributes.StatusCode
                    };

                    if (_cacheConfiguration.Value.IsFssCacheEnabled)
                    {
                        _logger.LogInformation(EventIds.FSSSearchAllYearWeekResponseStoreToCacheStart.ToEventId(), "Request for storing file share service search attribute year and week data response in azure table storage is started for with _X-Correlation-ID:{correlationId}", correlationId);

                        await _fileShareServiceCache.InsertCacheObject(searchAttributes, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName, "BatchAttribute", PartitionKey, correlationId);

                        _logger.LogInformation(EventIds.FSSSearchAllYearWeekResponseStoreToCacheCompleted.ToEventId(), "Request for storing file share service search attribute year and week data response in azure table storage is completed for _X-Correlation-ID:{correlationId}", correlationId);
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
                                _logger.LogInformation(EventIds.GetSearchAttributeRequestDataFound.ToEventId(), "Request to search attribute year and week data completed successfully from File Share Service for BatchSearchAttribute for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                            }
                            else
                            {
                                _logger.LogError(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No data received from File Share Service for request to search attribute year and week for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                                throw new InvalidDataException("No data received from File Share Service for request to search attribute year and week");
                            }
                            break;
                        }
                    }
                }
                else
                {
                    _logger.LogError(EventIds.GetSearchAttributeRequestDataNotFound.ToEventId(), "No data received from File Share Service for request to search attribute year and week for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{correlationId}", _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                    throw new InvalidDataException("No Data received from File Share Service for request to search attribute year and week");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetSearchAttributeRequestDataFailed.ToEventId(), "Request to search attribute year and week data failed with exception:{exceptionMessage} for User:{SignInName} and Identity:{UserIdentifier} with _X-Correlation-ID:{CorrelationId}", ex.Message, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                throw;
            }

            return new YearWeekResponseDataModel { YearWeekModel = yearWeekModelList, IsYearAndWeekAttributesCached = isCached };
        }

        public async Task<ShowDailyFilesResponseListModel> GetDailyBatchDetailsFiles(string correlationId)
        {
            try
            {
                BatchSearchResponse searchResult = new();
                bool isCached = false;
                const string frequency = "Daily";
                const string rowKey = "DailyKey";

                if (_cacheConfiguration.Value.IsFssCacheEnabled)
                {
                    BatchSearchResponseModel batchSearchResponseModel = await _fileShareServiceCache.GetBatchResponseFromCache(PartitionKey, rowKey, frequency, correlationId);

                    if (batchSearchResponseModel.BatchSearchResponse != null)
                    {
                        searchResult = batchSearchResponseModel.BatchSearchResponse;
                        isCached = true;
                    }
                }

                if (searchResult.Entries == null)
                {
                    string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

                    _logger.LogInformation(EventIds.ShowDailyFilesResponseStarted.ToEventId(), "Maritime safety information request to get daily NM files response started for daily user:{SignInName} and Identity:{userId} with _X-Correlation-ID:{correlationId}", _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);

                    string searchText = $" and $batch(Frequency) eq '{frequency}' and $batch(Content) eq null ";
                    if (_userService.IsDistributorUser)
                    {
                        searchText = $" and $batch(Frequency) eq '{frequency}' and $batch(Content) eq 'Tracings' ";
                    }

                    IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                    IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId, fileShareApiClient);

                    searchResult = result.Data;

                    if (_cacheConfiguration.Value.IsFssCacheEnabled && searchResult != null && searchResult.Entries.Count > 0)
                    {
                        _logger.LogInformation(EventIds.FSSDailyBatchFilesResponseStoreToCacheStart.ToEventId(), "Request for storing file share service daily NM files response in azure table storage is started for with _X-Correlation-ID:{correlationId}", correlationId);

                        await _fileShareServiceCache.InsertCacheObject(searchResult, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName, frequency, PartitionKey, correlationId);

                        _logger.LogInformation(EventIds.FSSDailyBatchFilesResponseStoreToCacheCompleted.ToEventId(), "Request for storing file share service daily NM files response in azure table storage is completed for with _X-Correlation-ID:{correlationId}", correlationId);
                    }
                }

                if (searchResult != null && searchResult.Entries != null && searchResult.Entries.Count > 0)
                {
                    _logger.LogInformation(EventIds.ShowDailyFilesResponseDataFound.ToEventId(), "Maritime safety information request to get daily NM files response data found for user:{SignInName} and Identity:{userId} with _X-Correlation-ID:{correlationId}", _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);

                    ShowDailyFilesResponseListModel showDailyFilesResponseListModel = new()
                    {
                        ShowDailyFilesResponseModel = NMHelper.GetDailyShowFilesResponse(searchResult),
                        IsDailyFilesResponseCached = isCached
                    };
                    return showDailyFilesResponseListModel;
                }
                else
                {
                    _logger.LogError(EventIds.ShowDailyFilesResponseDataNotFound.ToEventId(), "Maritime safety information request to get daily NM files response data not found for user:{SignInName} and Identity:{userId} with _X-Correlation-ID:{correlationId}", _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                    throw new InvalidDataException("Invalid data received for daily NM files");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowDailyFilesResponseFailed.ToEventId(), "Maritime safety information request to get daily NM files failed to return data with exception:{exceptionMessage} for user:{SignInName} and Identity:{userId} with _X-Correlation-ID:{correlationId}", ex.Message, _userService.SignInName ?? "Public", _userService.UserIdentifier, correlationId);
                throw;
            }
        }

        public async Task<ShowNMFilesResponseModel> GetLeisureFilesAsync(string correlationId)
        {
            try
            {
                BatchSearchResponse searchResult = new();
                bool isCached = false;
                const string frequency = "leisure";
                const string partitionKey = "Public";
                const string rowKey = "LeisureKey";
                if (_cacheConfiguration.Value.IsFssCacheEnabled)
                {
                    BatchSearchResponseModel batchSearchResponseModel = await _fileShareServiceCache.GetBatchResponseFromCache(partitionKey, rowKey, frequency, correlationId);

                    if (batchSearchResponseModel.BatchSearchResponse != null)
                    {
                        searchResult = batchSearchResponseModel.BatchSearchResponse;
                        isCached = true;
                    }
                }
                if (searchResult.Entries == null)
                {
                    string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

                    _logger.LogInformation(EventIds.ShowLeisureFilesResponseStarted.ToEventId(), "Request to get leisure files started with caching data:{isCached} and _X-Correlation-ID:{correlationId}", isCached, correlationId);

                    const string searchText = $" and $batch(Frequency) eq '{frequency}'";

                    IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                    IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId, fileShareApiClient);

                    searchResult = result.Data;
                }
                if (searchResult != null && searchResult.Entries != null && searchResult.Entries.Count > 0)
                {
                    if (_cacheConfiguration.Value.IsFssCacheEnabled && !isCached)
                    {                       
                        _logger.LogInformation(EventIds.FSSLeisureBatchFilesResponseStoreToCacheStart.ToEventId(), "Request for storing file share service search leisure NM files response in azure table storage is started with caching data:{isCached} and _X-Correlation-ID:{correlationId}", isCached, correlationId);

                        await _fileShareServiceCache.InsertCacheObject(searchResult, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName, frequency, partitionKey, correlationId);

                        _logger.LogInformation(EventIds.FSSLeisureBatchFilesResponseStoreToCacheCompleted.ToEventId(), "Request for storing file share service search leisure NM files response in azure table storage is completed with caching data:{isCached} and _X-Correlation-ID:{correlationId}", isCached, correlationId);
                    }
                   
                    List<ShowFilesResponseModel> ListshowFilesResponseModels = NMHelper.ListFilesResponseLeisure(searchResult);
                    ShowNMFilesResponseModel showNMFilesResponseModel = new() { ShowFilesResponseModel = ListshowFilesResponseModels, IsBatchResponseCached = isCached };
                    _logger.LogInformation(EventIds.ShowLeisureFilesResponseDataFound.ToEventId(), "Request to get leisure files completed and data found with caching data:{isCached} and _X-Correlation-ID:{correlationId}", isCached, correlationId);
                    return showNMFilesResponseModel;
                }
                else
                {
                    _logger.LogError(EventIds.ShowLeisureFilesResponseDataNotFound.ToEventId(), "Request to get leisure files completed and data not found with caching data:{isCached} and _X-Correlation-ID:{correlationId}", isCached, correlationId);
                    throw new InvalidDataException("Invalid data received for leisure files");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ShowLeisureFilesResponseFailed.ToEventId(), "Request to get leisure files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<ShowWeeklyFilesResponseModel> GetWeeklyFilesResponseModelsAsync(int year, int week, string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.GetWeeklyFilesResponseStarted.ToEventId(), "Maritime safety information request to get weekly NM files response started with _X-Correlation-ID:{correlationId}", correlationId);

                ShowWeeklyFilesResponseModel showWeeklyFilesResponses = new();

                YearWeekResponseDataModel yearWeekResponseDataModel = await GetAllYearWeek(correlationId);
                showWeeklyFilesResponses.YearAndWeekList = yearWeekResponseDataModel.YearWeekModel;
                showWeeklyFilesResponses.IsYearAndWeekAttributesCached = yearWeekResponseDataModel.IsYearAndWeekAttributesCached;

                if (year == 0 && week == 0)
                {
                    year = Convert.ToInt32(showWeeklyFilesResponses.YearAndWeekList.OrderByDescending(x => x.Year).Select(x => x.Year).FirstOrDefault());
                    week = Convert.ToInt32(showWeeklyFilesResponses.YearAndWeekList.OrderByDescending(x => x.Week).Where(x => x.Year == year).Select(x => x.Week).FirstOrDefault());
                }

                ShowNMFilesResponseModel showNMFilesResponseModel = await GetWeeklyBatchFiles(year, week, correlationId);
                showWeeklyFilesResponses.ShowFilesResponseList = showNMFilesResponseModel.ShowFilesResponseModel;
                showWeeklyFilesResponses.IsWeeklyBatchResponseCached = showNMFilesResponseModel.IsBatchResponseCached;

                _logger.LogInformation(EventIds.GetWeeklyFilesResponseCompleted.ToEventId(), "Maritime safety information request to get weekly NM files response completed with _X-Correlation-ID:{correlationId}", correlationId);

                return showWeeklyFilesResponses;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetWeeklyFilesResponseFailed.ToEventId(), "Maritime safety information request to get weekly NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<byte[]> DownloadFssFileAsync(string batchId, string fileName, string correlationId, string frequency)
        {
            try
            {
                _logger.LogInformation(EventIds.GetSingleNMFileStarted.ToEventId(), "Maritime safety information request to get single {frequency} NM file started for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", frequency, batchId, fileName, correlationId);

                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

                IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                Stream stream = await _fileShareService.FSSDownloadFileAsync(batchId, fileName, accessToken, correlationId, fileShareApiClient, frequency);

                byte[] fileBytes = await NMHelper.GetFileBytesFromStream(stream);

                _logger.LogInformation(EventIds.GetSingleNMFileCompleted.ToEventId(), "Maritime safety information request to get single {frequency} NM file completed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", frequency, batchId, fileName, correlationId);

                return fileBytes;
            }
            catch (Exception)
            {
                _logger.LogInformation(EventIds.GetSingleNMFileFailed.ToEventId(), "Maritime safety information request to get single {frequency} NM file failed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", frequency, batchId, fileName, correlationId);
                throw;
            }

        }

        public async Task<byte[]> DownloadFSSZipFileAsync(string batchId, string fileName, string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.GetDailyZipNMFileStarted.ToEventId(), "Maritime safety information request to get daily zip NM file started for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

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

        public async Task<ShowNMFilesResponseModel> GetCumulativeBatchFiles(string correlationId)
        {
            try
            {
                BatchSearchResponse searchResult = new();
                bool isCached = false;
                const string frequency = "Cumulative";
                const string partitionKey = "Public";
                const string rowKey = "CumulativeKey";

                if (_cacheConfiguration.Value.IsFssCacheEnabled)
                {
                    BatchSearchResponseModel batchSearchResponseModel = await _fileShareServiceCache.GetBatchResponseFromCache(partitionKey, rowKey, frequency, correlationId);

                    if (batchSearchResponseModel.BatchSearchResponse != null)
                    {
                        searchResult = batchSearchResponseModel.BatchSearchResponse;
                        isCached = true;
                    }
                }

                if (searchResult.Entries == null)
                {
                    _logger.LogInformation(EventIds.GetCumulativeFilesResponseStarted.ToEventId(), "Maritime safety information request to get cumulative NM files response started with _X-Correlation-ID:{correlationId}", correlationId);

                    string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

                    const string searchText = $" and $batch(Frequency) eq '{frequency}'";

                    IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                    IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId, fileShareApiClient);

                    searchResult = result.Data;

                    if (_cacheConfiguration.Value.IsFssCacheEnabled && searchResult != null && searchResult.Entries.Count > 0)
                    {
                        _logger.LogInformation(EventIds.FSSSearchCumulativeBatchFilesResponseStoreToCacheStart.ToEventId(), "Request for storing file share service search cumulative NM files response in azure table storage is started with _X-Correlation-ID:{correlationId}", correlationId);

                        await _fileShareServiceCache.InsertCacheObject(searchResult, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName, frequency, partitionKey, correlationId);

                        _logger.LogInformation(EventIds.FSSSearchCumulativeBatchFilesResponseStoreToCacheCompleted.ToEventId(), "Request for storing file share service search cumulative NM files response in azure table storage is completed with _X-Correlation-ID:{correlationId}", correlationId);
                    }
                }

                if (searchResult != null && searchResult.Entries.Count > 0)
                {
                    List<ShowFilesResponseModel> showFilesResponseModel = NMHelper.ListFilesResponseCumulative(searchResult.Entries);
                    ShowNMFilesResponseModel showNMFilesResponseModel = new() { ShowFilesResponseModel = showFilesResponseModel, IsBatchResponseCached = isCached };
                    _logger.LogInformation(EventIds.GetCumulativeFilesResponseCompleted.ToEventId(), "Maritime safety information request to get cumulative NM files response completed with _X-Correlation-ID:{correlationId}", correlationId);
                    return showNMFilesResponseModel;
                }
                _logger.LogError(EventIds.GetCumulativeNMFilesRequestDataNotFound.ToEventId(), "Maritime safety information request to get cumulative NM files returned no data with _X-Correlation-ID:{correlationId}", correlationId);
                throw new InvalidDataException("Invalid data received for cumulative NM files");
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetCumulativeFilesResponseFailed.ToEventId(), "Maritime safety information request to cumulative NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<List<ShowFilesResponseModel>> GetAnnualBatchFiles(string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.GetAnnualFilesResponseStarted.ToEventId(), "Maritime safety information request to get annual NM files response started with _X-Correlation-ID:{correlationId}", correlationId);

                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(_userService.IsDistributorUser, correlationId);

                const string searchText = $" and $batch(Frequency) eq 'annual'";

                IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                IResult<BatchSearchResponse> result = await _fileShareService.FSSBatchSearchAsync(searchText, accessToken, correlationId, fileShareApiClient);

                BatchSearchResponse searchResult = result.Data;

                if (searchResult != null && searchResult.Entries.Count > 0)
                {
                    List<ShowFilesResponseModel> showFilesResponseModel = NMHelper.GetShowAnnualFilesResponse(searchResult.Entries).ToList();
                    _logger.LogInformation(EventIds.GetAnnualFilesResponseCompleted.ToEventId(), "Maritime safety information request to get annual NM files response completed with _X-Correlation-ID:{correlationId}", correlationId);
                    return showFilesResponseModel;
                }
                else
                {
                    _logger.LogError(EventIds.GetAnnualNMFilesRequestDataNotFound.ToEventId(), "Maritime safety information request to get annual NM files returned no data with _X-Correlation-ID:{correlationId}", correlationId);
                    throw new InvalidDataException("Invalid data received for annual NM files");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.GetAnnualFilesResponseFailed.ToEventId(), "Maritime safety information request to annual NM files failed to return data with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }
    }
}
