using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RNWService : IRNWService
    {
        private readonly IRNWRepository _rnwRepository;
        private readonly IOptions<RadioNavigationalWarningConfiguration> _radioNavigationalWarningConfiguration;
        private readonly ILogger<RNWService> _logger;

        public RNWService(IRNWRepository repository,
                        IOptions<RadioNavigationalWarningConfiguration> radioNavigationalWarningConfiguration,
                        ILogger<RNWService> logger)
        {
            _rnwRepository = repository;
            _radioNavigationalWarningConfiguration = radioNavigationalWarningConfiguration;
            _logger = logger;
        }

        public async Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarning radioNavigationalWarning, string correlationId)
        {
            if (radioNavigationalWarning.WarningType != WarningTypes.UK_Coastal && radioNavigationalWarning.WarningType != WarningTypes.NAVAREA_1)
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
                _logger.LogInformation(EventIds.AddNewRNWRecordStart.ToEventId(), "Maritime safety information add new RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                await _rnwRepository.AddRadioNavigationWarning(radioNavigationalWarning);
                _logger.LogInformation(EventIds.AddNewRNWRecordCompleted.ToEventId(), "Maritime safety information add new RNW record to database request completed for _X-Correlation-ID:{correlationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ErrorInRnwRepositoryProcess.ToEventId(), ex, "Maritime safety information error has occurred in the process to add new RNW record to database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }

            return true;
        }

        public async Task<RadioNavigationalWarningsAdminFilter> GetRadioNavigationWarningsForAdmin(int pageIndex, int? warningType, int? year, string correlationId)
        {
            try
            {
                int rnwAdminListRecordPerPage = _radioNavigationalWarningConfiguration.Value.AdminListRecordPerPage;
                int srNo = (pageIndex - 1) * rnwAdminListRecordPerPage;

                List<RadioNavigationalWarningsAdmin> radioNavigationalWarningsAdminList = await _rnwRepository.GetRadioNavigationWarningsAdminList();

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

                RadioNavigationalWarningsAdminFilter radioNavigationalWarningsAdminListFilter = new()
                {
                    RadioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList,
                    PageCount = (int)Math.Ceiling(pageCount),
                    CurrentPageIndex = pageIndex,
                    WarningTypes = await _rnwRepository.GetWarningTypes(),
                    Years = await _rnwRepository.GetYears(),
                    WarningType = warningType,
                    Year = year,
                    SrNo = srNo,
                };
                return radioNavigationalWarningsAdminListFilter;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.RNWAdminListError.ToEventId(), ex, "Maritime safety information request failed to get RNW records for Admin from database with exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
            return await _rnwRepository.GetWarningTypes();
        }

        public async Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsData(string correlationId)
        {
            try
            {
                _logger.LogInformation(EventIds.RNWListDetailFromDatabaseStarted.ToEventId(), "Maritime safety information request to get RNW details from database started for _X-Correlation-ID:{correlationId}", correlationId);

                List<RadioNavigationalWarningsData> radioNavigationalWarningsData = await _rnwRepository.GetRadioNavigationalWarningsDataList();

                _logger.LogInformation(EventIds.RNWListDetailFromDatabaseCompleted.ToEventId(), "Maritime safety information request to get RNW details from database completed for _X-Correlation-ID:{correlationId}", correlationId);

                return radioNavigationalWarningsData;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ErrorInRNWListDetailFromDatabase.ToEventId(), ex, "Maritime safety information error has occurred in the process to get RNW detail from database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<string> GetRadioNavigationalWarningsLastModifiedDateTime(string correlationId)
        {
            _logger.LogInformation(EventIds.RNWLastModifiedDateTimeFromDatabaseStarted.ToEventId(), "Maritime safety information request to get RNW last modified date time from database started for _X-Correlation-ID:{correlationId}", correlationId);

            DateTime lastUpdatedDateTime = await _rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();

            _logger.LogInformation(EventIds.RNWLastModifiedDateTimeFromDatabaseCompleted.ToEventId(), "Maritime safety information request to get RNW last modified date time from database completed for _X-Correlation-ID:{correlationId}", correlationId);

            return DateTimeExtensions.ToRnwDateFormat(lastUpdatedDateTime);
        }

        #region Edit Radio Navigational Warning
        public EditRadioNavigationalWarningsAdmin EditRadioNavigationWarningListForAdmin(int? id, string correlationId)
        {
            try
            {
                EditRadioNavigationalWarningsAdmin radioNavigationalWarningsAdminList = _rnwRepository.EditRadioNavigation(id);
                return radioNavigationalWarningsAdminList;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ErrorInRetrievingRNWRecord.ToEventId(), ex, "Maritime safety information error has occurred while retrieving a RNW record from database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<bool> EditRadioNavigationWarningsRecord(EditRadioNavigationalWarningsAdmin radioNavigationalWarning, string correlationId)
        {
            if (radioNavigationalWarning.WarningType != WarningTypes.UK_Coastal && radioNavigationalWarning.WarningType != WarningTypes.NAVAREA_1)
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
                _logger.LogInformation(EventIds.EditRNWRecordStart.ToEventId(), "Maritime safety information edit RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                await _rnwRepository.UpdateRadioNavigationWarning(radioNavigationalWarning);
                _logger.LogInformation(EventIds.EditRNWRecordCompleted.ToEventId(), "Maritime safety information edit RNW record to database request completed for _X-Correlation-ID:{correlationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.ErrorInRnwRepositoryProcess.ToEventId(), ex, "Maritime safety information error has occurred in the process to edit RNW record to database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }

            return true;
        }

        #endregion Edit Radio Navigational Warning
    }
}
