using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareServiceCache
    {
        Task<BatchAttributesSearchModel> GetAllYearWeekFromCache(string partitionKey, string rowKey, string correlationId);

        Task<BatchSearchResponse> GetWeeklyBatchFilesFromCache(string partitionKey, int year, int week, string correlationId);

        Task InsertEntityAsync(CustomTableEntity customTableEntity, string tableName);
    }
}
