﻿using Microsoft.Extensions.Logging;

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
        /// 910053 -  Event data for Retrieval of MSI Get Weekly File Response Completed.
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
        /// 910074 - Request To Get NM Zip File Started.
        /// </summary>
        GetNMZipFileStarted = 910074,

        /// <summary>
        /// 910075 - Request To Get NM Zip File Completed.
        /// </summary>
        GetNMZipFileCompleted = 910075,

        /// <summary>
        /// 910076 - Request To Get NM Zip File Failed.
        /// </summary>
        GetNMZipFileFailed = 910076,

        /// <summary>
        /// 910077 - Request For FSS To Get NM Zip File Started.
        /// </summary>
        FSSGetNMZipFileStarted = 910077,

        /// <summary>
        /// 910078 - Request For FSS To Get NM Zip File Completed.
        /// </summary>
        FSSGetNMZipFileCompleted = 910078,

        /// <summary>
        /// 910079 - Request For FSS To Get NM Zip File Failed.
        /// </summary>
        FSSGetNMZipFileResponseFailed = 910079,

        /// <summary>
        /// 910080-  Event data for Retrieval of MSI Show Daily File Failed.
        /// </summary>
        ShowDailyFilesFailed = 910080,

        /// <summary>
        /// 910081-  Request For FSS To Get NM Zip File Return IsSuccess False.
        /// </summary>
        FSSGetNMZipFileReturnIsSuccessFalse = 910081,

        /// <summary>
        /// 910082 -  Request For FSS To Get NM Zip File Has Error.
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
        /// 910114 - Request for searching all year week from cache azure table storage is started.
        /// </summary>
        FSSSearchAllYearWeekFromCacheStart = 910114,
        /// <summary>
        /// 910115 - Request for searching all year week from cache azure table storage is completed.
        /// </summary>
        FSSSearchAllYearWeekFromCacheCompleted = 910115,
        /// <summary>
        /// 910116 - Data not found for searching all year week from cache azure table storage.
        /// </summary>
        FSSSearchAllYearWeekDataNotFoundFromCache = 910116,
        /// <summary>
        /// 910117- Request for searching weekly batch files from cache azure table storage is started.
        /// </summary>
        FSSSearchWeeklyBatchResponseFromCacheStart = 910117,
        /// <summary>
        /// 910118 - Request for searching weekly batch files from cache azure table storage is completed.
        /// </summary>
        FSSSearchWeeklyBatchResponseFromCacheCompleted = 910118,
        /// <summary>
        /// 910119 - Data not found for searching weekly batch files from cache azure table storage.
        /// </summary>
        FSSSearchWeeklyBatchResponseDataNotFoundFromCache = 910119,
        /// <summary>
        /// 910120 - Request for storing file share service search all year week response in azure table storage is started.
        /// </summary>
        FSSSearchAllYearWeekResponseStoreToCacheStart = 910120,
        /// <summary>
        /// 910121 - Request for storing file share service search all year week response in azure table storage is completed.
        /// </summary>
        FSSSearchAllYearWeekResponseStoreToCacheCompleted = 910121,
        /// <summary>
        /// 910122 - Request for storing file share service search weekly batch files response in azure table storage is started.
        /// </summary>
        FSSSearchWeeklyBatchFilesResponseStoreToCacheStart = 910122,
        /// <summary>
        /// 910123 - Request for storing file share service search weekly batch files response in azure table storage is completed.
        /// </summary>
        FSSSearchWeeklyBatchFilesResponseStoreToCacheCompleted = 910123,
        /// <summary>
        /// 910124 - Deletion started for expired all year and week cache data from table.
        /// </summary>
        DeleteExpiredYearWeekCacheDataFromTableStarted = 910124,
        /// <summary>
        /// 910125 - Deletion completed for expired all year and week cache data from table.
        /// </summary>
        DeleteExpiredYearWeekCacheDataFromTableCompleted = 910125,
        /// <summary>
        /// 910126 - Deletion started for expired searching weekly NM file cache data from table.
        /// </summary>
        DeleteExpiredSearchWeeklyBatchResponseFromCacheStarted = 910126,
        /// <summary>
        /// 910127 - Deletion completed for expired searching weekly NM file cache data from table.
        /// </summary>
        DeleteExpiredSearchWeeklyBatchResponseFromCacheCompleted = 910127,
        /// <summary>
        /// 910128 - Failed to get searching attribute year and week data from cache azure table.
        /// </summary>
        FSSSearchAllYearWeekFromCacheFailed = 910128,
        /// <summary>
        /// 910129 - Failed to get searching weekly NM files from cache azure table.
        /// </summary>
        FSSSearchWeeklyBatchResponseFromCacheFailed = 910129,
        /// <summary>
        /// 910130 - Process failed to insert entity value in cache table.
        /// </summary>
        FSSCacheDataInsertFailed = 910130,
        /// <summary>
        /// 910131 - Started processing the Options request for the New Files Published event webhook
        /// </summary>
        NewFilesPublishedWebhookOptionsCallStarted = 910131,
        /// <summary>
        /// 910132 - Completed processing the Options request for the New Files Published event webhook
        /// </summary>
        NewFilesPublishedWebhookOptionsCallCompleted = 910132,
        /// <summary>
        /// 910133 -  Request for clearing FSS search cache data from Azure table started.
        /// </summary>
        ClearFSSSearchCacheEventStarted = 910133,
        /// <summary>
        /// 910134 - Request for storing file share service search cumulative batch files response in azure table storage is started.
        /// </summary>
        FSSSearchCumulativeBatchFilesResponseStoreToCacheStart = 910134,
        /// <summary>
        /// 910135 - Request for storing file share service search cumulative batch files response in azure table storage is completed.
        /// </summary>
        FSSSearchCumulativeBatchFilesResponseStoreToCacheCompleted = 910135,
        /// <summary>
        /// 910136- Request for searching batch files from cache azure table storage is started.
        /// </summary>
        FSSSearchBatchResponseFromCacheStart = 910136,
        /// <summary>
        /// 910137 - Request for searching batch files from cache azure table storage is completed.
        /// </summary>
        FSSSearchBatchResponseFromCacheCompleted = 910137,
        /// <summary>
        /// 910138 - Deletion started for expired searching NM file cache data from table.
        /// </summary>
        DeleteExpiredSearchBatchResponseFromCacheStarted = 910138,
        /// <summary>
        /// 910139 - Deletion completed for expired searching NM file cache data from table.
        /// </summary>
        DeleteExpiredSearchBatchResponseFromCacheCompleted = 910139,
        /// <summary>
        /// 910140 - Data not found for searching batch files from cache azure table storage.
        /// </summary>
        FSSSearchBatchResponseDataNotFoundFromCache = 910140,
        /// <summary>
        /// 910141 - Failed to get searching NM files from cache azure table.
        /// </summary>
        FSSSearchBatchResponseFromCacheFailed = 910141,
        /// <summary>
        /// 910142 - Request for storing file share service daily files response in azure table storage is started.
        /// </summary>
        FSSDailyBatchFilesResponseStoreToCacheStart = 910142,
        /// <summary>
        /// 910143 - Request for storing file share service daily files response in azure table storage is completed.
        /// </summary>
        FSSDailyBatchFilesResponseStoreToCacheCompleted = 910143,
        /// <summary>
        /// 910146 - Request for clearing FSS search cache data from Azure table completed.
        /// </summary>
        ClearFSSSearchCacheEventCompleted = 910146,
        /// <summary>
        /// 910147 - Request for validation event for clearing FSS search cache from Azure table
        /// </summary>
        ClearFSSSearchCacheValidationEvent = 910147,
        /// <summary>
        /// 910148 - Request for clearing FSS search cache data from Azure table started.
        /// </summary>
        ClearFSSSearchCacheStarted = 910148,
        /// <summary>
        /// 910149 - Request for clearing FSS search cache data from Azure table completed.
        /// </summary>
        ClearFSSSearchCacheCompleted = 910149,
        /// <summary>
        /// 910150 -   Event data for Retrieval of MSI Annual File Request Started.
        /// </summary>
        ShowAnnualFilesRequestStarted = 910150,
        /// <summary>
        /// 910151 -   Event data for Retrieval of MSI Annual File Request Completed.
        /// </summary>
        ShowAnnualFilesRequestCompleted = 910151,
        /// <summary>
        /// 910152 -    Event data for Retrieval of MSI Show Annual File Failed.
        /// </summary>
        ShowAnnualFilesFailed = 910152,
        /// <summary>
        /// 910153 -  Event data for Retrieval of MSI Get Annual File Response Started.
        /// </summary>
        GetAnnualFilesResponseStarted = 910153,
        /// <summary>
        /// 910154 -  Event data for Retrieval of MSI Get Annual File Response Completed.
        /// </summary>
        GetAnnualFilesResponseCompleted = 910154,
        /// <summary>
        /// 910155 -  Get NM Batch Files For Annual Data Not Found.
        /// </summary>
        GetAnnualNMFilesRequestDataNotFound = 910155,
        /// <summary>
        /// 910156 -  Event data for Retrieval of MSI Get Annual File Response Failed.
        /// </summary>
        GetAnnualFilesResponseFailed = 910156,
        /// <summary>
        /// 910157 - Request for searching annual batch files from cache azure table storage is started.
        /// </summary>
        FSSSearchAnnualBatchFilesResponseStoreToCacheStart = 910157,
        /// <summary>
        /// 910158 - Request for searching annual batch files from cache azure table storage is completed.
        /// </summary>
        FSSSearchAnnualBatchFilesResponseStoreToCacheCompleted = 910158,
        /// <summary>
        /// 910159 -  Maritime safety information request to get RNW detail Failed.
        /// </summary>
        RNWListDetailFailed = 910159,
        /// <summary> 
        /// 910160 -  Maritime safety information request for Show Selection started.
        /// </summary>
        RNWShowSelectionStarted = 910160,
        /// <summary> 
        /// 910161 -  Maritime safety information request for Show Selection completed.
        /// </summary>
        RNWShowSelectionCompleted = 910161,
        /// <summary>
        /// 910162 -  Maritime safety information request Show Selection Failed.
        /// </summary>
        RNWShowSelectionFailed = 910162,
        /// <summary>
        /// 910163 - Maritime safety information request to get banner notification message started.
        /// </summary>
        BannerNotificationRequestStarted = 910163,
        /// <summary>
        /// 910164 - Maritime safety information request to get banner notification message completed.
        /// </summary>
        BannerNotificationRequestCompleted = 910164,
        /// <summary>
        /// 910165 - Maritime safety information request to get banner notification message failed.
        /// </summary>
        BannerNotificationRequestFailed = 910165,
        /// <summary>
        /// 910166 - Maritime safety information request to get banner notification message from azure table started.
        /// </summary>
        GetBannerNotificationMessageFromTableStarted = 910166,
        /// <summary>
        /// 910167 - Maritime safety information request to get banner notification message from azure table completed.
        /// </summary>
        GetBannerNotificationMessageFromTableCompleted = 910167,
        /// <summary>
        /// 910168 - Maritime safety information request to get banner notification message from azure table failed.
        /// </summary>
        GetBannerNotificationMessageFromTableFailed = 910168,
        /// <summary>
        /// 910169 - Request For Download All Weekly NM Files Started.
        /// </summary>
        DownloadAllWeeklyNMFileStarted = 910169,
        /// <summary>
        /// 910170 - Request For Download All Weekly NM Files Completed.
        /// </summary>
        DownloadAllWeeklyNMFileCompleted = 910170,
        /// <summary>
        /// 910171 - Request For Download All Weekly NM Files Failed.
        /// </summary>
        DownloadAllWeeklyNMFileFailed = 910171,
        /// <summary>
        /// 910172 - Maritime safety information user requested to add another record with the same reference number.
        /// </summary>
        AddRecordWithSameReferenceNumber = 910172
    }
    
    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
