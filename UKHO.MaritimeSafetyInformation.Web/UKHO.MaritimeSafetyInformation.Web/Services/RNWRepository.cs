using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RNWRepository : IRNWRepository
    {
        private readonly RadioNavigationalWarningsContext _context;

        public RNWRepository(RadioNavigationalWarningsContext context)
        {
            _context = context;
        }

        public async Task AddRadioNavigationWarning(RadioNavigationalWarning radioNavigationalWarning)
        {
            _context.Add(radioNavigationalWarning);
            await _context.SaveChangesAsync(); 
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
            return await _context.WarningType.ToListAsync();
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
                         Content = rnwWarnings.Content != null ? rnwWarnings.Content.Length > 300 ? string.Concat(rnwWarnings.Content.Substring(0, 300), "...") : rnwWarnings.Content : string.Empty,
                         ExpiryDate = rnwWarnings.ExpiryDate,
                         ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate),
                         IsDeleted = rnwWarnings.IsDeleted ? "Yes" : "No",
                         WarningTypeName = warning.Name
                     }).OrderByDescending(a => a.DateTimeGroup).ToListAsync();

            return radioNavigationalWarningsAdminLists;
        }


        public async Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsDataList()
        {
            List<RadioNavigationalWarningsData> radioNavigationalWarningsData
            = await (from rnwWarnings in _context.RadioNavigationalWarnings
                     join warning in _context.WarningType on rnwWarnings.WarningType equals warning.Id
                     where !rnwWarnings.IsDeleted && rnwWarnings.ExpiryDate >= DateTime.UtcNow
                     select new RadioNavigationalWarningsData
                     {
                         Reference = rnwWarnings.Reference,
                         DateTimeGroup = rnwWarnings.DateTimeGroup,
                         Description = rnwWarnings.Summary,
                         DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup)
                     }).OrderByDescending(a => a.DateTimeGroup)
                     .ToListAsync();

            return radioNavigationalWarningsData;
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
