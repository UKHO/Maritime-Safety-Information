using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository
    {
        Task<List<RadioNavigationalWarningsAdminList>> GetRadioNavigationWarningsAdminList();        

        Task<List<WarningType>> GetWarningTypes();

        Task<List<string>> GetYears();

        RadioNavigationalWarningsAdminList EditRadioNavigation(int id);

        Task AddRadioNavigationWarning(RadioNavigationalWarningsAdminList radioNavigationalWarning);

        int GetWarningType(RadioNavigationalWarningsAdminList radioNavigationalWarning);
    }
}
