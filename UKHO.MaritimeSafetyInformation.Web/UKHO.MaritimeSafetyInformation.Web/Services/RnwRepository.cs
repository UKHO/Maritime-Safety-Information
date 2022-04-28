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

        public RadioNavigationalWarningsAdminListPage GetRadioNavigationForAdmin(int pageIndex = 1)
        {
            RadioNavigationalWarningsAdminListPage radioNavigationalWarningsAdminListPage = new();
            List<RadioNavigationalWarningsAdminList> radioNavigationalWarningsAdminList = new();
            int rnwAdminListRecordPerPage = _configuration.GetValue<int>("RadioNavigationalWarningConfiguration:AdminListRecordPerPage");

            radioNavigationalWarningsAdminList = (from rnwWarnings in _context.RadioNavigationalWarnings
                                                  join warningType in _context.WarningType on rnwWarnings.WarningType equals warningType.Id
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
                                                      WarningTypeName = warningType.Name

                                                  })
                                                  .OrderByDescending(a => a.DateTimeGroup).ToList();

            double pageCount = (double)(radioNavigationalWarningsAdminList.Count / Convert.ToDecimal(rnwAdminListRecordPerPage));
            radioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList.Skip((pageIndex - 1) * rnwAdminListRecordPerPage).Take(rnwAdminListRecordPerPage).ToList();

            radioNavigationalWarningsAdminListPage.RadioNavigationalWarningsAdminList = radioNavigationalWarningsAdminList;
            radioNavigationalWarningsAdminListPage.PageCount = (int)Math.Ceiling(pageCount);
            radioNavigationalWarningsAdminListPage.CurrentPageIndex = pageIndex;
            return radioNavigationalWarningsAdminListPage;
        }
    }
}
