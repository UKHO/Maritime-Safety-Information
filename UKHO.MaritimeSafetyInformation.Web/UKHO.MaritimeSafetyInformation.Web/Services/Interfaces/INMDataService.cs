using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        public Task<List<ShowFilesResponseModel>> GetBatchDetailsFiles(int year, int week);
        public List<KeyValuePair<string, string>> GetPastYears();
        public List<KeyValuePair<string, string>> GetAllWeeksofYear(int year);
    }
}
