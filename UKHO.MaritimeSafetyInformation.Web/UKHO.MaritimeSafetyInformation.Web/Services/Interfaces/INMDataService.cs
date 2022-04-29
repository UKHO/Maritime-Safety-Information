using UKHO.MaritimeSafetyInformation.Common.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        public Task<List<ShowFilesResponseModel>> GetNMBatchFiles(int year, int week, string correlationId);
        public List<KeyValuePair<string, string>> GetPastYears(string correlationId);
        public List<KeyValuePair<string, string>> GetAllWeeksofYear(int year, string correlationId);
        public Task<List<ShowDailyFilesResponseModel>> GetDailyBatchDetailsFiles(string CurrentCorrelationId);
    }
}
