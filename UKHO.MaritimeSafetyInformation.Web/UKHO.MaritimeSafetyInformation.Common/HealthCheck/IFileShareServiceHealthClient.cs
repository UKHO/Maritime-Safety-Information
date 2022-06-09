using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public interface IFileShareServiceHealthClient
    {
        Task<HealthCheckResult> CheckHealthAsync();
    }
}
