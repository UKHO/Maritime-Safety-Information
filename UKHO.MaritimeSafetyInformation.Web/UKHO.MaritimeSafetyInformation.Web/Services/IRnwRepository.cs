using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository
    {
        void AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings);
        RadioNavigationalWarningsAdminListFilter GetRadioNavigationForAdmin(int pageIndex = 1);
    }
}
