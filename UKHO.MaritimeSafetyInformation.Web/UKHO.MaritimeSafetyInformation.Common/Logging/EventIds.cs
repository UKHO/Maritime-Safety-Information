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
        /// 910026 -  Maritime safety information create new RNW record request started.
        /// </summary>
        CreateNewRNWRecordStart = 910026,
        /// <summary>
        /// 910027 -  Maritime safety information create new RNW record request Completed.
        /// </summary>
        CreateNewRNWRecordCompleted = 910027,
        /// <summary>
        /// 910028 -  Maritime safety information add new RNW record to database request started.
        /// </summary>
        AddNewRNWRecordStart = 910028,
        /// <summary>
        /// 910029 -  Maritime safety information add new RNW record to database request Completed.
        /// </summary>
        AddNewRNWRecordCompleted = 910029,
        /// <summary> 
        /// 910030 -  Maritime safety information error has occurred in the process to add new RNW record to database.
        /// </summary>
        EditRNWRecordException = 910030,
        /// <summary> 
        /// 910031 -  Maritime safety information invalid new RNW record request.
        /// </summary>
        InvalidNewRNWRecordRequest = 910031,
        /// <summary> 
        /// 910032 -  Maritime safety information invalid value received for parameter warningType.
        /// </summary>
        InvalidWarningTypeInRequest = 910032,
        /// <summary> 
        /// 910033 -  Maritime safety information invalid value received for parameter reference.
        /// </summary>
        InvalidReferenceInRequest = 910033,
        /// <summary> 
        /// 910034 -  Maritime safety information invalid value received for parameter summary.
        /// </summary>
        InvalidSummaryInRequest = 910034,
        /// <summary> 
        /// 910035 -  Maritime safety information invalid value received for parameter content.
        /// </summary>
        InvalidContentInRequest = 910035,
        /// <summary> 
        /// 910036 -  Maritime safety information request to get RNW records for Admin started.
        /// </summary>
        RNWAdminListStarted = 910036,
        /// <summary> 
        /// 910037 -  Maritime safety information request to get RNW records for Admin completed.
        /// </summary>
        RNWAdminListCompleted = 910037,
        /// <summary> 
        /// 910038 -  Maritime safety information get RNW records for Admin from database request error.
        /// </summary>
        RNWAdminListError = 910038,
        /// <summary>
        /// 910039 -  Event data for Retrieval of MSI Get Weekly File Response Started.
        /// </summary>
        GetWeeklyFilesResponseStarted = 910039,
        /// <summary>
        /// 910040 -  Event data for Retrieval of MSI Get Weekly File Response Failed.
        /// </summary>
        GetWeeklyFilesResponseFailed = 910040,
        /// <summary>
        /// 910041 -  Event data for Retrieval of MSI Get Weekly File Index Get Response Failed.
        /// </summary>
        ShowWeeklyFilesIndexGetFailed = 910041,
        /// <summary>
        /// 910042 -  Event data for Retrieval of MSI Get Weekly File Index Post Response Failed.
        /// </summary>
        ShowWeeklyFilesIndexPostFailed = 910042,
        /// <summary>
        /// 910043 -  Event data for Retrieval of MSI Show Weekly File Response For Index Post Started.
        /// </summary>
        ShowWeeklyFilesResponseStartIndexPost = 910043,
        /// <summary>
        /// 910044 -  Event data for Retrieval of MSI Show Weekly File Response For Index Post Completed.
        /// </summary>
        ShowWeeklyFilesResponseIndexPostCompleted = 910044,
        /// <summary>
        /// 910045 -  Event data for Retrieval of MSI Show Weekly File Response For Index Get Completed.
        /// </summary>
        ShowWeeklyFilesResponseIndexGetCompleted = 910045,
        /// <summary>
        /// 910046 - Event data for Get Search Attribute data from FSS for GetAllYearWeek Started
        /// </summary>
        GetSearchAttributeRequestDataStarted = 910046,
        /// <summary>
        /// 910047 - Event data for Get Search Attribute data from FSS for GetAllYearWeek Not found
        /// </summary>
        GetSearchAttributeRequestDataFound = 910047,
        /// <summary>
        /// 910048 - Event data for Get Search Attribute data from FSS for GetAllYearWeek Failed
        /// </summary>
        GetSearchAttributeRequestDataFailed = 910048,
        /// <summary>
        /// 910049 -  Search Attribute Response Started for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseStarted = 910049,
        /// <summary>
        /// 910050 -  Search Attribute Response Completed for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseCompleted = 910050,
        /// <summary>
        /// 910051 - Search Attribute Response threw an exception in case of errors for File Share Service Client
        /// </summary>
        FSSSearchAttributeResponseError = 910051,
        /// <summary>
        /// 910052 - No data received Year Week data from FSS for Notices to Mariners  
        /// </summary>
        GetSearchAttributeRequestDataNotFound = 910052,
        /// <summary>
        /// 910053 -  Event data for Retrieval of MSI Get Weekly File Response Started.
        /// </summary>
        GetWeeklyFilesResponseCompleted = 910053,

        /// <summary>
        /// 910054 -Request For Download Single Weekly NM File Started.
        /// </summary>
        DownloadSingleWeeklyNMFileStarted = 910054,

        /// <summary>
        /// 910055 -Request For Download Single Weekly NM File Completed.
        /// </summary>
        DownloadSingleWeeklyNMFileCompleted= 910055,

        /// <summary>
        /// 910056 -Download Single Weekly NM File Called With Invalid Arguments.
        /// </summary>
        DownloadSingleWeeklyNMFileInvalidParameter = 910056,

        /// <summary>
        /// 910057 -Download Single Weekly NM File Failed.
        /// </summary>
        DownloadSingleWeeklyNMFileFailed = 910057,

        /// <summary>
        /// 910058 - Request To Get Single Weekly NM File Started.
        /// </summary>
        GetSingleWeeklyNMFileStarted = 910058,

        /// <summary>
        /// 910059 - Request To Get Single Weekly NM File Completed.
        /// </summary>
        GetSingleWeeklyNMFileCompleted = 910059,

        /// <summary>
        /// 910060 - Request To Get Single Weekly NM File Failed.
        /// </summary>
        GetSingleWeeklyNMFileFailed = 910060,

        /// <summary>
        /// 910061 - Request For FSS To Get Single Weekly NM File Started.
        /// </summary>
        FSSGetSingleWeeklyNMFileStarted = 910061,

        /// <summary>
        /// 910062 - Request For FSS To Get Single Weekly NM File Completed.
        /// </summary>
        FSSGetSingleWeeklyNMFileCompleted = 910062,

        /// <summary>
        /// 910063 - Request For FSS To Get Single Weekly NM File Failed.
        /// </summary>
        FSSGetSingleWeeklyNMFileResponseFailed = 910063,
        /// <summary> 
        /// 910064 -  Maritime safety information request to get RNW detail started.
        /// </summary>
        RNWListDetailStarted = 910064,
        /// <summary> 
        /// 910065 -  Maritime safety information request to get RNW detail completed.
        /// </summary>
        RNWListDetailCompleted = 910065,
        /// <summary> 
        /// 910066 -  Maritime safety information request to get RNW detail from database started.
        /// </summary>
        RNWListDetailFromDatabaseStarted = 910066,
        /// <summary> 
        /// 910067 -  Maritime safety information request to get RNW detail from database Completed.
        /// </summary>
        RNWListDetailFromDatabaseCompleted = 910067,
        /// <summary> 
        /// 910068 -  Maritime safety information error has occurred in the process to get RNW detail from database.
        /// </summary>
        ErrorInRNWListDetailFromDatabase = 910068,
        /// <summary> 
        /// 910069 -  Maritime safety information request to get last modified date time from database started.
        /// </summary>
        RNWLastModifiedDateTimeFromDatabaseStarted = 910069,
        /// <summary> 
        /// 910070 -  Maritime safety information request to get last modified date time from database completed.
        /// </summary>
        RNWLastModifiedDateTimeFromDatabaseCompleted = 910070,
        /// <summary> 
        /// 910071 -  Maritime safety information Edit RNW records for Admin completed.
        /// </summary>
        EditRNWRecordCompleted = 910071,
        /// <summary> 
        /// 910072 -  Maritime safety information Edit RNW record started.
        /// </summary>
        EditRNWRecordStarted = 910072,
        /// <summary> 
        /// 910073 - Maritime safety information edit RNW record Id not found.
        /// </summary>
        EditRNWRecordIDNotFound = 910073,
        /// <summary> 
        /// 910074 - Maritime safety information edit RNW record Id mismatch.
        /// </summary>
        EditRNWRecordIdMismatch = 910074,
        /// <summary> 
        /// 910075 - Maritime safety information edit RNW list is null.
        /// </summary>
        EditRNWRecordNotFound = 910075,
        /// <summary> 
        /// 910076 - Maritime safety information Error in retrieving RNW record.
        /// </summary>
        ErrorInRetrievingRNWRecord = 910076

    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
