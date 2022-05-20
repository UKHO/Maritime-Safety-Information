using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRNWRepository
    {
        Task<List<RadioNavigationalWarningsAdminList>> GetRadioNavigationWarningsAdminList();        

        Task<List<WarningType>> GetWarningTypes();

        Task<List<string>> GetYears();

        Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsDataList();
    }
}
