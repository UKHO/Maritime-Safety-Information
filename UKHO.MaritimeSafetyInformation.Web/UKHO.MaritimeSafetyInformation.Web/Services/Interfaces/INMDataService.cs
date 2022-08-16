using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        Task<ShowNMFilesResponseModel> GetWeeklyBatchFiles(int year, int week, string correlationId);
        Task<ShowDailyFilesResponseListModel> GetDailyBatchDetailsFiles(string correlationId);
        Task<ShowWeeklyFilesResponseModel> GetWeeklyFilesResponseModelsAsync(int year, int week, string correlationId);
        Task<byte[]> DownloadFssFileAsync(string batchId, string fileName, string correlationId, string frequency);
        Task<ShowNMFilesResponseModel> GetLeisureFilesAsync(string correlationId);
        Task<byte[]> DownloadFSSZipFileAsync(string batchId, string fileName, bool isDistributorUser, string correlationId);
        //Task<byte[]> DownloadFSSAllZipFileAsync(string batchId, string fileName, bool isDistributorUser, string correlationId);
        Task<ShowNMFilesResponseModel> GetCumulativeBatchFiles(string correlationId);
        Task<ShowNMFilesResponseModel> GetAnnualBatchFiles(string correlationId);
    }
}
