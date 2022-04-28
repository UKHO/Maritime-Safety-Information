using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository 
    {
        Task AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings);

        List<WarningType> GetWarningType();
    }
}
