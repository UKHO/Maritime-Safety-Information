using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
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
            _fakeCacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();
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
            enterpriseEventCacheDataRequest.Attributes.FirstOrDefault(x => x.Key == "ProductCode").Value = "TestProductCode";

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
                Links = new()
                {
                    BatchDetails = new()
                    {
                        Href = "https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0"
                    },
                    BatchStatus = new()
                    {
                        Href = "https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status"
                    }
                },
                BatchId = "83d08093-7a67-4b3a-b431-92ba42feqw12",
                BusinessUnit = "MaritimeSafetyInformation",
                Attributes = new List<Common.Models.WebhookRequest.Attribute>()
                {   new Common.Models.WebhookRequest.Attribute() {Key = "CellName", Value= "Notices to Mariners"},
                    new Common.Models.WebhookRequest.Attribute() { Key = "ProductCode", Value = "Notices to Mariners" }
                },
                BatchPublishedDate = Convert.ToDateTime("2022-04-04T11:22:18.2943076Z"),
                Files = new List<CacheFile>()
                {
                    new CacheFile() {
                        Filename = "S631-1_Update_Wk45_21_Only.zip",
                        FileSize= 99073923,
                        MimeType= "application/zip",
                        Hash= "yNpJTWFKhD3iasV8B/ePKw==",
                        Attributes = new List<Common.Models.WebhookRequest.Attribute>()
                        {   new Common.Models.WebhookRequest.Attribute() {Key = "TestKey", Value= "Test Value"}
                        },
                        Links = new() {
                        Get= new() {
                                Href = "https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip"
                            }
                        }
                    }
                }
            };
        }
    }
}
