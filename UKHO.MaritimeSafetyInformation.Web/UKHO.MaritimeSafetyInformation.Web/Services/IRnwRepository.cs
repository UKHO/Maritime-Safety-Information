using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public interface IRnwRepository
    {
        Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdminAsync(int pageIndex, int warningType, string year, bool reLoadData, string correlationId);
    }
}
