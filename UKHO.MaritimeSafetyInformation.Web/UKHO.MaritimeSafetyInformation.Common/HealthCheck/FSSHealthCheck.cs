using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public class FSSHealthCheck : IHealthCheck
    {
        private readonly IFSSHealthClient _fssHealthClient;
        private readonly ILogger<FSSHealthCheck> _logger;

        public FSSHealthCheck(IFSSHealthClient fssHealthClient, ILogger<FSSHealthCheck> logger)
        {
            _fssHealthClient = fssHealthClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult healthCheckResult = await _fssHealthClient.CheckHealthAsync();
            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                _logger.LogDebug(EventIds.FssIsHealthy.ToEventId(), "FSS for maritime safety information is healthy");
            }
            else
            {
                _logger.LogError(EventIds.FssIsUnHealthy.ToEventId(), healthCheckResult.Exception, "FSS for maritime safety information is unhealthy responded with error {Message}", healthCheckResult.Exception.Message);
            }
            return healthCheckResult;
        }
    }
}
