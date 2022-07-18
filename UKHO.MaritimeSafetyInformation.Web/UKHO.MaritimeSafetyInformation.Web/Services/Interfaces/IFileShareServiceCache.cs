using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareServiceCache
    {
        Task<BatchAttributesSearchModel> GetAllYearWeekFromCache(string rowKey, string correlationId);

        Task<BatchSearchResponseModel> GetWeeklyBatchResponseFromCache(int year, int week, string correlationId);

        Task InsertEntityAsync(object data, string rowKey, string tableName, string requestType, string correlationId);

        Task<BatchSearchResponseModel> GetCumulativeBatchFilesFromCache(string correlationId);
    }
}
