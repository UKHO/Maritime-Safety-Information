using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwService : IRnwService
    {
        private readonly IRnwRepository _rnwRepository;
        private readonly IOptions<RadioNavigationalWarningConfiguration> _radioNavigationalWarningConfiguration;
        private readonly ILogger<RnwService> _logger;
        private static List<RadioNavigationalWarningsAdminList> s_allRadioNavigationalWarningsAdminList;

        public RnwService(IRnwRepository repository,
                        IOptions<RadioNavigationalWarningConfiguration> radioNavigationalWarningConfiguration,
                        ILogger<RnwService> logger)
        {
            _rnwRepository = repository;
            _radioNavigationalWarningConfiguration = radioNavigationalWarningConfiguration;
            _logger = logger;
        }

        public async Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarnings radioNavigationalWarnings, string correlationId)
        public async Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int warningType, string year, bool reLoadData, string correlationId)
        {
            try
            {
                if (radioNavigationalWarnings.WarningType != 0 && radioNavigationalWarnings.Reference != "" && radioNavigationalWarnings.DateTimeGroup.ToString() != null
                           && radioNavigationalWarnings.Summary != null && radioNavigationalWarnings.Content != null)
                RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminListFilter = new();

                int rnwAdminListRecordPerPage = _radioNavigationalWarningConfiguration.Value.AdminListRecordPerPage;
                int srNo = (pageIndex - 1) * rnwAdminListRecordPerPage;

                if (s_allRadioNavigationalWarningsAdminList == null || reLoadData)
                {
                    _logger.LogInformation(EventIds.MSIAddNewRNWRecordStart.ToEventId(), "Maritime safety information add new RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                    await _rnwRepository.AddRadioNavigationWarnings(radioNavigationalWarnings);
                    _logger.LogInformation(EventIds.MSIAddNewRNWRecordCompleted.ToEventId(), "Maritime safety information add new RNW record to database request completed for _X-Correlation-ID:{correlationId}", correlationId);
                    s_allRadioNavigationalWarningsAdminList = await _rnwRepository.GetRadioNavigationWarningsAdminList(correlationId);
                }

                    return true;
                List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminList = s_allRadioNavigationalWarningsAdminList;

                if (warningType != 0)
                {
                    radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.WarningType == warningType).ToList();
                }
                else

                if (!string.IsNullOrEmpty(year))
                {
                    _logger.LogInformation(EventIds.MSIInvalidNewRNWRecordRequest.ToEventId(), "Maritime safety information invalid new RNW record request for _X-Correlation-ID:{correlationId}", correlationId);
                    radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.DateTimeGroup.Year.ToString().Trim() == year).ToList();
                }

                double pageCount = (double)(radioNavigationalWarningsAdminList.Count / Convert.ToDecimal(rnwAdminListRecordPerPage));
                radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Skip(srNo).Take(rnwAdminListRecordPerPage).ToList();

                    return false;
                }
                radioNavigationalWarningsAdminListFilter.RadioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList;
                radioNavigationalWarningsAdminListFilter.PageCount = (int)Math.Ceiling(pageCount);
                radioNavigationalWarningsAdminListFilter.CurrentPageIndex = pageIndex;
                radioNavigationalWarningsAdminListFilter.WarningTypes = await _rnwRepository.GetWarningTypes();
                radioNavigationalWarningsAdminListFilter.Years = await _rnwRepository.GetYears();
                radioNavigationalWarningsAdminListFilter.WarningType = warningType;
                radioNavigationalWarningsAdminListFilter.Year = year;
                radioNavigationalWarningsAdminListFilter.SrNo = srNo;
                return radioNavigationalWarningsAdminListFilter;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.MSIAddNewRNWRequestError.ToEventId(), ex, "Maritime safety information add new RNW record to database request failed with error with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                return false;
                _logger.LogError(EventIds.MSIGetRnwForAdminRequestError.ToEventId(), ex, "Maritime safety information request failed to get RNW records for Admin from database with exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                return new RadioNavigationalWarningsAdminListFilter();
            }
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
           return await _rnwRepository.GetWarningTypes();
        }
    }
}
