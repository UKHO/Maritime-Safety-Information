using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : IRnwRepository
    {
        private readonly RadioNavigationalWarningsContext _context;

        public RnwRepository(RadioNavigationalWarningsContext context)
        {
            _context = context;
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
                         Content = RnwHelper.FormatContent(rnwWarnings.Content),
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

        public RadioNavigationalWarningsAdminList EditRadioNavigation(int id)
        {
            try
            {
                var rnwWarnings = _context.Set<RadioNavigationalWarnings>().Find(id);
                RadioNavigationalWarningsAdminList rnwList = new RadioNavigationalWarningsAdminList();
                rnwList.Id = rnwWarnings.Id;
                var WarningName = _context.WarningType.Where(x => x.Id == rnwWarnings.WarningType).FirstOrDefault().Name;
                rnwList.WarningTypeName = WarningName;
                rnwList.Reference = rnwWarnings.Reference;
                rnwList.DateTimeGroup = rnwWarnings.DateTimeGroup;
                 rnwList.DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.DateTimeGroup);
                rnwList.Summary = rnwWarnings.Summary;
                rnwList.Content = RnwHelper.FormatContent(rnwWarnings.Content);
                rnwList.ExpiryDate = rnwWarnings.ExpiryDate;
                 rnwList.ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate);
                rnwList.IsDeleted = rnwWarnings.IsDeleted ? "Yes" : "No";
                
                return rnwList;
            }
            catch (Exception ex)
            {

                throw;
            }
            
                      
        }

        public async Task AddRadioNavigationWarning(RadioNavigationalWarningsAdminList radioNavigationalWarningAdminList)
        {
            RadioNavigationalWarnings rnwList = new RadioNavigationalWarnings();
            rnwList.Id = radioNavigationalWarningAdminList.Id;
            //var WarningName = _context.WarningType.Where(x => x.Id == rnwWarnings.WarningType).FirstOrDefault().Name;
            var warningType = _context.WarningType.Where(x=>x.Name == radioNavigationalWarningAdminList.WarningTypeName).FirstOrDefault().Id;
            rnwList.WarningType = warningType;
            rnwList.Reference = radioNavigationalWarningAdminList.Reference;
            rnwList.DateTimeGroup = radioNavigationalWarningAdminList.DateTimeGroup;
            //rnwList.DateTimeGroupRnwFormat = DateTimeExtensions.ToRnwDateFormat(radioNavigationalWarningAdminList.DateTimeGroup);
            rnwList.Summary = radioNavigationalWarningAdminList.Summary;
            rnwList.Content = RnwHelper.FormatContent(radioNavigationalWarningAdminList.Content);
            rnwList.ExpiryDate = radioNavigationalWarningAdminList.ExpiryDate;
            //rnwList.ExpiryDateRnwFormat = DateTimeExtensions.ToRnwDateFormat(rnwWarnings.ExpiryDate);
            //rnwList.IsDeleted = radioNavigationalWarningAdminList.IsDeleted ? "Yes" : "No";
            _context.Update(rnwList);
           
            
            await _context.SaveChangesAsync();
        }


    }
}
