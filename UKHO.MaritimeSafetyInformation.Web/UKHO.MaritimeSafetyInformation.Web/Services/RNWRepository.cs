using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common;
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
            _context.Add(radioNavigationalWarning);
            await _context.SaveChangesAsync(); 
        }

        public async Task<List<WarningType>> GetWarningTypes()
        {
            return await _context.WarningType.ToListAsync();
        }
    }
}
