using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UKHO.MaritimeSafetyInformation.Web.HealthCheck
{
    public class EventHubLoggingHealthCheck :IHealthCheck
    {
        private readonly IEventHubLoggingHealthClient _eventHubLoggingHealthClient;

        public EventHubLoggingHealthCheck(IEventHubLoggingHealthClient eventHubLoggingHealthClient)
        {
            _eventHubLoggingHealthClient = eventHubLoggingHealthClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult healthCheckResult = await _eventHubLoggingHealthClient.CheckHealthAsync(context, cancellationToken);
            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                //_logger.LogDebug(EventIds.EventHubLoggingIsHealthy.ToEventId(), "Event hub is healthy");
            }
            else
            {
                //_logger.LogError(EventIds.EventHubLoggingIsUnhealthy.ToEventId(), healthCheckResult.Exception, "Event hub is unhealthy responded with error {Message}", healthCheckResult.Exception.Message);
            }
            return healthCheckResult;
        }
    }
}
