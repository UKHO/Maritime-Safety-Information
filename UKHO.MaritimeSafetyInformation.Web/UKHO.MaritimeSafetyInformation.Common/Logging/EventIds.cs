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
        /// 910015 -  Event data for Retrieval of MSI Daily File Request.
        /// </summary>
        ShowDailyFilesRequest = 910015,
        /// <summary>
        /// 910016 -  Event data for Retrieval of MSI Daily File Completed.
        /// </summary>
        ShowDailyFilesCompleted = 910016,
        /// <summary>
        /// 910017 -  Event data for Retrieval of MSI Daily File Response Started.
        /// </summary>
        ShowDailyFilesResponseStarted = 910017,
        /// <summary>
        /// 910018 -  Event data for Retrieval of MSI Daily File Response Data Found.
        /// </summary>
        ShowDailyFilesResponseDataFound = 910018,
        /// <summary>
        /// 910019-  Event data for Retrieval of MSI Daily File Response Data Not Found.
        /// </summary>
        ShowDailyFilesResponseDataNotFound = 910019,
        /// <summary>
        /// 910020-  Event data for Retrieval of MSI Daily File Response Data Not Found.
        /// </summary>
        ShowDailyFilesResponseFailed = 910020,
        /// <summary>
        /// 910021-  Event data for Retrieval of MSI Get Daily File Result.
        /// </summary>
        GetDailyFilesResultRequest = 910021,
        /// <summary>
        /// 910022-  Event data for Retrieval of MSI Get Daily File Result Completed.
        /// </summary>
        GetDailyFilesResultCompleted = 910022,
        /// <summary>
        /// 910023-  Event data for Retrieval of MSI Get Daily File Result Completed.
        /// </summary>
        GetWeeklyFilesResultRequest = 910023,
        /// <summary>
        /// 910024-  Event data for Retrieval of MSI Get Daily File Result Completed.
        /// </summary>
        GetWeeklyFilesResultRequestCompleted = 910024,
        /// <summary>
        /// 910025 -  Unhandled Exception Occured.
        /// </summary>
        UnhandledCleanUpException = 910025,
        /// <summary>
        /// 910026 -  Search Attribute Response Started for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseStarted = 910026,
        /// <summary>
        /// 910027 - Search Attribute Response Completed for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseCompleted = 910027,
        /// <summary>
        /// 910028 - Search Attribute Response threw an exception in case of errors for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseError = 910028,
        /// <summary>
        /// 910029 -  Get All weeks and Year data for Notices to Mariners Started
        /// </summary>
        NoticesToMarinersGetAllYearsandWeeksStarted = 910029,
        /// <summary>
        /// 910030 -  Get All weeks and Year data for Notices to Mariners Completed
        /// </summary>
        NoticesToMarinersGetAllYearsandWeeksCompleted = 910030,
        /// <summary>
        /// 910031 - Recieved Year Week data from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataStarted = 910031,
        /// <summary>
        /// 910032 - Recieved Year Week data from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataFound = 910032,
        /// <summary>
        /// 910033 - No data recieved Year Week data from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataNotFound = 910033,
        /// <summary>
        /// 910034 - Failed to laod data for Year Week from FSS for Notices to Mariners 
        /// </summary>
        GetSearchAttributeRequestDataFailed = 910034
    }


    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
