using Azure.Data.Tables;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureTableStorageClient : IAzureTableStorageClient
    {
        private static async Task<TableClient> GetTableClient(string tableName, string storageAccountConnectionString) 
        {
            var serviceClient = new TableServiceClient(storageAccountConnectionString);
            TableClient tableClient = serviceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        public async Task<FssWeeklyAttributeResponseCache> GetEntityAsync(string partitionKey, string rowKey, string tableName, string storageAccountConnectionString)
        {
            TableClient tableClient = await GetTableClient(tableName, storageAccountConnectionString);
            try
            {
                return await tableClient.GetEntityAsync<FssWeeklyAttributeResponseCache>(partitionKey, rowKey);
            }
            catch (Exception)
            {
                return new FssWeeklyAttributeResponseCache();
            }
        }

        public async Task DeleteEntityAsync(string category, string id, string tableName, string storageAccountConnectionString)
        {
            TableClient tableClient = await GetTableClient(tableName, storageAccountConnectionString);
            await tableClient.DeleteEntityAsync(category, id);
        }

        public async Task InsertEntityAsync(FssWeeklyAttributeResponseCache fssWeeklyAttributeResponseCache, string tableName, string storageAccountConnectionString)
        {
            TableClient tableClient = await GetTableClient(tableName, storageAccountConnectionString);
            await tableClient.AddEntityAsync(fssWeeklyAttributeResponseCache);
        }
    }
}
