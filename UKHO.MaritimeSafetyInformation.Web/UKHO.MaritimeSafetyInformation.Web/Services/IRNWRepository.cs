using UKHO.MaritimeSafetyInformation.Web.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRNWRepository : IRepository<RadioNavigationalWarnings>
    {
        void AddRadioNavigation(RadioNavigationalWarnings model);

        RadioNavigationalWarnings EditRadioNavigation(int id);

        void UpdateRadioNavigation(RadioNavigationalWarnings model);

        void DeleteRadioNavigation(RadioNavigationalWarnings model);
    }
}
