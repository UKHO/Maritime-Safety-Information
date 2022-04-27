using Microsoft.AspNetCore.Mvc.Rendering;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.DTO;
using UKHO.MaritimeSafetyInformation.Common.Models.RNW;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : IRnwRepository
    {
        private readonly RadioNavigationalWarningsContext _context;
        public RnwRepository(RadioNavigationalWarningsContext context)
        {
            _context = context;
        }

        public void AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings)
        {
            _context.Add(radioNavigationalWarnings);
            _context.SaveChanges();
        }

        public RadioNavigationalWarningsAdminList GetRadioNavigationForAdmin(int pageIndex = 1)
        {
            RadioNavigationalWarningsAdminList radioNavigationalWarningsAdminList = new ();
            List<RadioNavigationalWarnings> radioNavigationalWarnings = new ();
            List<WarningType> warningTypes = new();
            const int RnwAdminListRecordPerPage = 20;

            radioNavigationalWarnings = _context.RadioNavigationalWarnings.ToList();
            warningTypes = _context.WarningType.ToList();
           // ViewBag.WarningTypes = new SelectList(warningTypes, "Id", "Name");

            radioNavigationalWarningsAdminList.RadioNavigationalWarnings = (from rnwWarnings in radioNavigationalWarnings
                                                                            select rnwWarnings)
                                                                           .OrderBy(rnwWarnings => rnwWarnings.DateTimeGroup)
                                                                           .Skip((pageIndex - 1) * RnwAdminListRecordPerPage)
                                                                           .Take(RnwAdminListRecordPerPage).ToList();

            double pageCount = (double)((decimal)radioNavigationalWarnings.Count() / Convert.ToDecimal(RnwAdminListRecordPerPage));
            radioNavigationalWarningsAdminList.PageCount = (int)Math.Ceiling(pageCount);
            radioNavigationalWarningsAdminList.CurrentPageIndex = pageIndex;
            return radioNavigationalWarningsAdminList;
        }
    }
}
