using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : IRnwRepository
    {
        private readonly RadioNavigationalWarningsContext _context;
        private readonly ILogger<RnwRepository> _logger;

        public RnwRepository(RadioNavigationalWarningsContext context,
                             ILogger<RnwRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddRadioNavigationWarnings(RadioNavigationalWarnings radioNavigationalWarnings, string correlationId)
        {
            try
            {
                if (radioNavigationalWarnings.WarningType != 0 && radioNavigationalWarnings.Reference != "" && radioNavigationalWarnings.DateTimeGroup.HasValue 
                    && radioNavigationalWarnings.Summary != null && radioNavigationalWarnings.Content != null)
                {
                    _logger.LogInformation(EventIds.MSIAddNewRNWRecordStart.ToEventId(), "Maritime safety information add new RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                    _context.Add(radioNavigationalWarnings);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation(EventIds.MSIAddNewRNWRecordCompleted.ToEventId(), "Maritime safety information add new RNW record to database request completed for _X-Correlation-ID:{correlationId}", correlationId);

                    return true;
                }
                else
                {
                    _logger.LogInformation(EventIds.MSIAddNewRNWRecordCompleted.ToEventId(), "Maritime safety information add new RNW record to database request failed for _X-Correlation-ID:{correlationId}", correlationId);

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.MSIAddNewRNWRequestError.ToEventId(), ex, "Maritime safety information add new RNW record to database request failed with error with Exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                return false;
            }
        }

        public List<WarningType> GetWarningType()
        {
            List<WarningType> warningType = (from c in _context.WarningType select c).ToList();
            return warningType;
        }
    }
}
