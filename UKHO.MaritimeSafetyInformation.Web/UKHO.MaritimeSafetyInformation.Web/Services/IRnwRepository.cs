using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository 
    {
        Task<bool> AddRadioNavigationWarnings(RadioNavigationalWarnings radioNavigationalWarnings, string correlationId);

        List<WarningType> GetWarningType();
    }
}
