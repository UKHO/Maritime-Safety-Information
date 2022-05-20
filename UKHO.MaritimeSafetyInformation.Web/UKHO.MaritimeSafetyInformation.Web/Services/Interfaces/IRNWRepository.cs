using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRNWRepository
    {
        Task AddRadioNavigationWarning(RadioNavigationalWarning radioNavigationalWarning);

        Task<List<WarningType>> GetWarningTypes();

        Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsDataList();

        Task<List<RadioNavigationalWarningsAdminList>> GetRadioNavigationWarningsAdminList();

        Task<List<string>> GetYears();
    }
}
