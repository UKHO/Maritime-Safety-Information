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

        #region Edit Radio Navigational Warning
        public EditRadioNavigationalWarningsAdmin EditRadioNavigation(int id)
        {
            RadioNavigationalWarning rnwWarnings = _context.Set<RadioNavigationalWarning>().Find(id);
            EditRadioNavigationalWarningsAdmin rnwList = new();
            rnwList.Id = rnwWarnings.Id;
            string WarningName = _context.WarningType.Where(x => x.Id == rnwWarnings.WarningType).FirstOrDefault().Name;
            rnwList.WarningTypeName = WarningName;
            rnwList.Reference = rnwWarnings.Reference;
            rnwList.DateTimeGroup = rnwWarnings.DateTimeGroup;
            rnwList.DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup);
            rnwList.Summary = rnwWarnings.Summary;
            rnwList.Content = RnwHelper.FormatContent(rnwWarnings.Content);
            rnwList.ExpiryDate = rnwWarnings.ExpiryDate;
            rnwList.ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate);
            rnwList.IsDeleted = rnwWarnings.IsDeleted ? "true" : "false";
            return rnwList;
        }

        public async Task UpdateRadioNavigationWarning(EditRadioNavigationalWarningsAdmin radioNavigationalWarningAdmin)
        {
            RadioNavigationalWarning rnwList = new();
            rnwList.WarningType = radioNavigationalWarningAdmin.WarningType;
            rnwList.Reference = radioNavigationalWarningAdmin.Reference;
            rnwList.DateTimeGroup = radioNavigationalWarningAdmin.DateTimeGroup;
            rnwList.Summary = radioNavigationalWarningAdmin.Summary;
            rnwList.Content = RnwHelper.FormatContent(radioNavigationalWarningAdmin.Content);
            rnwList.ExpiryDate = radioNavigationalWarningAdmin.ExpiryDate;
            rnwList.IsDeleted = bool.Parse(radioNavigationalWarningAdmin.IsDeleted);
            rnwList.LastModified = DateTime.UtcNow;
            _context.Update(rnwList);
            await _context.SaveChangesAsync();
        }

        public int GetWarningType(EditRadioNavigationalWarningsAdmin radioNavigationalWarningAdminList)
        {
            var x = _context.WarningType.Select(x => x.Name).ToList();
            return _context.WarningType.Where(x => x.Name == radioNavigationalWarningAdminList.WarningTypeName).FirstOrDefault().Id;
        }
        #endregion Edit Radio Navigational Warning
    }
}
