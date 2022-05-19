using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRNWService
    {
        Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarning radioNavigationalWarning, string correlationId);

        Task<List<WarningType>> GetWarningTypes();
    }
}
