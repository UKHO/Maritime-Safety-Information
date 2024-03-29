﻿using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IFileShareServiceCache
    {
        Task<BatchAttributesSearchModel> GetAllYearsAndWeeksFromCache(string rowKey, string partitionKey, string correlationId);
        Task<BatchSearchResponseModel> GetWeeklyBatchResponseFromCache(int year, int week, string partitionKey, string correlationId);       
        Task<BatchSearchResponseModel> GetBatchResponseFromCache(string partitionKey, string rowKey, string frequency, string correlationId);
        Task InsertCacheObject(object data, string rowKey, string tableName, string requestType, string partitionKey, string correlationId);
    }
}
