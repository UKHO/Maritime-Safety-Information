using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
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
            radioNavigationalWarning.LastModified = DateTime.UtcNow;
            _context.Add(radioNavigationalWarning);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RadioNavigationalWarningsAdmin>> GetRadioNavigationWarningsAdminList()
        {
            return await (from rnwWarnings in _context.RadioNavigationalWarnings
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
                              IsDeleted = rnwWarnings.IsDeleted ? "Yes" : "No",
                              WarningTypeName = warning.Name
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
                              WarningType = warningType.Name,
                              Reference = rnwWarnings.Reference,
                              DateTimeGroup = rnwWarnings.DateTimeGroup,
                              Description = rnwWarnings.Summary,
                              DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup),
                              Content = rnwWarnings.Content
                          }).OrderByDescending(a => a.DateTimeGroup)
                     .ToListAsync();
        }

        public async Task<List<RadioNavigationalWarningsData>> ShowRadioNavigationalWarningsDataList(int[] data)
        {
            return await (from rnwWarnings in _context.RadioNavigationalWarnings
                          join warningType in _context.WarningType on rnwWarnings.WarningType equals warningType.Id
                          where !rnwWarnings.IsDeleted && (rnwWarnings.ExpiryDate == null || rnwWarnings.ExpiryDate >= DateTime.UtcNow) && data.Contains(rnwWarnings.Id)
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
            return await _context.RadioNavigationalWarnings.MaxAsync(i => i.LastModified);
        }

        public EditRadioNavigationalWarningAdmin GetRadioNavigationalWarningById(int id)
        {
            RadioNavigationalWarning rnwWarning = _context.Set<RadioNavigationalWarning>().Find(id);
            EditRadioNavigationalWarningAdmin rnw = new();
            rnw.Id = rnwWarning.Id;
            string warningName = _context.WarningType.FirstOrDefault(x => x.Id == rnwWarning.WarningType).Name;
            rnw.WarningTypeName = warningName;
            rnw.Reference = rnwWarning.Reference;
            rnw.DateTimeGroup = rnwWarning.DateTimeGroup;
            rnw.Summary = rnwWarning.Summary;
            rnw.Content = rnwWarning.Content;
            rnw.ExpiryDate = rnwWarning.ExpiryDate;
            rnw.IsDeleted = rnwWarning.IsDeleted;
            return rnw;
        }

        public async Task UpdateRadioNavigationalWarning(EditRadioNavigationalWarningAdmin radioNavigationalWarningAdmin)
        {
            RadioNavigationalWarning rnw = await _context.RadioNavigationalWarnings.FirstOrDefaultAsync(r => r.Id == radioNavigationalWarningAdmin.Id);
            rnw.WarningType = radioNavigationalWarningAdmin.WarningType;
            rnw.Reference = radioNavigationalWarningAdmin.Reference;
            rnw.DateTimeGroup = radioNavigationalWarningAdmin.DateTimeGroup;
            rnw.Summary = radioNavigationalWarningAdmin.Summary;
            rnw.Content = radioNavigationalWarningAdmin.Content;
            rnw.ExpiryDate = radioNavigationalWarningAdmin.ExpiryDate;
            rnw.IsDeleted = radioNavigationalWarningAdmin.IsDeleted;
            rnw.LastModified = DateTime.UtcNow;
            _context.Update(rnw);
            await _context.SaveChangesAsync();
        }

    }
}
