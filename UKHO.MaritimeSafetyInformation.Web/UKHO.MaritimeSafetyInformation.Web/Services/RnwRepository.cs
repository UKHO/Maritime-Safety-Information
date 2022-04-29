using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : IRnwRepository
    {
        private readonly RadioNavigationalWarningsContext _context;
        private readonly IConfiguration _configuration;
        public RnwRepository(RadioNavigationalWarningsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings)
        {
            _context.Add(radioNavigationalWarnings);
            _context.SaveChanges();
        }

        public RadioNavigationalWarningsAdminListFilter GetRadioNavigationForAdmin(int pageIndex, int warningTypeId, string year)
        {

            RadioNavigationalWarningsAdminListFilter radioNavigationalWarningsAdminListFilter = new();
            List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminList = new();
            int rnwAdminListRecordPerPage = _configuration.GetValue<int>("RadioNavigationalWarningConfiguration:AdminListRecordPerPage");
            List<RadioNavigationalWarnings> radioNavigationalWarnings = _context.RadioNavigationalWarnings.ToList();
            List<WarningType> warningType = _context.WarningType.ToList();
            int SrNo = (pageIndex - 1) * rnwAdminListRecordPerPage;

            radioNavigationalWarningsAdminList = (from rnwWarnings in radioNavigationalWarnings
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
            return  radioNavigationalWarningsAdminListFilter;
        }
    }
}
