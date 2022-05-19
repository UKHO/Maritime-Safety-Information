using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRNWService
    {
        Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarning radioNavigationalWarning, string correlationId);
        Task<List<WarningType>> GetWarningTypes();
        Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int? warningType, int? year, string correlationId);
    }
}
