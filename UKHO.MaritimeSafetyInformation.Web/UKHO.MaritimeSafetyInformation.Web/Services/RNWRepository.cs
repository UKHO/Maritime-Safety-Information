using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
#if MSIAdminProject
        [ExcludeFromCodeCoverage]
#endif

    public class RNWRepository : IRNWRepository
    {
        private readonly RadioNavigationalWarningsContext _context;

        public RNWRepository(RadioNavigationalWarningsContext context)
        {
            _context = context;
        }

        public async Task AddRadioNavigationWarning(RadioNavigationalWarning radioNavigationalWarning)
        {
            radioNavigationalWarning.LastModified = DateTime.UtcNow;
            _context.Add(radioNavigationalWarning);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RadioNavigationalWarningsAdmin>> GetRadioNavigationWarningsAdminList()
        {
            return await (from rnwWarnings in _context.RadioNavigationalWarnings
                          where !rnwWarnings.IsDeleted
                          join warning in _context.WarningType on rnwWarnings.WarningType equals warning.Id
                          select new RadioNavigationalWarningsAdmin
                          {
                              Id = rnwWarnings.Id,
                              WarningType = rnwWarnings.WarningType,
                              Reference = rnwWarnings.Reference,
                              DateTimeGroup = rnwWarnings.DateTimeGroup,
                              DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup),
                              Summary = rnwWarnings.Summary,
                              Content = RnwHelper.FormatContent(rnwWarnings.Content),
                              ExpiryDate = rnwWarnings.ExpiryDate,
                              ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate),
                              WarningTypeName = warning.Name,
                              Status = DateTime.Compare(rnwWarnings.ExpiryDate ?? DateTime.UtcNow.AddDays(1), DateTime.UtcNow) < 1 ? "Expired" : "Active"
                          }).OrderByDescending(a => a.DateTimeGroup).ToListAsync();
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
            return await _context.WarningType.ToListAsync();
        }

        public async Task<List<string>> GetYears()
        {
            return await _context.RadioNavigationalWarnings
                                .Select(p => p.DateTimeGroup.Year.ToString())
                                .Distinct().ToListAsync();
        }

        public async Task<List<RadioNavigationalWarningsData>> GetRadioNavigationalWarningsDataList()
        {
            return await (from rnwWarnings in _context.RadioNavigationalWarnings
                          join warningType in _context.WarningType on rnwWarnings.WarningType equals warningType.Id
                          where !rnwWarnings.IsDeleted && (rnwWarnings.ExpiryDate == null || rnwWarnings.ExpiryDate >= DateTime.UtcNow)
                          select new RadioNavigationalWarningsData
                          {
                              Id = rnwWarnings.Id,
                              WarningType = warningType.Name,
                              Reference = rnwWarnings.Reference,
                              DateTimeGroup = rnwWarnings.DateTimeGroup,
                              Description = rnwWarnings.Summary,
                              DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup),
                              Content = rnwWarnings.Content
                          }).OrderByDescending(a => a.DateTimeGroup)
                     .ToListAsync();
        }

        public async Task<List<RadioNavigationalWarningsData>> GetSelectedRadioNavigationalWarningsDataList(int[] selectedIds)
        {
            return await (from rnwWarnings in _context.RadioNavigationalWarnings
                          join warningType in _context.WarningType on rnwWarnings.WarningType equals warningType.Id
                          where !rnwWarnings.IsDeleted && (rnwWarnings.ExpiryDate == null || rnwWarnings.ExpiryDate >= DateTime.UtcNow) && selectedIds.Contains(rnwWarnings.Id)
                          select new RadioNavigationalWarningsData
                          {
                              Id = rnwWarnings.Id,
                              WarningType = warningType.Name,
                              Reference = rnwWarnings.Reference,
                              DateTimeGroup = rnwWarnings.DateTimeGroup,
                              Description = rnwWarnings.Summary,
                              DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup),
                              Content = rnwWarnings.Content
                          }).OrderByDescending(a => a.DateTimeGroup)
                     .ToListAsync();
        }

        public async Task<DateTime> GetRadioNavigationalWarningsLastModifiedDateTime()
        {
            try
            {
                if (_context.RadioNavigationalWarnings.Any())
                {
                    return await _context.RadioNavigationalWarnings.MaxAsync(i => i.LastModified);
                }
                return DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public RadioNavigationalWarning GetRadioNavigationalWarningById(int id)
        {
            return _context.Set<RadioNavigationalWarning>().Find(id);
        }

        public async Task UpdateRadioNavigationalWarning(RadioNavigationalWarning radioNavigationalWarning)
        {
            radioNavigationalWarning.LastModified = DateTime.UtcNow;
            _context.Update(radioNavigationalWarning);
            await _context.SaveChangesAsync();
        }

    }
}
