using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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
        private IAzureTableStorageClient fakeAzureTableStorageClient;
        private IOptions<CacheConfiguration> fakeCacheConfiguration;
        private IEnterpriseEventCacheDataRequestValidator fakeEnterpriseEventCacheDataRequestValidator;
        private IOptions<FileShareServiceConfiguration> fakeFileShareServiceConfig;
        private IAzureStorageService fakeAzureStorageService;
        private ILogger<WebhookService> fakeLogger;

        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        private WebhookService webhookService;

        [SetUp]
        public void Setup()
        {
            fakeAzureTableStorageClient = A.Fake<IAzureTableStorageClient>();
            fakeCacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();
            fakeEnterpriseEventCacheDataRequestValidator = A.Fake<IEnterpriseEventCacheDataRequestValidator>();
            fakeFileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            fakeFileShareServiceConfig.Value.BusinessUnit = "MaritimeSafetyInformation";
            fakeFileShareServiceConfig.Value.ProductType = "Notices to Mariners";
            fakeAzureStorageService = A.Fake<IAzureStorageService>();
            fakeLogger = A.Fake<ILogger<WebhookService>>();

            webhookService = new WebhookService(fakeAzureTableStorageClient,
                                                fakeCacheConfiguration,
                                                fakeEnterpriseEventCacheDataRequestValidator,
                                                fakeFileShareServiceConfig,
                                                fakeAzureStorageService, fakeLogger);

        }

        [Test]
        public async Task WhenValidateNewFilesPublishedEventDataIsCalled_ThenShouldReturnValidationResult()
        {
            FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest = GetNewFilesPublishedEventData();
            A.CallTo(() => fakeEnterpriseEventCacheDataRequestValidator.Validate(A<FSSNewFilesPublishedEventData>.Ignored)).Returns(new ValidationResult());
            ValidationResult result = await webhookService.ValidateNewFilesPublishedEventData(enterpriseEventCacheDataRequest);
            Assert.That(0, Is.EqualTo(result.Errors.Count));
            Assert.That(true, Is.EqualTo(result.IsValid));
        }

        [Test]
        public async Task WhenDeleteBatchSearchResponseCacheDataIsCalledWithWrongBusinessUnit_ThenShouldNotDeleteCache()
        {
            FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest = GetNewFilesPublishedEventData();
            enterpriseEventCacheDataRequest.BusinessUnit = "TestBusinessUnit";

            A.CallTo(() => fakeAzureTableStorageClient.DeleteTablesAsync(A<List<string>>.Ignored, A<string>.Ignored));

            bool result = await webhookService.DeleteBatchSearchResponseCacheData(enterpriseEventCacheDataRequest, CorrelationId);
            Assert.That(false, Is.EqualTo(result));
        }

        [Test]
        public async Task WhenDeleteBatchSearchResponseCacheDataIsCalledWithWrongProductCode_ThenShouldNotDeleteCache()
        {
            FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest = GetNewFilesPublishedEventData();
            enterpriseEventCacheDataRequest.Attributes.FirstOrDefault(x => x.Key == "Product Type").Value = "TestProductCode";

            A.CallTo(() => fakeAzureTableStorageClient.DeleteTablesAsync(A<List<string>>.Ignored, A<string>.Ignored));

            bool result = await webhookService.DeleteBatchSearchResponseCacheData(enterpriseEventCacheDataRequest, CorrelationId);
            Assert.That(false, Is.EqualTo(result));
        }

        [Test]
        public async Task WhenDeleteBatchSearchResponseCacheDataIsCalled_ThenShouldDeleteCache()
        {
            FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest = GetNewFilesPublishedEventData();

            A.CallTo(() => fakeAzureTableStorageClient.DeleteTablesAsync(A<List<string>>.Ignored, A<string>.Ignored));

            bool result = await webhookService.DeleteBatchSearchResponseCacheData(enterpriseEventCacheDataRequest, CorrelationId);
            Assert.That(true, Is.EqualTo(result));
        }

        private static FSSNewFilesPublishedEventData GetNewFilesPublishedEventData()
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
                {
                    new Common.Models.WebhookRequest.Attribute() { Key = "Product Type", Value = "Notices to Mariners" }
                },
                BatchPublishedDate = Convert.ToDateTime("2022-04-04T11:22:18.2943076Z"),
                Files = new List<File>()
                {
                    new File() {
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
