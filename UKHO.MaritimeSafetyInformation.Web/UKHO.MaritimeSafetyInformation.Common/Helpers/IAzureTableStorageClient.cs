using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public interface IAzureTableStorageClient
    {
        Task<FssWeeklyAttributeResponseCache> GetEntityAsync(string partitionKey, string rowKey, string tableName, string storageAccountConnectionString);
        Task DeleteEntityAsync(string category, string id, string tableName, string storageAccountConnectionString);
        Task InsertEntityAsync(FssWeeklyAttributeResponseCache fssWeeklyAttributeResponseCache, string tableName, string storageAccountConnectionString);
    }
}
