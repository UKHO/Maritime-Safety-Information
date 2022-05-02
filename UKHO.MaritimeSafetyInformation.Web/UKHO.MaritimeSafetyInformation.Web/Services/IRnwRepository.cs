using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository
    {
        RadioNavigationalWarningsAdminListFilter GetRadioNavigationWarningsForAdmin(int pageIndex,int warningType, string year, string correlationId);
    }
}
