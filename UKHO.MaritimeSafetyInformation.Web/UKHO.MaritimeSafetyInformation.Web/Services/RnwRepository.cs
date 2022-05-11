using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
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

        public async Task<List<RadioNavigationalWarningsAdminList>> GetRadioNavigationWarningsAdminList(string correlationId)
        {
            _logger.LogInformation(EventIds.MSIGetRnwForAdminDatabaseCallStarted.ToEventId(), "Maritime safety information query to get RNW records for Admin from database started for _X-Correlation-ID:{correlationId}", correlationId);

            List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminLists
            = await (from rnwWarnings in _context.RadioNavigationalWarnings
                     join warning in _context.WarningType on rnwWarnings.WarningType equals warning.Id
                     select new RadioNavigationalWarningsAdminList
                     {
                         Id = rnwWarnings.Id,
                         WarningType = rnwWarnings.WarningType,
                         Reference = rnwWarnings.Reference,
                         DateTimeGroup = rnwWarnings.DateTimeGroup,
                         DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup),
                         Summary = rnwWarnings.Summary,
                         Content = rnwWarnings.Content != null ? rnwWarnings.Content.Length > 300 ? string.Concat(rnwWarnings.Content.Substring(0, 300), "...") : rnwWarnings.Content : string.Empty,
                         ExpiryDate = rnwWarnings.ExpiryDate,
                         ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate),
                         IsDeleted = rnwWarnings.IsDeleted ? "Yes" : "No",
                         WarningTypeName = warning.Name
                     }).OrderByDescending(a => a.DateTimeGroup).ToListAsync();

            _logger.LogInformation(EventIds.MSIGetRnwForAdminDatabaseCallCompleted.ToEventId(), "Maritime safety information query to get RNW records for Admin from database completed for _X-Correlation-ID:{correlationId}", correlationId);
            return radioNavigationalWarningsAdminLists;
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
            return await _context.WarningType.ToListAsync();
        }

        public async Task<List<string>> GetYears()
        {
            List<string> years = await (_context.RadioNavigationalWarnings
                                .Select(p => p.DateTimeGroup.Year.ToString())
                                .Distinct().ToListAsync());
            return years;
        }
    }
}
