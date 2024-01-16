using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Helpers;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helpers
{
    [TestFixture]
    public class AzureStorageServiceTest
    {
        private AzureStorageService azureStorageService;

        [SetUp]
        public void Setup()
        {
            azureStorageService = new AzureStorageService();
        }

        [Test]
        public void WhenInValidGetStorageAccountConnectionStringRequest_ThenReturnKeyNotFoundExceptionResponse()
        {
            string storageAccountName = "", storageAccountKey = "Testkey";

            Assert.ThrowsAsync(Is.TypeOf<KeyNotFoundException>()
                 .And.Message.EqualTo("Storage account accesskey not found")
                  , delegate { azureStorageService.GetStorageAccountConnectionString(storageAccountName, storageAccountKey); return Task.CompletedTask; });
        }

        [Test]
        public void WhenValidGetStorageAccountConnectionStringRequest_ThenReturnResponse()
        {
            string storageAccountName = "Test", storageAccountKey = "Testkey";
            string response = azureStorageService.GetStorageAccountConnectionString(storageAccountName, storageAccountKey);

            Assert.That(response, Is.Not.Null);
            Assert.That("DefaultEndpointsProtocol=https;AccountName=Test;AccountKey=Testkey;EndpointSuffix=core.windows.net", Is.EqualTo(response));
        }
    }
}
