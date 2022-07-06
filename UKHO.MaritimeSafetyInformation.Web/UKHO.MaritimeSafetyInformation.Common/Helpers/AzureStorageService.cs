
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

            string storageAccountConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey};EndpointSuffix=core.windows.net";

            return storageAccountConnectionString;
        }
    }
}
