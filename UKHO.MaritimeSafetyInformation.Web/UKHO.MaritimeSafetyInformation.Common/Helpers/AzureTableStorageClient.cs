using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Data.Tables;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AzureTableStorageClient : IAzureTableStorageClient
    {
        private static async Task<TableClient> GetTableClient(string tableName, string storageAccountConnectionString)
        {
            var serviceClient = new TableServiceClient(storageAccountConnectionString);
            var tableItem = serviceClient.Query().FirstOrDefault(sc => sc.Name.Equals(tableName,StringComparison.InvariantCultureIgnoreCase));

            var tableClient = serviceClient.GetTableClient(tableName);
            if (tableItem is null)
            {
                await tableClient.CreateAsync();
            }
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

        public async Task DeleteTablesAsync(List<string> tableNames, string storageAccountConnectionString)
        {
            var tableServiceClient = new TableServiceClient(storageAccountConnectionString);

            foreach (var tableClient in tableNames.Select(x => tableServiceClient.GetTableClient(x)))
            {
                await tableClient.DeleteAsync();
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
                        orderby r.StartDate descending
                        select r).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
