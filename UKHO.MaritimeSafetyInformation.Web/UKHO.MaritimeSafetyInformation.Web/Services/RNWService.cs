using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RNWService : IRNWService
    {
        private readonly IRNWRepository _rnwRepository;
        private readonly ILogger<RNWService> _logger;

        public RNWService(IRNWRepository repository,
                        ILogger<RNWService> logger)
        {
            _rnwRepository = repository;
            _logger = logger;
        }

        public async Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarning radioNavigationalWarning, string correlationId)
        {
            if (radioNavigationalWarning.WarningType != WarningTypes.UK_Coastal && radioNavigationalWarning.WarningType != WarningTypes.NAVAREA_1)
            {
                await Task.CompletedTask;
                _logger.LogInformation(EventIds.MSIInvalidWarningTypeInRequest.ToEventId(), "Maritime safety information invalid value received for parameter warningType for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new InvalidDataException("Invalid value received for parameter warningType");
            }

            if (string.IsNullOrEmpty(radioNavigationalWarning.Reference))
            {
                _logger.LogInformation(EventIds.MSIInvalidReferenceInRequest.ToEventId(), "Maritime safety information invalid value received for parameter reference for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new ArgumentNullException("Invalid value received for parameter reference", new Exception());
            }

            if (string.IsNullOrEmpty(radioNavigationalWarning.Summary))
            {
                _logger.LogInformation(EventIds.MSIInvalidSummaryInRequest.ToEventId(), "Maritime safety information invalid value received for parameter summary for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new ArgumentNullException("Invalid value received for parameter summary", new Exception());
            }

            if (string.IsNullOrEmpty(radioNavigationalWarning.Content))
            {
                _logger.LogInformation(EventIds.MSIInvalidContentInRequest.ToEventId(), "Maritime safety information invalid value received for parameter content for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new ArgumentNullException("Invalid value received for parameter content", new Exception());
            }

            try
            {
                _logger.LogInformation(EventIds.MSIAddNewRNWRecordStart.ToEventId(), "Maritime safety information add new RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                await _rnwRepository.AddRadioNavigationWarning(radioNavigationalWarning);
                _logger.LogInformation(EventIds.MSIAddNewRNWRecordCompleted.ToEventId(), "Maritime safety information add new RNW record to database request completed for _X-Correlation-ID:{correlationId}", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.MSIErrorInRnwRepositoryProcess.ToEventId(), ex, "Maritime safety information error has occurred in the process to add new RNW record to database with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }

            return true;
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
           return await _rnwRepository.GetWarningTypes();
        }
    }
}
