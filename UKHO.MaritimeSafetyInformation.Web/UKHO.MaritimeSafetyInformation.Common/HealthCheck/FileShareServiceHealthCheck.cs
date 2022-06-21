using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public class FileShareServiceHealthCheck : IHealthCheck
    {
        private readonly IFileShareServiceHealthClient _fileShareServiceHealthClient;
        private readonly ILogger<FileShareServiceHealthCheck> _logger;

        public FileShareServiceHealthCheck(IFileShareServiceHealthClient fileShareServiceHealthClient, ILogger<FileShareServiceHealthCheck> logger)
        {
            _fileShareServiceHealthClient = fileShareServiceHealthClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult healthCheckResult = await _fileShareServiceHealthClient.CheckHealthAsync(cancellationToken);
            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                _logger.LogDebug(EventIds.FileShareServiceIsHealthy.ToEventId(), "File Share Service is healthy");
            }
            else
            {
                _logger.LogError(EventIds.FileShareServiceIsUnHealthy.ToEventId(), healthCheckResult.Exception, "File Share Service is unhealthy responded with error {Message}", healthCheckResult.Exception.Message);
            }
            return healthCheckResult;
        }
    }
}
