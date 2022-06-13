
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public interface IRNWDatabaseHealthClient
    {
        Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken);
    }
}
