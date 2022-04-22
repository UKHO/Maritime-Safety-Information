
namespace UKHO.MaritimeSafetyInformation.Common.Logging
{
    public enum EventIds
    {
        /// <summary>
        /// 910001 - Event hub for MSI is healthy.
        /// </summary>
        EventHubLoggingIsHealthy = 910001,
        /// <summary>
        /// 910002 - Event hub for MSI is unhealthy.
        /// </summary>
        EventHubLoggingIsUnhealthy = 910002,
        /// <summary>
        /// 910003 -  Event data for MSI event hub health check.
        /// </summary>
        EventHubLoggingEventDataForHealthCheck = 910003,
    }
}
