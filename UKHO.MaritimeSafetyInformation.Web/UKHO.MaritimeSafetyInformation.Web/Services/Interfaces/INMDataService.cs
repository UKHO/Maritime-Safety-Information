using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        Task<List<ShowFilesResponseModel>> GetWeeklyBatchFiles(int year, int week, string correlationId);
        Task<List<ShowDailyFilesResponseModel>> GetDailyBatchDetailsFiles(string correlationId);
        Task<ShowWeeklyFilesResponseModel> GetWeeklyFilesResponseModelsAsync(int year, int week, string correlationId);
        Task<byte[]> DownloadFssFileAsync(string batchId, string fileName, string correlationId);
    }
}
