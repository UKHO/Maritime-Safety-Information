using Azure;
using Azure.Data.Tables;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
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

        public async Task<CustomTableEntity> GetEntityAsync(string partitionKey, string rowKey, string tableName, string storageAccountConnectionString)
        {
            try
            {
                TableClient tableClient = await GetTableClient(tableName, storageAccountConnectionString);
                return await tableClient.GetEntityAsync<CustomTableEntity>(partitionKey, rowKey);
            }
            catch (Exception)
            {
                return new CustomTableEntity();
            }
        }

        public async Task DeleteEntityAsync(string category, string id, string tableName, string storageAccountConnectionString)
        {
            TableClient tableClient = await GetTableClient(tableName, storageAccountConnectionString);
            await tableClient.DeleteEntityAsync(category, id);
        }

        public async Task InsertEntityAsync(CustomTableEntity customTableEntity, string tableName, string storageAccountConnectionString)
        {
            TableClient tableClient = await GetTableClient(tableName, storageAccountConnectionString);
            await tableClient.AddEntityAsync(customTableEntity);
        }

        public async Task DeleteTablesAsync( List<string> tableNames, string storageAccountConnectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            foreach (var tableName in tableNames)
            {
                CloudTable _cloudTable = tableClient.GetTableReference(tableName);
                await _cloudTable.DeleteIfExistsAsync();
            }
        }

        public async Task<MsiBannerNotificationEntity> GetSingleEntityAsync(string tableName, string storageAccountConnectionString)
        {
            try
            {
                List<MsiBannerNotificationEntity> msiBannerNotificationEntityList = new();
                TableClient tableClient = await GetTableClient(tableName, storageAccountConnectionString);
                AsyncPageable<MsiBannerNotificationEntity> linqEntities = tableClient.QueryAsync<MsiBannerNotificationEntity>(r => r.StartDate <= DateTime.UtcNow && r.ExpiryDate > DateTime.UtcNow && r.IsNotificationEnabled);

                await foreach (MsiBannerNotificationEntity item in linqEntities)
                {
                    msiBannerNotificationEntityList.Add(item);
                }

                return (from r in msiBannerNotificationEntityList
                        orderby r.StartDate ascending
                        select r).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
