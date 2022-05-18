using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;


namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwService : IRnwService
    {
        private readonly IRnwRepository _rnwRepository;
        private readonly ILogger<RnwService> _logger;
        private readonly IOptions<RadioNavigationalWarningConfiguration> _radioNavigationalWarningConfiguration;

        public RnwService(IRnwRepository repository,
                        ILogger<RnwService> logger, IOptions<RadioNavigationalWarningConfiguration> radioNavigationalWarningConfiguration)
        {
            _rnwRepository = repository;
            _radioNavigationalWarningConfiguration = radioNavigationalWarningConfiguration;
            _logger = logger;
        }

        public async Task<bool> CreateNewRadioNavigationWarningsRecord(RadioNavigationalWarnings radioNavigationalWarnings, string correlationId)
        {
            if (radioNavigationalWarnings.WarningType != WarningTypes.UK_Coastal && radioNavigationalWarnings.WarningType != WarningTypes.NAVAREA_1)
            {
                await Task.CompletedTask;
                _logger.LogInformation(EventIds.MSIInvalidWarningTypeInRequest.ToEventId(), "Maritime safety information invalid value received for parameter warningType for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new InvalidDataException("Invalid value recieved for parameter warningType");
            }

            if (string.IsNullOrEmpty(radioNavigationalWarnings.Reference))
            {
                _logger.LogInformation(EventIds.MSIInvalidReferenceInRequest.ToEventId(), "Maritime safety information invalid value received for parameter reference for the _X-Correlation-ID:{correlationId}", correlationId);
                throw new ArgumentNullException("Invalid value recieved for parameter reference", new Exception());
            }

            if (string.IsNullOrEmpty(radioNavigationalWarnings.Summary))
            {
                _logger.LogInformation(EventIds.MSIInvalidSummaryInRequest.ToEventId(), "Maritime safety information invalid value received for parameter summary for the _X-Correlation-ID", correlationId);
                throw new ArgumentNullException("Invalid value recieved for parameter summary", new Exception());
            }

            if (string.IsNullOrEmpty(radioNavigationalWarnings.Content))
            {
                _logger.LogInformation(EventIds.MSIInvalidContentInRequest.ToEventId(), "Maritime safety information invalid value received for parameter content for the _X-Correlation-ID", correlationId);
                throw new ArgumentNullException("Invalid value recieved for parameter content", new Exception());
            }

            try
            {
                _logger.LogInformation(EventIds.MSIAddNewRNWRecordStart.ToEventId(), "Maritime safety information add new RNW record to database request started for _X-Correlation-ID:{correlationId}", correlationId);
                await _rnwRepository.AddRadioNavigationWarnings(radioNavigationalWarnings);
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
                _logger.LogError(EventIds.MSIGetRnwForAdminListError.ToEventId(), ex, "Maritime safety information request failed to get RNW records for Admin from database with exception:{ex} and _X-Correlation-ID:{correlationId}", ex.Message, correlationId);
                throw;
            }
        }
    }
}