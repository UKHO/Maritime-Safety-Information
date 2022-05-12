using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwService : IRnwService
    {
        private readonly IRnwRepository _rnwRepository;
        private readonly ILogger<RnwService> _logger;

        public RnwService(IRnwRepository repository,
                        ILogger<RnwService> logger)
        {
            _rnwRepository = repository;
            _logger = logger;
        }

        public async Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarnings radioNavigationalWarnings, string correlationId)
        {
            try
            {
                if (radioNavigationalWarnings.WarningType != 0 && radioNavigationalWarnings.Reference != "" && radioNavigationalWarnings.DateTimeGroup.ToString() != null
                           && radioNavigationalWarnings.Summary != null && radioNavigationalWarnings.Content != null)
                {
                    _logger.LogInformation(EventIds.MSIAddNewRNWRecordStart.ToEventId(), "Maritime safety information add new RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                    await _rnwRepository.AddRadioNavigationWarnings(radioNavigationalWarnings);
                    _logger.LogInformation(EventIds.MSIAddNewRNWRecordCompleted.ToEventId(), "Maritime safety information add new RNW record to database request completed for _X-Correlation-ID:{correlationId}", correlationId);

                    return true;
                }
                else
                {
                    _logger.LogInformation(EventIds.MSIInvalidNewRNWRecordRequest.ToEventId(), "Maritime safety information invalid new RNW record request for _X-Correlation-ID:{correlationId}", correlationId);
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.MSIAddNewRNWRequestError.ToEventId(), ex, "Maritime safety information add new RNW record to database request failed with error with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
           return await _rnwRepository.GetWarningTypes();
        }
    }
}
