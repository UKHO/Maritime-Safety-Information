using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareServiceCache
    {
        Task<BatchAttributesSearchModel> GetAllYearsAndWeeksFromCache(string rowKey, string correlationId);

        Task<BatchSearchResponseModel> GetWeeklyBatchResponseFromCache(int year, int week, string correlationId);

        Task InsertCacheObject(object data, string rowKey, string tableName, string requestType, string correlationId, string partitionKey);

        Task<BatchSearchResponseModel> GetCumulativeBatchFilesFromCache(string correlationId);
    }
}
