
namespace UKHO.MaritimeSafetyInformation.Common.Configuration
{
    public class CacheConfiguration
    {
        public string CacheStorageAccountName { get; set; }
        public string CacheStorageAccountKey { get; set; }
        public string FssWeeklyAttributeTableName { get; set; }
        public string FssWeeklyBatchSearchTableName { get; set; }
        public bool IsFssCacheEnabled { get; set; }
    }
}
