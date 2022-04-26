using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public interface IEventHubLoggingHealthClient
    {
        Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
    }
}
