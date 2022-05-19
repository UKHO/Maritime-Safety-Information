using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IRNWRepository
    {
        Task AddRadioNavigationWarnings(RadioNavigationalWarnings radioNavigationalWarnings);

        Task<List<WarningType>> GetWarningTypes();
    }
}
