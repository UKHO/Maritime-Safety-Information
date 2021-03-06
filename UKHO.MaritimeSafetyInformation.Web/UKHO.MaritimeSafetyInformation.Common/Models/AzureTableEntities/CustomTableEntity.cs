using Azure;
using Azure.Data.Tables;

namespace UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities
{
    public class CustomTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Response { get; set; }
        public DateTime CacheExpiry { get; set; }
    }
}
