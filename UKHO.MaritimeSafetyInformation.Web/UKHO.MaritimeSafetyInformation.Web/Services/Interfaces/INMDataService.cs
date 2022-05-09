using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        Task<List<ShowFilesResponseModel>> GetWeeklyBatchFiles(int year, int week, string correlationId);
        Task<List<YearWeekModel>> GetAllYearWeek(string correlationId);
    }
}
