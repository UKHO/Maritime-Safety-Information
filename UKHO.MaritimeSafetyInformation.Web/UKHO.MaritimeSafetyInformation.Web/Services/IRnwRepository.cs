using UKHO.MaritimeSafetyInformation.Common.Models.DTO;
using UKHO.MaritimeSafetyInformation.Common.Models.RNW;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository
    {
        void AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings);
        RadioNavigationalWarningsAdminList GetRadioNavigationForAdmin(int pageIndex = 1);
    }
}
