﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public class RNWDatabaseHealthCheck : IHealthCheck
    {
        private readonly IRNWDatabaseHealthClient _rnwDatabaseHealthClient;
        private readonly ILogger<RNWDatabaseHealthCheck> _logger;

        public RNWDatabaseHealthCheck(IRNWDatabaseHealthClient rnwDatabaseHealthClient, ILogger<RNWDatabaseHealthCheck> logger)
        {
            _rnwDatabaseHealthClient = rnwDatabaseHealthClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult healthCheckResult = await _rnwDatabaseHealthClient.CheckHealthAsync(cancellationToken);
            if (healthCheckResult.Status == HealthStatus.Healthy)
            {
                _logger.LogDebug(EventIds.RNWDatabaseIsHealthy.ToEventId(), "Radio Navigational Warnings database is healthy");
            }
            else
            {
                _logger.LogError(EventIds.RNWDatabaseIsUnHealthy.ToEventId(), healthCheckResult.Exception, "Radio Navigational Warnings database is unhealthy responded with error {Message}", healthCheckResult.Exception.Message);
            }
            return healthCheckResult;
        }
    }
}
