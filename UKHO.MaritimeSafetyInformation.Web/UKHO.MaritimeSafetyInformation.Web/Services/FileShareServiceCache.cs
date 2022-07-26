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
        private string PartitionKey => _userService.IsDistributorUser ? "Distributor" : "Public";
        private string ConnectionString => _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

        public FileShareServiceCache(IAzureTableStorageClient azureTableStorageClient, IOptions<CacheConfiguration> cacheConfiguration, IAzureStorageService azureStorageService, ILogger<FileShareServiceCache> logger, IUserService userService)
        {
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
            _azureStorageService = azureStorageService;
            _logger = logger;
            _userService = userService;
        }

        public async Task<BatchAttributesSearchModel> GetAllYearsAndWeeksFromCache(string rowKey, string correlationId)
        {
            BatchAttributesSearchModel searchResult = new();
            try
            {
                _logger.LogInformation(EventIds.FSSSearchAllYearWeekFromCacheStart.ToEventId(), "Maritime safety information request for searching attribute year and week data from cache azure table storage is started for _X-Correlation-ID:{correlationId}", correlationId);

                CustomTableEntity cacheInfo = await GetCacheTableData(PartitionKey, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName);

                if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry > DateTime.UtcNow)
                {
                    searchResult = JsonConvert.DeserializeObject<BatchAttributesSearchModel>(cacheInfo.Response);

                    _logger.LogInformation(EventIds.FSSSearchAllYearWeekFromCacheCompleted.ToEventId(), "Maritime safety information request for searching attribute year and week data from cache azure table storage is completed for _X-Correlation-ID:{correlationId}", correlationId);
                }
                else if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry <= DateTime.UtcNow)
                {
                    _logger.LogInformation(EventIds.DeleteExpiredYearWeekCacheDataFromTableStarted.ToEventId(), "Deletion started for expired all year and week cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssCacheResponseTableName, correlationId);
                    await _azureTableStorageClient.DeleteEntityAsync(PartitionKey, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName, ConnectionString);
                    _logger.LogInformation(EventIds.DeleteExpiredYearWeekCacheDataFromTableCompleted.ToEventId(), "Deletion completed for expired all year and week cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssCacheResponseTableName, correlationId);
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

        public async Task InsertCacheObject(object data, string rowKey, string tableName, string requestType, string correlationId, string partitionKey)
        {
            try
            {
                CustomTableEntity customTableEntity = new()
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    Response = JsonConvert.SerializeObject(data),
                    CacheExpiry = DateTime.UtcNow.AddMinutes(_cacheConfiguration.Value.CacheTimeOutInMins)
                };

                await _azureTableStorageClient.InsertEntityAsync(customTableEntity, tableName, ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSCacheDataInsertFailed.ToEventId(), "Process failed to insert entity value in cache table for request type:{requestType}, tableName:{tableName} with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", requestType, tableName, ex.Message, correlationId);
            }
        }

        public async Task<BatchSearchResponseModel> GetBatchResponseFromCache(string partitionKey, string rowKey, string frequency, string correlationId)
        {
            BatchSearchResponseModel searchResult = new();
            try
            {
                _logger.LogInformation(EventIds.FSSSearchBatchResponseFromCacheStart.ToEventId(), "Maritime safety information request for searching {frequency} NM response from cache azure table storage is started with _X-Correlation-ID:{correlationId}", frequency, correlationId);

                CustomTableEntity cacheInfo = await GetCacheTableData(partitionKey, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName);

                if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry > DateTime.UtcNow)
                {
                    searchResult.BatchSearchResponse = JsonConvert.DeserializeObject<BatchSearchResponse>(cacheInfo.Response);

                    _logger.LogInformation(EventIds.FSSSearchBatchResponseFromCacheCompleted.ToEventId(), "Maritime safety information request for searching {frequency} NM response from cache azure table storage is completed with _X-Correlation-ID:{correlationId}", frequency, correlationId);
                }
                else if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry <= DateTime.UtcNow)
                {
                    _logger.LogInformation(EventIds.DeleteExpiredSearchBatchResponseFromCacheStarted.ToEventId(), "Deletion started for expired searching {frequency} NM response cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", frequency, _cacheConfiguration.Value.FssCacheResponseTableName, correlationId);
                    await _azureTableStorageClient.DeleteEntityAsync(partitionKey, rowKey, _cacheConfiguration.Value.FssCacheResponseTableName, ConnectionString);
                    _logger.LogInformation(EventIds.DeleteExpiredSearchBatchResponseFromCacheCompleted.ToEventId(), "Deletion completed for expired searching {frequency} NM response cache data from table:{TableName} for _X-Correlation-ID:{CorrelationId}", frequency, _cacheConfiguration.Value.FssCacheResponseTableName, correlationId);
                }
                else
                {
                    _logger.LogInformation(EventIds.FSSSearchBatchResponseDataNotFoundFromCache.ToEventId(), "Maritime safety information cache data not found for searching {frequency} NM response from azure table storage with _X-Correlation-ID:{correlationId}", frequency, correlationId);
                }

                return searchResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSSearchCumulativeBatchResponseFromCacheFailed.ToEventId(), "Failed to get searching cumulative NM response from cache azure table with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);

                return searchResult;
            }
        }

        private async Task<CustomTableEntity> GetCacheTableData(string partitionKey, string rowKey, string tableName)
        {
            return await _azureTableStorageClient.GetEntityAsync(partitionKey, rowKey, tableName, ConnectionString);
        }
    }
}
