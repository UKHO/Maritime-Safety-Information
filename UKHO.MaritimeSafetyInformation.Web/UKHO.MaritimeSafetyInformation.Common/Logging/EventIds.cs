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
        /// 910007 -  Notices To Mariners Weekly File Request Completed.
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
        /// 910054 -Request For Download Single NM File Started.
        /// </summary>
        DownloadSingleNMFileStarted = 910054,

        /// <summary>
        /// 910055 -Request For Download Single NM File Completed.
        /// </summary>
        DownloadSingleNMFileCompleted = 910055,

        /// <summary>
        /// 910056 -Download Single NM File Called With Invalid Arguments.
        /// </summary>
        DownloadSingleNMFileInvalidParameter = 910056,

        /// <summary>
        /// 910057 -Download Single NM File Failed.
        /// </summary>
        DownloadSingleNMFileFailed = 910057,

        /// <summary>
        /// 910058 - Request To Get Single NM File Started.
        /// </summary>
        GetSingleNMFileStarted = 910058,

        /// <summary>
        /// 910059 - Request To Get Single NM File Completed.
        /// </summary>
        GetSingleNMFileCompleted = 910059,

        /// <summary>
        /// 910060 - Request To Get Single NM File Failed.
        /// </summary>
        GetSingleNMFileFailed = 910060,

        /// <summary>
        /// 910061 - Request For FSS To Get Single NM File Started.
        /// </summary>
        FSSGetSingleNMFileStarted = 910061,

        /// <summary>
        /// 910062 - Request For FSS To Get Single NM File Completed.
        /// </summary>
        FSSGetSingleNMFileCompleted = 910062,

        /// <summary>
        /// 910063 - Request For FSS To Get Single NM File Failed.
        /// </summary>
        FSSGetSingleNMFileResponseFailed = 910063,
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
        /// 910071 -Request For Download Daily NM File Started.
        /// </summary>
        DownloadDailyNMFileStarted = 910071,

        /// <summary>
        /// 910072 -Request For Download Daily NM File Completed.
        /// </summary>
        DownloadDailyNMFileCompleted = 910072,

        /// <summary>
        /// 910073 -Request For Download Daily NM File Failed.
        /// </summary>
        DownloadDailyNMFileFailed = 910073,

        /// <summary>
        /// 910074 - Request To Get Daily Zip NM File Started.
        /// </summary>
        GetDailyZipNMFileStarted = 910074,

        /// <summary>
        /// 910075 - Request To Get Daily Zip NM File Completed.
        /// </summary>
        GetDailyZipNMFileCompleted = 910075,

        /// <summary>
        /// 910076 - Request To Get Daily Zip NM File Failed.
        /// </summary>
        GetDailyZipNMFileFailed = 910076,

        /// <summary>
        /// 910077 - Request For FSS To Get Daily Zip NM File Started.
        /// </summary>
        FSSGetDailyZipNMFileStarted = 910077,

        /// <summary>
        /// 910078 - Request For FSS To Get Daily Zip NM File Completed.
        /// </summary>
        FSSGetDailyZipNMFileCompleted = 910078,

        /// <summary>
        /// 910079 - Request For FSS To Get Daily Zip NM File Failed.
        /// </summary>
        FSSGetDailyZipNMFileResponseFailed = 910079,

        /// <summary>
        /// 910080-  Event data for Retrieval of MSI Show Daily File Failed.
        /// </summary>
        ShowDailyFilesFailed = 910080,

        /// <summary>
        /// 910081-  Request For FSS To Get Daily Zip NM File Return IsSuccess False.
        /// </summary>
        FSSGetDailyZipNMFileReturnIsSuccessFalse = 910081,

        /// <summary>
        /// 910082-  Request For FSS To Get Daily Zip NM File Has Error.
        /// </summary>
        FSSDownloadZipFileAsyncHasError = 910082,
        /// <summary> 
        /// 910083 -  Maritime safety information Edit RNW records for Admin completed.
        /// </summary>
        EditRNWRecordCompleted = 910083,
        /// <summary> 
        /// 910084 -  Maritime safety information Edit RNW record started.
        /// </summary>
        EditRNWRecordStarted = 910084,
        /// <summary> 
        /// 910085 - Maritime safety information edit RNW record Id not found.
        /// </summary>
        EditRNWRecordIDNotFound = 910085,
        /// <summary> 
        /// 910086 - Maritime safety information edit RNW record Id mismatch.
        /// </summary>
        EditRNWRecordIdMismatch = 910086,
        /// <summary> 
        /// 910087 - Maritime safety information edit RNW list is null.
        /// </summary>
        EditRNWRecordNotFound = 910087,
        /// <summary> 
        /// 910088 - Maritime safety information Error in retrieving RNW record.
        /// </summary>
        ErrorInRetrievingRNWRecord = 910088,
        /// <summary> 
        /// 910089 - Maritime safety information Error in Creating RNW record.
        /// </summary>
        CreateRNWRecordException = 910089,
        /// <summary> 
        /// 910090 -  Maritime safety information request to show RNW details from database started.
        /// </summary>
        RNWShowListDetailFromDatabaseStarted = 910090,
        /// <summary> 
        /// 910091 -  Maritime safety information request to show RNW details from database completed.
        /// </summary>
        RNWShowListDetailFromDatabaseCompleted = 910091,
        /// <summary> 
        /// 910092 -  Maritime safety information error has occurred in the process to show RNW detail from database.
        /// </summary>
        ErrorInRNWShowListDetailFromDatabase = 910092,
        /// <summary>
        /// 910093 - Radio Navigational Warning database is healthy.
        /// </summary>
        RNWDatabaseIsHealthy = 910093,
        /// <summary>
        /// 910094 - Radio Navigational Warning database is unhealthy.
        /// </summary>
        RNWDatabaseIsUnHealthy = 910094,
        /// <summary>
        /// 910095 - File share service is healthy.
        /// </summary>
        FileShareServiceIsHealthy = 910095,
        /// <summary>
        /// 910096 - File share service is unhealthy.
        /// </summary>
        FileShareServiceIsUnHealthy = 910096,
        /// <summary>
        /// 910097 -  Notices To Mariners Weekly File Request failed.
        /// </summary>
        NoticesToMarinersWeeklyFilesRequestFailed = 910097,
        /// <summary>
        /// 910098 -  System Error.
        /// </summary>
        SystemError = 910098,
        /// <summary>
        /// 910099 -  Unauthorized Access.
        /// </summary>
        UnauthorizedAccess = 910099,
        /// <summary>
        /// 910100 -   Event data for Retrieval of MSI Cumulative File Request Started.
        /// </summary>
        ShowCumulativeFilesRequestStarted = 910100,
        /// <summary>
        /// 910101 -   Event data for Retrieval of MSI Cumulative File Request Completed.
        /// </summary>
        ShowCumulativeFilesRequestCompleted = 910101,
        /// <summary>
        /// 910102 -    Event data for Retrieval of MSI Show Cumulative File Failed.
        /// </summary>
        ShowCumulativeFilesFailed = 910102,
        /// <summary>
        /// 910103 -  Event data for Retrieval of MSI Get Cumulative File Response Started.
        /// </summary>
        GetCumulativeFilesResponseStarted = 910103,
        /// <summary>
        /// 910104 -  Get NM Batch Files Data Not Found.
        /// </summary>
        GetCumulativeNMFilesRequestDataNotFound = 910104,
        /// <summary>
        /// 910105 -  Event data for Retrieval of MSI Get Cumulative File Response Completed.
        /// </summary>
        GetCumulativeFilesResponseCompleted = 910105,
        /// <summary>
        /// 910106 -  Event data for Retrieval of MSI Get Cumulative File Response Failed.
        /// </summary>
        GetCumulativeFilesResponseFailed = 910106,


        /// <summary>
        /// 910107 -  Event data for Retrieval of Leisure file Started.
        /// </summary>
        ShowLeisureFilesRequestStarted = 910107,
        /// <summary>
        /// 910108 -  Event data for Retrieval of Leisure file Completed.
        /// </summary>
        ShowLeisureFilesRequestCompleted = 910108,
        /// <summary>
        /// 910109 -  Event data for Retrieval of Leisure file Failed.
        /// </summary>
        ShowLeisureFilesRequestFailed = 910109,
        /// <summary>
        /// 910110 -  Event data for Retrieval of Leisure file Response Started.
        /// </summary>
        ShowLeisureFilesResponseStarted = 910110,
        /// <summary>
        /// 910111 -  Event data for Retrieval of Leisure file Response Completed and Data Found.
        /// </summary>
        ShowLeisureFilesResponseDataFound = 910111,
        /// <summary>
        /// 910112 -  Event data for Retrieval of Leisure file Response Completed and Data Found.
        /// </summary>
        ShowLeisureFilesResponseDataNotFound = 910112,
        /// <summary>
        /// 910113 -  Event data for Retrieval of Leisure file Response Failed.
        /// </summary>
        ShowLeisureFilesResponseFailed = 910113,

    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
