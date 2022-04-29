using UKHO.MaritimeSafetyInformation.Common;
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

        public async Task AddRadioNavigation(RadioNavigationalWarnings radioNavigationalWarnings)
        {
            _context.Add(radioNavigationalWarnings);
            await _context.SaveChangesAsync();
        }

        public List<WarningType> GetWarningType()
        {
            List<WarningType> warningType = (from c in _context.WarningType select c).ToList();
            return warningType;
        }
    }
}
