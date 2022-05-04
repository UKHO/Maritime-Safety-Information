using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : IRnwRepository
    {
        private readonly RadioNavigationalWarningsContext _context;
        private readonly IOptions<RadioNavigationalWarningConfiguration> _radioNavigationalWarningConfiguration;
        private readonly ILogger<RnwRepository> _logger;
        private static List<RadioNavigationalWarningsAdminList> s_allRadioNavigationalWarningsAdminList;

        public RnwRepository(RadioNavigationalWarningsContext context,
                            IOptions<RadioNavigationalWarningConfiguration> radioNavigationalWarningConfiguration,
                            ILogger<RnwRepository> logger)
        {
            _context = context;
            _radioNavigationalWarningConfiguration = radioNavigationalWarningConfiguration;
            _logger = logger;
        }

        public async Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int warningTypeId, string year, bool reLoadData, string correlationId)
        {
            try
            {
                RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminListFilter = new();
                List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminList = new();

                int rnwAdminListRecordPerPage = _radioNavigationalWarningConfiguration.Value.AdminListRecordPerPage;
                int srNo = (pageIndex - 1) * rnwAdminListRecordPerPage;

                radioNavigationalWarningsAdminList = GetRadioNavigationWarningsAdminList(reLoadData);

                if (warningTypeId != 0)
                {
                    radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.WarningType == warningTypeId).ToList();
                }

                if (!string.IsNullOrEmpty(year))
                {
                    radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.DateTimeGroup.Year.ToString().Trim() == year).ToList();
                }

                double pageCount = (double)(radioNavigationalWarningsAdminList.Count / Convert.ToDecimal(rnwAdminListRecordPerPage));
                radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Skip(srNo).Take(rnwAdminListRecordPerPage).ToList();

                radioNavigationalWarningsAdminListFilter.RadioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList;
                radioNavigationalWarningsAdminListFilter.PageCount = (int)Math.Ceiling(pageCount);
                radioNavigationalWarningsAdminListFilter.CurrentPageIndex = pageIndex;
                radioNavigationalWarningsAdminListFilter.WarningTypes = _context.WarningType.ToList();
                radioNavigationalWarningsAdminListFilter.Years = (from p in _context.RadioNavigationalWarnings
                                                                  select p.DateTimeGroup.Year.ToString()).Distinct().ToList();
                radioNavigationalWarningsAdminListFilter.WarningType = warningTypeId;
                radioNavigationalWarningsAdminListFilter.Year = year;
                radioNavigationalWarningsAdminListFilter.SrNo = srNo;
                return radioNavigationalWarningsAdminListFilter;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.MSIGetRnwForAdminRequestError.ToEventId(), ex, "Maritime safety information get RNW record from database request failed with error with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                return new RadioNavigationalWarningsAdminListFilter();
            }
        }

        private List<RadioNavigationalWarningsAdminList> GetRadioNavigationWarningsAdminList(bool reLoadData = true)
        {
            if (s_allRadioNavigationalWarningsAdminList == null || reLoadData)
            {
                s_allRadioNavigationalWarningsAdminList = (from rnwWarnings in _context.RadioNavigationalWarnings
                                                           join warning in _context.WarningType on rnwWarnings.WarningType equals warning.Id
                                                           select new RadioNavigationalWarningsAdminList
                                                           {
                                                               Id = rnwWarnings.Id,
                                                               WarningType = rnwWarnings.WarningType,
                                                               Reference = rnwWarnings.Reference,
                                                               DateTimeGroup = rnwWarnings.DateTimeGroup,
                                                               DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup),
                                                               Summary = rnwWarnings.Summary,
                                                               Content = rnwWarnings.Content,
                                                               ExpiryDate = rnwWarnings.ExpiryDate,
                                                               ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate),
                                                               IsDeleted = rnwWarnings.IsDeleted == true ? "Yes" : "No",
                                                               WarningTypeName = warning.Name

                                                           })
                                                         .OrderByDescending(a => a.DateTimeGroup).ToList();
            }
            return s_allRadioNavigationalWarningsAdminList;
        }
    }
}
