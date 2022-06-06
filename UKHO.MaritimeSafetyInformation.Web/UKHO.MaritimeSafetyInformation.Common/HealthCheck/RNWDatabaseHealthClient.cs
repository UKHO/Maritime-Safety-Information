
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public class RNWDatabaseHealthClient : IRNWDatabaseHealthClient
    {
        private readonly RadioNavigationalWarningsContext _context;

        public RNWDatabaseHealthClient(RadioNavigationalWarningsContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync()
        {
            try
            {
                await _context.WarningType.ToListAsync();

                return HealthCheckResult.Healthy("Radio Navigational Warning database is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Radio Navigational Warning database is unhealthy", new Exception(ex.Message));
            }      
        }
    }
}
