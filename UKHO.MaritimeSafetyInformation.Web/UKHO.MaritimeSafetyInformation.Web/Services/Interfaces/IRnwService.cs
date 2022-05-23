using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRnwService
    {
        Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int? warningType, int? year, string correlationId);

        RadioNavigationalWarningsAdminList EditRadioNavigationWarningListForAdmin(int id, string correlationId);

        Task<bool> EditRadioNavigationWarningsRecord(RadioNavigationalWarningsAdminList radioNavigationalWarning, string correlationId);
    }
}
