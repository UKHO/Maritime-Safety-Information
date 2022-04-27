using UKHO.MaritimeSafetyInformation.Common.Models.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository 
    {
        Task AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings);
    }
}
