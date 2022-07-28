
namespace UKHO.MaritimeSafetyInformation.Common.Configuration
{
    public class CacheConfiguration
    {
        public string CacheStorageAccountName { get; set; }
        public string CacheStorageAccountKey { get; set; }
        public string FssWeeklyBatchSearchTableName { get; set; }
        public string FssCacheResponseTableName { get; set; }
        public bool IsFssCacheEnabled { get; set; }
        public int CacheTimeOutInMins { get; set; }
    }
}
