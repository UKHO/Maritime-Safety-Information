using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository
    {
        RadioNavigationalWarningsAdminListFilter GetRadioNavigationForAdmin(int pageIndex,int warningType, string year);
    }
}
