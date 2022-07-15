using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class FileShareServiceCache : IFileShareServiceCache
    {
        private readonly IAzureTableStorageClient _azureTableStorageClient;
        private readonly IOptions<CacheConfiguration> _cacheConfiguration;
        private readonly IAzureStorageService _azureStorageService;
        private readonly ILogger<FileShareServiceCache> _logger;
        private readonly IUserService _userService;

        public FileShareServiceCache(IAzureTableStorageClient azureTableStorageClient, IOptions<CacheConfiguration> cacheConfiguration, IAzureStorageService azureStorageService, ILogger<FileShareServiceCache> logger, IUserService userService)
        {
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
            _azureStorageService = azureStorageService;
            _logger = logger;
            _userService = userService;
        }

        public async Task<BatchAttributesSearchModel> GetAllYearWeekFromCache(string rowKey, string correlationId)
        {
            BatchAttributesSearchModel searchResult = new();
            try
            {
                string partitionKey = _userService.IsDistributorUser ? "Distributor" : "Public";

                _logger.LogInformation(EventIds.FSSSearchAllYearWeekFromCacheStart.ToEventId(), "Maritime safety information request for searching attribute year and week data from cache azure table storage is started for _X-Correlation-ID:{correlationId}", correlationId);

                CustomTableEntity cacheInfo = await GetCacheTableData(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyAttributeTableName);

                if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry > DateTime.UtcNow)
                {
                    searchResult = JsonConvert.DeserializeObject<BatchAttributesSearchModel>(cacheInfo.Response);

                    _logger.LogInformation(EventIds.FSSSearchAllYearWeekFromCacheCompleted.ToEventId(), "Maritime safety information request for searching attribute year and week data from cache azure table storage is completed for _X-Correlation-ID:{correlationId}", correlationId);
                }
                else if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry <= DateTime.UtcNow)
                {
                    string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

                    _logger.LogInformation(EventIds.DeleteExpiredYearWeekCacheDataFromTableStarted.ToEventId(), "Deletion started for expired all year and week cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyAttributeTableName, correlationId);
                    await _azureTableStorageClient.DeleteEntityAsync(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyAttributeTableName, connectionString);
                    _logger.LogInformation(EventIds.DeleteExpiredYearWeekCacheDataFromTableCompleted.ToEventId(), "Deletion completed for expired all year and week cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyAttributeTableName, correlationId);
                }
                else
                {
                    _logger.LogInformation(EventIds.FSSSearchAllYearWeekDataNotFoundFromCache.ToEventId(), "Maritime safety information cache data not found for searching attribute year and week data from azure table storage for _X-Correlation-ID:{correlationId}", correlationId);
                }

                return searchResult;
            }
            catch(Exception ex)
            {
                _logger.LogError(EventIds.FSSSearchAllYearWeekFromCacheFailed.ToEventId(), "Failed to get searching attribute year and week data from cache azure table with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                
                return searchResult;
            }
        }

        public async Task<BatchSearchResponseModel> GetWeeklyBatchResponseFromCache(int year, int week, string correlationId)
        {
            BatchSearchResponseModel searchResult = new();
            try
            {
                string partitionKey = _userService.IsDistributorUser ? "Distributor" : "Public";
                string rowKey = year.ToString() + '|' + week.ToString();

                _logger.LogInformation(EventIds.FSSSearchWeeklyBatchResponseFromCacheStart.ToEventId(), "Maritime safety information request for searching weekly NM response from cache azure table storage is started for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);

                CustomTableEntity cacheInfo = await GetCacheTableData(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyBatchSearchTableName);

                if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry > DateTime.UtcNow)
                {
                    searchResult.batchSearchResponse = JsonConvert.DeserializeObject<BatchSearchResponse>(cacheInfo.Response);

                    _logger.LogInformation(EventIds.FSSSearchWeeklyBatchResponseFromCacheCompleted.ToEventId(), "Maritime safety information request for searching weekly NM response from cache azure table storage is completed for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);
                }
                else if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry <= DateTime.UtcNow)
                {
                    string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

                    _logger.LogInformation(EventIds.DeleteExpiredSearchWeeklyBatchResponseFromCacheStarted.ToEventId(), "Deletion started for expired searching weekly NM response cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyBatchSearchTableName, correlationId);
                    await _azureTableStorageClient.DeleteEntityAsync(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyBatchSearchTableName, connectionString);
                    _logger.LogInformation(EventIds.DeleteExpiredSearchWeeklyBatchResponseFromCacheCompleted.ToEventId(), "Deletion completed for expired searching weekly NM response cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyBatchSearchTableName, correlationId);
                }
                else
                {
                    _logger.LogInformation(EventIds.FSSSearchWeeklyBatchResponseDataNotFoundFromCache.ToEventId(), "Maritime safety information cache data not found for searching weekly NM response from azure table storage for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);
                }

                return searchResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSSearchWeeklyBatchResponseFromCacheFailed.ToEventId(), "Failed to get searching weekly NM response from cache azure table with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);

                return searchResult;
            }
        }

        public async Task InsertEntityAsync(object data, string rowKey, string tableName, string requestType, string correlationId)
        {
            try
            {
                string userRole = _userService.IsDistributorUser ? "Distributor" : "Public";

                CustomTableEntity customTableEntity = new()
                {
                    PartitionKey = userRole,
                    RowKey = rowKey,
                    Response = JsonConvert.SerializeObject(data),
                    CacheExpiry = DateTime.UtcNow.AddMinutes(_cacheConfiguration.Value.CacheTimeOutInMins)
                };

                string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

                await _azureTableStorageClient.InsertEntityAsync(customTableEntity, tableName, connectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSCacheDataInsertFailed.ToEventId(), "Process failed to insert entity value in cache table for request type:{requestType}, tableName:{tableName} with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", requestType, tableName, ex.Message, correlationId);
            }
        }

        private async Task<CustomTableEntity> GetCacheTableData(string partitionKey, string rowKey, string tableName)
        {
            string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

            return await _azureTableStorageClient.GetEntityAsync(partitionKey, rowKey, tableName, connectionString);
        }
    }
}
