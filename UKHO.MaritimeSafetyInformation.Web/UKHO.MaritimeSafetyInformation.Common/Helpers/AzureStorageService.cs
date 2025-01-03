
namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public class AzureStorageService : IAzureStorageService
    {
        public string GetStorageAccountConnectionString(string storageAccountName = null, string storageAccountKey = null)
        {
            if (string.IsNullOrWhiteSpace(storageAccountName) || string.IsNullOrWhiteSpace(storageAccountKey))
            {
                throw new KeyNotFoundException($"Storage account accesskey not found");
            }

            //return $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey};EndpointSuffix=core.windows.net";
            return $@"AccountName={storageAccountName};AccountKey={storageAccountKey};DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/{storageAccountName};QueueEndpoint=http://127.0.0.1:10001/{storageAccountName};TableEndpoint=http://127.0.0.1:10002/{storageAccountName};";
        }
    }
}
