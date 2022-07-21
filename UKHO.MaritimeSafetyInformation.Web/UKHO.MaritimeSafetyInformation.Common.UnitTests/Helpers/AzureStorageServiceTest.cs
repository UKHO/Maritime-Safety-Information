using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Helpers;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.Helpers
{
    [TestFixture]
    public class AzureStorageServiceTest
    {
        private AzureStorageService _azureStorageService;

        [SetUp]
        public void Setup()
        {
            _azureStorageService = new AzureStorageService();
        }

        [Test]
        public void WhenInValidGetStorageAccountConnectionStringRequest_ThenReturnKeyNotFoundExceptionResponse()
        {
            string storageAccountName = "", storageAccountKey = "Testkey";

            Assert.ThrowsAsync(Is.TypeOf<KeyNotFoundException>()
                 .And.Message.EqualTo("Storage account accesskey not found")
                  , delegate { _azureStorageService.GetStorageAccountConnectionString(storageAccountName, storageAccountKey); return Task.CompletedTask; });
        }

        [Test]
        public void WhenValidGetStorageAccountConnectionStringRequest_ThenReturnResponse()
        {
            string storageAccountName = "Test", storageAccountKey = "Testkey";
            string response = _azureStorageService.GetStorageAccountConnectionString(storageAccountName, storageAccountKey);

            Assert.NotNull(response);
            Assert.AreEqual("DefaultEndpointsProtocol=https;AccountName=Test;AccountKey=Testkey;EndpointSuffix=core.windows.net", response);
        }
    }
}
