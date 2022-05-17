using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;
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

        public async Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int? warningType, int? year, bool reLoadData, string correlationId)
        {
            try
            {
                RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminListFilter = new();

                int rnwAdminListRecordPerPage = _radioNavigationalWarningConfiguration.Value.AdminListRecordPerPage;
                int srNo = (pageIndex - 1) * rnwAdminListRecordPerPage;

                if (s_allRadioNavigationalWarningsAdminList == null || reLoadData)
                {
                    s_allRadioNavigationalWarningsAdminList = await _rnwRepository.GetRadioNavigationWarningsAdminList();
                }

                List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminList = s_allRadioNavigationalWarningsAdminList;

                if (warningType != null)
                {
                    radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.WarningType == warningType).ToList();
                }

                if (year != null)
                {
                    radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.DateTimeGroup.Year.ToString().Trim() == year.ToString()).ToList();
                }

                double pageCount = (double)(radioNavigationalWarningsAdminList.Count / Convert.ToDecimal(rnwAdminListRecordPerPage));
                radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Skip(srNo).Take(rnwAdminListRecordPerPage).ToList();

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
                _logger.LogError(EventIds.MSIGetRnwForAdminRequestError.ToEventId(), ex, "Maritime safety information request failed to get RNW records for Admin from database with exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsData(string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.MSIGetRnwDetailFormDatabaseStarted.ToEventId(), "Maritime safety information request to get RNW detail from database started for _X-Correlation-ID:{correlationId}", correlationId);

                List<RadioNavigationalWarningsData> radioNavigationalWarningsData = await _rnwRepository.GetRadioNavigationalWarningsDataList();

                _logger.LogInformation(EventIds.MSIGetRnwDetailFormDatabaseCompleted.ToEventId(), "Maritime safety information request to get RNW detail from database completed for _X-Correlation-ID:{correlationId}", correlationId);

                return radioNavigationalWarningsData;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.MSIErrorInGetRnwDetailFromDatabase.ToEventId(), ex, "Maritime safety information error has occurred in the process to get RNW detail from database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }
    }
}
