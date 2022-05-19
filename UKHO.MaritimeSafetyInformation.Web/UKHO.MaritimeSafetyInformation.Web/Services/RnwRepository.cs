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

        public async Task<List<RadioNavigationalWarningsAdminList>> GetRadioNavigationWarningsAdminList()
        {
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
                         Content = FormatContent(rnwWarnings.Content),
                         ExpiryDate = rnwWarnings.ExpiryDate,
                         ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate),
                         IsDeleted = rnwWarnings.IsDeleted ? "Yes" : "No",
                         WarningTypeName = warning.Name
                     }).OrderByDescending(a => a.DateTimeGroup).ToListAsync();

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

        private static string FormatContent(string content)
        {
            if(!string.IsNullOrEmpty(content))
            {
                return content.Length > 300 ? string.Concat(content.Substring(0, 300), "...") : content;
            }
            return string.Empty;
        }
    }
}
