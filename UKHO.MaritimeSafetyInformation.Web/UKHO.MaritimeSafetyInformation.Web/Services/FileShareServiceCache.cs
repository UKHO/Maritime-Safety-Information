using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
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

        public FileShareServiceCache(IAzureTableStorageClient azureTableStorageClient, IOptions<CacheConfiguration> cacheConfiguration, IAzureStorageService azureStorageService)
        {
            _azureTableStorageClient = azureTableStorageClient;
            _cacheConfiguration = cacheConfiguration;
            _azureStorageService = azureStorageService;
        }

        public async Task<BatchAttributesSearchModel> GetAllYearWeekFromCache(string partitionKey, string rowKey, string correlationId)
        {
            BatchAttributesSearchModel SearchResult =new();
            CustomTableEntity cacheInfo = await GetCacheTableData(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyAttributeTableName);

            if (!string.IsNullOrEmpty(cacheInfo.Response))
            {
                SearchResult = JsonConvert.DeserializeObject<BatchAttributesSearchModel>(cacheInfo.Response);
            }

            return SearchResult;
        }

        public async Task<BatchSearchResponse> GetWeeklyBatchFilesFromCache(string partitionKey, int year, int week, string correlationId)
        {
            BatchSearchResponse SearchResult = new();
            string rowKey = year.ToString() + '|' + week.ToString();
            CustomTableEntity cacheInfo = await GetCacheTableData(partitionKey, rowKey, _cacheConfiguration.Value.FssWeeklyBatchSearchTableName);

            if (!string.IsNullOrEmpty(cacheInfo.Response))
            {
                SearchResult = JsonConvert.DeserializeObject<BatchSearchResponse>(cacheInfo.Response);
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
