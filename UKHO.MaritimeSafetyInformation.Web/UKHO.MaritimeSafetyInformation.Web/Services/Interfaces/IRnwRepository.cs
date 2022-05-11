using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository
    {
        Task<List<RadioNavigationalWarningsAdminList>> GetRadioNavigationWarningsAdminList(string correlationId);        

        Task<List<WarningType>> GetWarningTypes();

        Task<List<string>> GetYears();

        Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsDataList(string correlationId);
    }
}
