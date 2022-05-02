using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : IRnwRepository
    {
        private readonly RadioNavigationalWarningsContext _context;
        private readonly IOptions<RadioNavigationalWarningConfiguration> _radioNavigationalWarningConfiguration;
        public RnwRepository(RadioNavigationalWarningsContext context, IOptions<RadioNavigationalWarningConfiguration> radioNavigationalWarningConfiguration)
        {
            _context = context;
            _radioNavigationalWarningConfiguration = radioNavigationalWarningConfiguration;
        }

        public RadioNavigationalWarningsAdminListFilter GetRadioNavigationWarningsForAdmin(int pageIndex, int warningTypeId, string year)
        {
            RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminListFilter = new();
            List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminList = new();

            int rnwAdminListRecordPerPage = _radioNavigationalWarningConfiguration.Value.AdminListRecordPerPage;
            List<RadioNavigationalWarnings> radioNavigationalWarnings = GetRadioNavigationWarnings();
            List<WarningType> warningType = GetWarningTypes();
            int SrNo = (pageIndex - 1) * rnwAdminListRecordPerPage;

            radioNavigationalWarningsAdminList = GetRadioNavigationWarningsAdminList(radioNavigationalWarnings, warningType);

            if (warningTypeId != 0)
            {
                radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.WarningType == warningTypeId).ToList();
            }

            if (!string.IsNullOrEmpty(year))
            {
                radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Where(a => a.DateTimeGroup.Year.ToString().Trim() == year).ToList();
            }

            double pageCount = (double)(radioNavigationalWarningsAdminList.Count / Convert.ToDecimal(rnwAdminListRecordPerPage));
            radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Skip(SrNo).Take(rnwAdminListRecordPerPage).ToList();

            radioNavigationalWarningsAdminListFilter.RadioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList;
            radioNavigationalWarningsAdminListFilter.PageCount = (int)Math.Ceiling(pageCount);
            radioNavigationalWarningsAdminListFilter.CurrentPageIndex = pageIndex;
            radioNavigationalWarningsAdminListFilter.WarningTypes = warningType;
            radioNavigationalWarningsAdminListFilter.Years = (from p in radioNavigationalWarnings
                                                              select p.DateTimeGroup.Year.ToString()).Distinct().ToList();
            radioNavigationalWarningsAdminListFilter.WarningType = warningTypeId;
            radioNavigationalWarningsAdminListFilter.Year = year;
            radioNavigationalWarningsAdminListFilter.SrNo = SrNo;
            return radioNavigationalWarningsAdminListFilter;
        }

        private List<RadioNavigationalWarnings> GetRadioNavigationWarnings()
        {
            return _context.RadioNavigationalWarnings.ToList();
        }

        private List<WarningType> GetWarningTypes()
        {
            return _context.WarningType.ToList();
        }

        private static List<RadioNavigationalWarningsAdminList> GetRadioNavigationWarningsAdminList(List<RadioNavigationalWarnings> radioNavigationalWarnings, List<WarningType> warningType)
        {
            List<RadioNavigationalWarningsAdminList>  radioNavigationalWarningsAdminList = (from rnwWarnings in radioNavigationalWarnings
                                                  join warning in warningType on rnwWarnings.WarningType equals warning.Id
                                                  select new RadioNavigationalWarningsAdminList
                                                  {
                                                      Id = rnwWarnings.Id,
                                                      WarningType = rnwWarnings.WarningType,
                                                      Reference = rnwWarnings.Reference,
                                                      DateTimeGroup = rnwWarnings.DateTimeGroup,
                                                      Summary = rnwWarnings.Summary,
                                                      Content = rnwWarnings.Content,
                                                      ExpiryDate = rnwWarnings.ExpiryDate,
                                                      IsDeleted = rnwWarnings.IsDeleted == true ? "Yes" : "No",
                                                      WarningTypeName = warning.Name

                                                  })
                                                 .OrderByDescending(a => a.DateTimeGroup).ToList();

            return radioNavigationalWarningsAdminList;
        }
    }
}
