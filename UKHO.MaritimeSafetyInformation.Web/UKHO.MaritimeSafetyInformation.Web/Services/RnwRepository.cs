using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class RnwRepository : IRnwRepository
    {
        private readonly RadioNavigationalWarningsContext _context;

        public RnwRepository(RadioNavigationalWarningsContext context)
        {
            _context = context;
        }

        public async Task AddRadioNavigationWarnings(RadioNavigationalWarnings radioNavigationalWarnings)
        {
            _context.Add(radioNavigationalWarnings);
            await _context.SaveChangesAsync(); 
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
            return await _context.WarningType.ToListAsync();
        }
    }
}
