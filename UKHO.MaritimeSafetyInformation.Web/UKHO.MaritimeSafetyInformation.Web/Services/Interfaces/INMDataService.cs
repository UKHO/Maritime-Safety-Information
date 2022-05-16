using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface INMDataService
    {
        Task<List<ShowFilesResponseModel>> GetWeeklyBatchFiles(int year, int week, string correlationId);
        Task<List<ShowDailyFilesResponseModel>> GetDailyBatchDetailsFiles(string correlationId);
        Task<List<YearWeekModel>> GetAllYearWeek(string correlationId);
     
        List<SelectListItem> GetAllYearsSelectItem(string correlationId);
       
        List<SelectListItem> GetAllWeeksOfYearSelectItem(int year, string correlationId);
        Task<ShowWeeklyFilesResponseModel> GetWeeklyFilesResponseModelsAsync(int year, int week, string correlationId);
    }
}
