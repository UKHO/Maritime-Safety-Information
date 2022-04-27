using UKHO.MaritimeSafetyInformation.Common.Models.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository : IRepository<RadioNavigationalWarnings>
    {
        void AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings);
    }
}
