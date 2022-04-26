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

        RetrievalOfMSIShowWeeklyFilesRequest = 910006,
        RetrievalOfMSIShowWeeklyFilesCompleted = 910007,
        RetrievalOfMSIShowFilesResponseStarted = 910008,
        RetrievalOfMSIShowFilesResponseDataFound = 910009,
        RetrievalOfMSIShowFilesResponseDataFoundNotFound = 910010,
        RetrievalOfMSIShowFilesResponseFailed = 910011,
        RetrievalOfMSIGetPastYearsStart = 910012,
        RetrievalOfMSIGetPastYearsFailed = 910013,
        RetrievalOfMSIGetAllWeeksofYearStart = 910014,
        RetrievalOfMSIGetAllWeeksofYearFailed = 910015,
        RetrievalOfMSIBatchSearchResponse = 910016,
        RetrievalOfMSIBatchSearchResponseFailed = 910017,
        RetrievalOfMSIFailed = 10019,
        UnhandledCleanUpException = 10071
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
