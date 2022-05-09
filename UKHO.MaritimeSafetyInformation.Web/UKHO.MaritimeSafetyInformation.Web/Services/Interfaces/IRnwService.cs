using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRnwService
    {
        Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarnings radioNavigationalWarnings, string correlationId);

        Task<List<WarningType>> GetWarningTypes();
    }
}
