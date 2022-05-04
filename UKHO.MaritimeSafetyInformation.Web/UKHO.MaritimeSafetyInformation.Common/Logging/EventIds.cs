using Microsoft.Extensions.Logging;

namespace UKHO.MaritimeSafetyInformation.Common.Logging
{
    public enum EventIds
    {
        /// <summary>
        /// 910001 - An unhandled exception occurred while processing the request.
        /// </summary>
        UnhandledControllerException = 910001,
        /// <summary>
        /// 910002 - Maritime safety information request started.
        /// </summary>
        Start = 910002,
        /// <summary>
        /// 910003 - Event hub for Maritime safety information is healthy.
        /// </summary>
        EventHubLoggingIsHealthy = 910003,
        /// <summary>
        /// 910004 - Event hub for Maritime safety information is unhealthy.
        /// </summary>
        EventHubLoggingIsUnhealthy = 910004,
        /// <summary>
        /// 910005 -  Event data for Maritime safety information event hub health check.
        /// </summary>
        EventHubLoggingEventDataForHealthCheck = 910005,
        /// <summary>
        /// 910006 -  Maritime safety information create new RNW record request started.
        /// </summary>
        MSICreateNewRNWRecordStart = 910006,
        /// <summary>
        /// 910007 -  Maritime safety information create new RNW record request Completed.
        /// </summary>
        MSICreateNewRNWRecordCompleted = 910007,
        /// <summary>
        /// 910008 -  Maritime safety information add new RNW record to database request started.
        /// </summary>
        MSIAddNewRNWRecordStart = 910008,
        /// <summary>
        /// 910009 -  Maritime safety information add new RNW record to database request Completed.
        /// </summary>
        MSIAddNewRNWRecordCompleted = 910009,
        /// <summary> 
        /// 910010 -  Maritime safety information add new RNW record to database request error.
        /// </summary>
        MSIAddNewRNWRequestError = 910010
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
