using Azure;
using Azure.Data.Tables;

namespace UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities
{
    public class MsiBannerNotificationEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Message { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string BannerStatus { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
