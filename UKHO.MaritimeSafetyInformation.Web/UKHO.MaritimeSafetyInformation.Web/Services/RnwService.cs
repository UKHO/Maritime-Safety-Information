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

        public RnwService(IRnwRepository repository,
                        IOptions<RadioNavigationalWarningConfiguration> radioNavigationalWarningConfiguration,
                        ILogger<RnwService> logger)
        {
            _rnwRepository = repository;
            _radioNavigationalWarningConfiguration = radioNavigationalWarningConfiguration;
            _logger = logger;
        }

        public async Task<RadioNavigationalWarningsAdminListFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int? warningType, int? year, string correlationId)
        {
            try
            {
                RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminListFilter = new();

                int rnwAdminListRecordPerPage = _radioNavigationalWarningConfiguration.Value.AdminListRecordPerPage;
                int srNo = (pageIndex - 1) * rnwAdminListRecordPerPage;

                List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminList = await _rnwRepository.GetRadioNavigationWarningsAdminList();

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
                _logger.LogError(EventIds.MSIRnwAdminListError.ToEventId(), ex, "Maritime safety information request failed to get RNW records for Admin from database with exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }

        #region Edit Radio Navigational Warning
        public RadioNavigationalWarningsAdminList EditRadioNavigationWarningListForAdmin(int id, string correlationId)
        {
            try
            {
                RadioNavigationalWarningsAdminList radioNavigationalWarningsAdminList = _rnwRepository.EditRadioNavigation(id);
                return radioNavigationalWarningsAdminList;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ErrorInRetrievingRNWRecord.ToEventId(), ex, "Maritime safety information error has occurred while retrieving a RNW record from database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
            
        }

        public async Task<bool> EditRadioNavigationWarningsRecord(RadioNavigationalWarningsAdminList radioNavigationalWarning, string correlationId)
        {
            int warningType = _rnwRepository.GetWarningType(radioNavigationalWarning);
            if (warningType != WarningTypes.UK_Coastal && warningType != WarningTypes.NAVAREA_1)
            {
                await Task.CompletedTask;
                _logger.LogInformation(EventIds.InvalidWarningTypeInRequest.ToEventId(), "Maritime safety information invalid value received for parameter warningType for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new InvalidDataException("Invalid value received for parameter warningType");
            }

            if (string.IsNullOrEmpty(radioNavigationalWarning.Reference))
            {
                _logger.LogInformation(EventIds.InvalidReferenceInRequest.ToEventId(), "Maritime safety information invalid value received for parameter reference for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new ArgumentNullException("Invalid value received for parameter reference", new Exception());
            }

            if (string.IsNullOrEmpty(radioNavigationalWarning.Summary))
            {
                _logger.LogInformation(EventIds.InvalidSummaryInRequest.ToEventId(), "Maritime safety information invalid value received for parameter summary for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new ArgumentNullException("Invalid value received for parameter summary", new Exception());
            }

            if (string.IsNullOrEmpty(radioNavigationalWarning.Content))
            {
                _logger.LogInformation(EventIds.InvalidContentInRequest.ToEventId(), "Maritime safety information invalid value received for parameter content for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new ArgumentNullException("Invalid value received for parameter content", new Exception());
            }

            try
            {
                _logger.LogInformation(EventIds.EditRNWRecordStart.ToEventId(), "Maritime safety information add new RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                await _rnwRepository.AddRadioNavigationWarning(radioNavigationalWarning);
                _logger.LogInformation(EventIds.EditRNWRecordCompleted.ToEventId(), "Maritime safety information add new RNW record to database request completed for _X-Correlation-ID:{correlationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ErrorInRnwRepositoryProcess.ToEventId(), ex, "Maritime safety information error has occurred in the process to add new RNW record to database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }

            return true;
        }

        #endregion Edit Radio Navigational Warning
    }
}
