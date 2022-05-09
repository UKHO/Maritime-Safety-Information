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
        /// 910006 -  Notices To Mariners Weekly File Request Started.
        /// </summary>
        NoticesToMarinersWeeklyFilesRequestStarted = 910006,
        /// <summary>
        /// 910007 -  Notices To Mariners Weekly File Request Started.
        /// </summary>
        NoticesToMarinersWeeklyFilesRequestCompleted = 910007,
        /// <summary>
        /// 910008 -  Get NM Batch Files Request Started.
        /// </summary>
        GetWeeklyNMFilesRequestStarted = 910008,
        /// <summary>
        /// 910009 -  Get NM Batch Files Data Found.
        /// </summary>
        GetWeeklyNMFilesRequestDataFound = 910009,
        /// <summary>
        /// 910010 -  Get NM Batch Files Not Data Found.
        /// </summary>
        GetWeeklyNMFilesRequestDataNotFound = 910010,
        /// <summary>
        /// 910011 -  Get NM Batch Files Request Failed.
        /// </summary>
        GetWeeklyNMFilesRequestFailed = 910011,
        /// <summary>
        /// 910012 -  FSS Batch Search Request Started.
        /// </summary>
        FSSBatchSearchResponseStarted = 910012,
        /// <summary>
        /// 910013 -  FSS Batch Search Request Started.
        /// </summary>
        FSSBatchSearchResponseCompleted = 910013,
        /// <summary>
        /// 910014 -  FSS Batch Search Request Failed.
        /// </summary>
        FSSBatchSearchResponseFailed = 910014,
        /// <summary>
        /// 910015 -  Unhandled Exception Occured.
        /// </summary>
        UnhandledCleanUpException = 910015,
        /// <summary>
        /// 910016 -  Search Attribute Response Started for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseStarted = 910016,
        /// <summary>
        /// 910017 - Search Attribute Response Completed for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseCompleted = 910017,
        /// <summary>
        /// 910018 - Search Attribute Response threw an exception in case of errors for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseError = 910018,        
        /// <summary>
        /// 910019 -  Get All weeks and Year data for Notices to Mariners Started
        /// </summary>
        NoticesToMarinersGetAllYearsandWeeksStarted = 910019,
        /// <summary>
        /// 910020 -  Get All weeks and Year data for Notices to Mariners Completed
        /// </summary>
        NoticesToMarinersGetAllYearsandWeeksCompleted = 910020,
        /// <summary>
        /// 910021 - Recieved Year Week data from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataStarted = 910021,
        /// <summary>
        /// 910022 - Recieved Year Week data from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataFound = 910022,
        /// <summary>
        /// 910023 - No data recieved Year Week data from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataNotFound = 910023,
        /// <summary>
        /// 910024 - Failed to laod data for Year Week from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataFailed = 910024
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
