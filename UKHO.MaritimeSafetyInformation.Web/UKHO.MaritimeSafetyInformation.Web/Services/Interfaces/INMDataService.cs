using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        Task<List<ShowFilesResponseModel>> GetWeeklyBatchFiles(int year, int week, string correlationId);
        Task<List<ShowDailyFilesResponseModel>> GetDailyBatchDetailsFiles(string correlationId);
        List<KeyValuePair<string, string>> GetAllYears(string correlationId);
        List<KeyValuePair<string, string>> GetAllWeeksofYear(int year, string correlationId);
    }
}
