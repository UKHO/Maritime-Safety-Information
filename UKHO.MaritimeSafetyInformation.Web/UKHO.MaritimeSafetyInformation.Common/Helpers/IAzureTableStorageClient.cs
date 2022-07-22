using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public interface IAzureTableStorageClient
    {
        Task<CustomTableEntity> GetEntityAsync(string partitionKey, string rowKey, string tableName, string storageAccountConnectionString);
        Task DeleteEntityAsync(string category, string id, string tableName, string storageAccountConnectionString);
        Task InsertEntityAsync(CustomTableEntity customTableEntity, string tableName, string storageAccountConnectionString);
        Task DeleteTablesAsync(string tableName, string storageAccountConnectionString);
    }
}
