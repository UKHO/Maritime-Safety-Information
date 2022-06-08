using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public interface IFSSHealthClient
    {
        Task<HealthCheckResult> CheckHealthAsync();
    }
}
