
namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public interface IAzureStorageService
    {
        string GetStorageAccountConnectionString(string storageAccountName = null, string storageAccountKey = null);
    }
}
