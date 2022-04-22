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
        RetrievalOfMSIShowWeeklyFilesRequest = 910003,
        RetrievalOfMSIShowWeeklyFilesCompleted = 910003,
        RetrievalOfMSIShowFilesResponseStarted = 910004,
        RetrievalOfMSIShowFilesResponseDataFound = 910005,
        RetrievalOfMSIShowFilesResponseDataFoundNotFound = 910006,
        RetrievalOfMSIShowFilesResponseFailed = 910007,
        RetrievalOfMSIGetPastYearsStart = 910008,
        RetrievalOfMSIGetPastYearsFailed = 910009,
        RetrievalOfMSIGetAllWeeksofYearStart = 910010,
        RetrievalOfMSIGetAllWeeksofYearFailed = 910011,
        RetrievalOfMSIBatchSearchResponse = 910012,
        RetrievalOfMSIBatchSearchResponseFailed = 910013,
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
