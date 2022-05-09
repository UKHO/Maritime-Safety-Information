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
        /// 910012 -  Get Past Years Request Started.
        /// </summary>
        GetAllYearsStarted = 910012,
        /// <summary>
        /// 910013 -  Get Past Years Request Failed.
        /// </summary>
        GetPastYearsFailed = 910013,
        /// <summary>
        /// 910014 -  Get All Weeks of Year Request Started.
        /// </summary>
        GetAllWeeksOfYearStarted = 910014,
        /// <summary>
        /// 910015 -  Get All Weeks of Year Request Failed.
        /// </summary>
        GetAllWeeksofYearFailed = 910015,
        /// <summary>
        /// 910016 -  FSS Batch Search Request Started.
        /// </summary>
        FSSBatchSearchResponseStarted = 910016,
        /// <summary>
        /// 910017 -  FSS Batch Search Request Started.
        /// </summary>
        FSSBatchSearchResponseCompleted = 910017,
        /// <summary>
        /// 910018 -  FSS Batch Search Request Failed.
        /// </summary>
        FSSBatchSearchResponseFailed = 910018,
		/// <summary>
        /// 910019 -  Event data for Retrieval of MSI Daily File Request.
        /// </summary>
        MSIShowDailyFilesRequest = 910019,
        /// <summary>
        /// 910020 -  Event data for Retrieval of MSI Daily File Completed.
        /// </summary>
        MSIShowDailyFilesCompleted = 910020,
        /// <summary>
        /// 910021 -  Event data for Retrieval of MSI Daily File Response Started.
        /// </summary>
        MSIShowDailyFilesResponseStarted = 910021,
        /// <summary>
        /// 910022 -  Event data for Retrieval of MSI Daily File Response Data Found.
        /// </summary>
        MSIShowDailyFilesResponseDataFound = 910022,
        /// <summary>
        /// 910023-  Event data for Retrieval of MSI Daily File Response Data Not Found.
        /// </summary>
        MSIShowDailyFilesResponseDataNotFound = 910023,
        /// <summary>
        /// 910024-  Event data for Retrieval of MSI Daily File Response Data Not Found.
        /// </summary>
        MSIShowDailyFilesResponseFailed = 910024,
        /// <summary>
        /// 910025-  Event data for Retrieval of MSI Get Daily File Result.
        /// </summary>
        MSIGetDailyFilesResultRequest = 910025,
        /// <summary>
        /// 910026-  Event data for Retrieval of MSI Get Daily File Result Completed.
        /// </summary>
        MSIGetDailyFilesResultCompleted = 910026,
        /// <summary>
        /// 910027-  Event data for Retrieval of MSI Get Daily File Result Completed.
        /// </summary>
        MSIGetWeeklyFilesResultRequest = 910027,
        /// <summary>
        /// 910028-  Event data for Retrieval of MSI Get Daily File Result Completed.
        /// </summary>
        MSIGetWeeklyFilesResultRequestCompleted = 910028,
        /// <summary>
        /// 910029 -  Unhandled Exception Occured.
        /// </summary>
        UnhandledCleanUpException = 910029
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
