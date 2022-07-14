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

        public FileShareServiceCache(IAzureTableStorageClient azureTableStorageClient, IOptions<CacheConfiguration> cacheConfiguration, IAzureStorageService azureStorageService, ILogger<FileShareServiceCache> logger)
        {
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
            _azureStorageService = azureStorageService;
            _logger = logger;
        }

        public async Task<BatchAttributesSearchModel> GetAllYearWeekFromCache(string partitionKey, string rowKey, string correlationId)
        {
            _logger.LogInformation(EventIds.FSSSearchAllYearWeekFromCacheStart.ToEventId(), "Maritime safety information request for searching attribute year and week data from cache azure table storage is started for _X-Correlation-ID:{correlationId}", correlationId);

            BatchAttributesSearchModel SearchResult =new();
            CustomTableEntity cacheInfo = await GetCacheTableData(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyAttributeTableName);

            if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry > DateTime.UtcNow)
            {
                SearchResult = JsonConvert.DeserializeObject<BatchAttributesSearchModel>(cacheInfo.Response);
                SearchResult.AttributeYearAndWeekIsCache = true;

                _logger.LogInformation(EventIds.FSSSearchAllYearWeekFromCacheCompleted.ToEventId(), "Maritime safety information request for searching attribute year and week data from cache azure table storage is completed for _X-Correlation-ID:{correlationId}", correlationId);
            }
            else if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry <= DateTime.UtcNow)
            {
                string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

                _logger.LogInformation(EventIds.DeleteExpiredYearWeekCacheDataFromTableStarted.ToEventId(), "Deletion started for expired all year and week cache data from table:{_cacheConfiguration.Value.FssWeeklyAttributeTableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyAttributeTableName, correlationId);
                await _azureTableStorageClient.DeleteEntityAsync(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyAttributeTableName, connectionString);
                _logger.LogInformation(EventIds.DeleteExpiredYearWeekCacheDataFromTableCompleted.ToEventId(), "Deletion completed for expired all year and week cache data from table:{_cacheConfiguration.Value.FssWeeklyAttributeTableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyAttributeTableName, correlationId);
            }
            else
            {
                _logger.LogInformation(EventIds.FSSSearchAllYearWeekDataNotFoundFromCache.ToEventId(), "Maritime safety information cache data not found for searching attribute year and week data from azure table storage for _X-Correlation-ID:{correlationId}", correlationId);
            }
            return SearchResult;
        }

        public async Task<BatchSearchResponseModel> GetWeeklyBatchFilesFromCache(string partitionKey, int year, int week, string correlationId)
        {
            _logger.LogInformation(EventIds.FSSSearchWeeklyBatchFilesFromCacheStart.ToEventId(), "Maritime safety information request for searching weekly NM files from cache azure table storage is started for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);

            BatchSearchResponseModel SearchResult = new();
            string rowKey = year.ToString() + '|' + week.ToString();
            CustomTableEntity cacheInfo = await GetCacheTableData(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyBatchSearchTableName);

            if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry > DateTime.UtcNow)
            {
                SearchResult.batchSearchResponse = JsonConvert.DeserializeObject<BatchSearchResponse>(cacheInfo.Response);
                SearchResult.WeeklyNMFilesIsCache = true;

                _logger.LogInformation(EventIds.FSSSearchWeeklyBatchFilesFromCacheCompleted.ToEventId(), "Maritime safety information request for searching weekly NM file from cache azure table storage is completed for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);
            }
            else if (!string.IsNullOrEmpty(cacheInfo.Response) && cacheInfo.CacheExpiry <= DateTime.UtcNow)
            {
                string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

                _logger.LogInformation(EventIds.DeleteExpiredSearchWeeklyBatchFilesFromCacheStarted.ToEventId(), "Deletion started for expired searching weekly NM file cache data from table:{_cacheConfiguration.Value.FssWeeklyBatchSearchTableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyBatchSearchTableName, correlationId);
                await _azureTableStorageClient.DeleteEntityAsync(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyBatchSearchTableName, connectionString);
                _logger.LogInformation(EventIds.DeleteExpiredSearchWeeklyBatchFilesFromCacheCompleted.ToEventId(), "Deletion completed for expired searching weekly NM file cache data from table:{_cacheConfiguration.Value.FssWeeklyBatchSearchTableName} for _X-Correlation-ID:{CorrelationId}", _cacheConfiguration.Value.FssWeeklyBatchSearchTableName, correlationId);
            }
            else 
            {
                _logger.LogInformation(EventIds.FSSSearchWeeklyBatchFilesDataNotFoundFromCache.ToEventId(), "Maritime safety information cache data not found for searching weekly NM file from azure table storage for year:{year} and week:{week} with _X-Correlation-ID:{correlationId}", year, week, correlationId);
            }

            return SearchResult;
        }

        public async Task InsertEntityAsync(CustomTableEntity customTableEntity, string tableName)
        {
            string connectionString =_azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey); 
            
            await _azureTableStorageClient.InsertEntityAsync(customTableEntity, tableName, connectionString);
        }

        private async Task<CustomTableEntity> GetCacheTableData(string partitionKey, string rowKey, string tableName)
        {
            string connectionString = _azureStorageService.GetStorageAccountConnectionString(_cacheConfiguration.Value.CacheStorageAccountName, _cacheConfiguration.Value.CacheStorageAccountKey);

            return await _azureTableStorageClient.GetEntityAsync(partitionKey, rowKey, tableName, connectionString);
        }
    }
}
