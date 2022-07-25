using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Validation;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class WebhookServiceTests
    {
        private IAzureTableStorageClient _fakeaAzureTableStorageClient;
        private IOptions<CacheConfiguration> _fakeCacheConfiguration;
        private IEnterpriseEventCacheDataRequestValidator _fakeEnterpriseEventCacheDataRequestValidator;
        private IOptions<FileShareServiceConfiguration> _fakeFileShareServiceConfig;
        private IAzureStorageService _fakeAzureStorageService;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        private WebhookService _webhookService;

        [SetUp]
        public void Setup()
        {
            _fakeaAzureTableStorageClient = A.Fake<IAzureTableStorageClient>();
            _fakeCacheConfiguration =  A.Fake<IOptions<CacheConfiguration>>();
            _fakeEnterpriseEventCacheDataRequestValidator = A.Fake<IEnterpriseEventCacheDataRequestValidator>();
            _fakeFileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            _fakeFileShareServiceConfig.Value.BusinessUnit = "MaritimeSafetyInformation";
            _fakeFileShareServiceConfig.Value.ProductType = "Notices to Mariners";
            _fakeAzureStorageService = A.Fake<IAzureStorageService>();

            _webhookService = new WebhookService(_fakeaAzureTableStorageClient,
                                                _fakeCacheConfiguration,
                                                _fakeEnterpriseEventCacheDataRequestValidator,
                                                _fakeFileShareServiceConfig,
                                                _fakeAzureStorageService);

        }

        [Test]
        public async Task WhenValidateEventGridCacheDataRequestIsCalled_ThenShouldReturnValidationResult()
        {
            EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest = GetEnterpriseEventCacheData();
            A.CallTo(() => _fakeEnterpriseEventCacheDataRequestValidator.Validate(A<EnterpriseEventCacheDataRequest>.Ignored)).Returns(new ValidationResult());
            ValidationResult result = await _webhookService.ValidateEventGridCacheDataRequest(enterpriseEventCacheDataRequest);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(true, result.IsValid);
        }

        [Test]
        public async Task WhenDeleteSearchAndDownloadCacheDataIsCalledWithWrongBusinessUnit_ThenShouldNotDeleteCache()
        {
            EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest = GetEnterpriseEventCacheData();
            enterpriseEventCacheDataRequest.BusinessUnit = "TestBusinessUnit";

            A.CallTo(() => _fakeaAzureTableStorageClient.DeleteTablesAsync(A<List<string>>.Ignored, A<string>.Ignored));

            bool result = await _webhookService.DeleteSearchAndDownloadCacheData(enterpriseEventCacheDataRequest, CorrelationId);
            Assert.AreEqual(false, result);
        }

        [Test]
        public async Task WhenDeleteSearchAndDownloadCacheDataIsCalledWithWrongProductCode_ThenShouldNotDeleteCache()
        {
            EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest = GetEnterpriseEventCacheData();
            enterpriseEventCacheDataRequest.Attributes.FirstOrDefault(x=>x.Key == "ProductCode").Value = "TestProductCode";

            A.CallTo(() => _fakeaAzureTableStorageClient.DeleteTablesAsync(A<List<string>>.Ignored, A<string>.Ignored));

            bool result = await _webhookService.DeleteSearchAndDownloadCacheData(enterpriseEventCacheDataRequest, CorrelationId);
            Assert.AreEqual(false, result);
        }

        [Test]
        public async Task WhenDeleteSearchAndDownloadCacheDataIsCalled_ThenShouldDeleteCache()
        {
            EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest = GetEnterpriseEventCacheData();

            A.CallTo(() => _fakeaAzureTableStorageClient.DeleteTablesAsync(A<List<string>>.Ignored, A<string>.Ignored));

            bool result = await _webhookService.DeleteSearchAndDownloadCacheData(enterpriseEventCacheDataRequest, CorrelationId);
            Assert.AreEqual(true, result);
        }

        private static EnterpriseEventCacheDataRequest GetEnterpriseEventCacheData()
        { 
            return new()
            {
                BusinessUnit = "MaritimeSafetyInformation",
                Attributes = new List<Attribute>()
                { new Attribute() {Key = "CellName", Value= "Notices to Mariners"},
                  new Attribute() { Key = "ProductCode", Value = "Notices to Mariners" }
                }
            };
        }
    }
}
