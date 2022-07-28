using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        Task<ShowNMFilesResponseModel> GetWeeklyBatchFiles(int year, int week, string correlationId);
        Task<List<ShowDailyFilesResponseModel>> GetDailyBatchDetailsFiles(string correlationId);
        Task<ShowWeeklyFilesResponseModel> GetWeeklyFilesResponseModelsAsync(int year, int week, string correlationId);
        Task<byte[]> DownloadFssFileAsync(string batchId, string fileName, string correlationId, string frequency);
        Task<List<ShowFilesResponseModel>> GetLeisureFilesAsync(string correlationId);
        Task<byte[]> DownloadFSSZipFileAsync(string batchId, string fileName, string correlationId);
        Task<ShowNMFilesResponseModel> GetCumulativeBatchFiles(string correlationId);
        Task<List<ShowFilesResponseModel>> GetAnnualBatchFiles(string correlationId);
    }
}
