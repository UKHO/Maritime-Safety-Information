using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.AzureTableEntities;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class FileShareServiceCacheTest
    {
        private IAzureTableStorageClient _azureTableStorageClient;
        private IOptions<CacheConfiguration> _cacheConfiguration;
        private IAzureStorageService _azureStorageService;
        private IFileShareServiceCache _fileShareServiceCache;
        private ILogger<FileShareServiceCache> _logger;

        [SetUp]
        public void Setup()
        {
            _azureTableStorageClient = A.Fake<IAzureTableStorageClient>();
            _cacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();
            _logger = A.Fake<ILogger<FileShareServiceCache>>();
            _fileShareServiceCache = A.Fake<IFileShareServiceCache>();
            _azureStorageService = A.Fake<IAzureStorageService>();

            _fileShareServiceCache = new FileShareServiceCache(_azureTableStorageClient, _cacheConfiguration, _azureStorageService, _logger);
        }

        [Test]
        public async Task WhenFSSCacheCallsGetAllYearWeekFromCache_ThenReturnsNullResponse()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(new CustomTableEntity());

            BatchAttributesSearchModel result = await _fileShareServiceCache.GetAllYearWeekFromCache("Public", "", "");

            Assert.IsInstanceOf<BatchAttributesSearchModel>(result);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task WhenFSSCacheCallsReceivedExpiredCacheData_ThenReturnsEmptyBatchAttributesResponse()
        {
            CustomTableEntity customTableEntity = new()
            {
                PartitionKey = "Public",
                RowKey = "",
                Response = "{\"IsSuccess\":true,\"StatusCode\":200,\"Errors\":[],\"Data\":{ \"searchBatchCount\":264,\"batchAttributes\":[{\"Key\":\"DATA DATE\",\"Values\":[\"2020 - 01 - 02\"]},{\"Key\":\"FREQUENCY\",\"Values\":[\"Weekly\"]},{\"Key\":\"PRODUCT TYPE\",\"Values\":[\"Notices to Mariners\"]},{\"Key\":\"WEEK NUMBER\",\"Values\":[\"1\"]},{\"Key\":\"YEAR\",\"Values\":[\"2020\",\"2021\",\"2022\"]},{\"Key\":\"YEAR / WEEK\",\"Values\":[\"2020 / 1\",\"2020 / 10\",\"2020 / 11\"]},{\"Key\":\"CONTENT\",\"Values\":[\"tracings\"]}]},\"AttributeYearAndWeekIsCache\":false}"
            };

            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(customTableEntity);

            A.CallTo(() => _azureTableStorageClient.DeleteEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .MustNotHaveHappened();

            BatchAttributesSearchModel result = await _fileShareServiceCache.GetAllYearWeekFromCache("Public", "", "");

            Assert.IsInstanceOf<BatchAttributesSearchModel>(result);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task WhenFSSCacheCallsGetAllYearWeekFromCache_ThenReturnsBatchAttributesResponse()
        {
            CustomTableEntity customTableEntity = new() { PartitionKey = "Public", RowKey = "",
                Response = "{\"IsSuccess\":true,\"StatusCode\":200,\"Errors\":[],\"Data\":{ \"searchBatchCount\":264,\"batchAttributes\":[{\"Key\":\"DATA DATE\",\"Values\":[\"2020 - 01 - 02\"]},{\"Key\":\"FREQUENCY\",\"Values\":[\"Weekly\"]},{\"Key\":\"PRODUCT TYPE\",\"Values\":[\"Notices to Mariners\"]},{\"Key\":\"WEEK NUMBER\",\"Values\":[\"1\"]},{\"Key\":\"YEAR\",\"Values\":[\"2020\",\"2021\",\"2022\"]},{\"Key\":\"YEAR / WEEK\",\"Values\":[\"2020 / 1\",\"2020 / 10\",\"2020 / 11\"]},{\"Key\":\"CONTENT\",\"Values\":[\"tracings\"]}]},\"AttributeYearAndWeekIsCache\":false}"};

            customTableEntity.CacheExpiry = DateTime.UtcNow.AddMinutes(2);
            BatchAttributesSearchModel SearchResult = JsonConvert.DeserializeObject<BatchAttributesSearchModel>(customTableEntity.Response);

            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(customTableEntity);

            BatchAttributesSearchModel result = await _fileShareServiceCache.GetAllYearWeekFromCache("Public", "", "");

            Assert.IsInstanceOf<BatchAttributesSearchModel>(result);
            Assert.AreEqual(SearchResult.Data.SearchBatchCount, result.Data.SearchBatchCount);
        }

        [Test]
        public async Task WhenFSSCacheCallsGetWeeklyBatchFilesFromCache_ThenReturnsNullResponse()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(new CustomTableEntity());

            BatchSearchResponseModel result = await _fileShareServiceCache.GetWeeklyBatchFilesFromCache("Public", 2022, 2, "");

            Assert.IsInstanceOf<BatchSearchResponseModel>(result);
            Assert.IsNull(result.batchSearchResponse);
        }

        [Test]
        public async Task WhenFSSCacheCallsReceivedExpiredCacheData_ThenReturnsEmptyBatchSearchResponse()
        {
            BatchSearchResponseModel batchSearchResponseModel = new();
            CustomTableEntity customTableEntity = new()
            {
                PartitionKey = "Public",
                RowKey = "",
                Response = "{\"count\":2,\"total\":2,\"entries\":[{\"batchId\":\"e637f124-27bb-4f6a-a413-3b3f879725cf\",\"status\":\"Committed\",\"attributes\":[{\"key\":\"Content\",\"value\":\"tracings\"}],\"businessUnit\":\"MaritimeSafetyInformation\",\"batchPublishedDate\":\"2022-07-06T12:07:23.137Z\",\"expiryDate\":\"2023-01-09T00:00:00Z\",\"files\":[{\"filename\":\"rs6-nms-2020-01.xml\",\"fileSize\":8758,\"mimeType\":\"application/xml\",\"hash\":\"mWZtPqCbWAqAqT4KPHV83w==\",\"attributes\":[],\"links\":{\"get\":{\"href\":\"/batch/e637f124-27bb-4f6a-a413-3b3f879725cf/files/rs6-nms-2020-01.xml\"}}}],\"allFilesZipSize\":3605391}],\"_links\":{ \"self\":{ \"href\":\"/batch?limit=100\"} }}"
            };

            batchSearchResponseModel.batchSearchResponse = JsonConvert.DeserializeObject<BatchSearchResponse>(customTableEntity.Response);

            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(customTableEntity);

            A.CallTo(() => _azureTableStorageClient.DeleteEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                          .MustNotHaveHappened();

            BatchSearchResponseModel result = await _fileShareServiceCache.GetWeeklyBatchFilesFromCache("Public", 2022, 2, "");

            Assert.IsInstanceOf<BatchSearchResponseModel>(result);
            Assert.IsNull(result.batchSearchResponse);
        }

        [Test]
        public async Task WhenFSSCacheCallsGetWeeklyBatchFilesFromCache_ThenReturnsBatchSearchResponse()
        {
            BatchSearchResponseModel batchSearchResponseModel = new();
            CustomTableEntity customTableEntity = new()
            {
                PartitionKey = "Public",
                RowKey = "",
                Response = "{\"count\":2,\"total\":2,\"entries\":[{\"batchId\":\"e637f124-27bb-4f6a-a413-3b3f879725cf\",\"status\":\"Committed\",\"attributes\":[{\"key\":\"Content\",\"value\":\"tracings\"}],\"businessUnit\":\"MaritimeSafetyInformation\",\"batchPublishedDate\":\"2022-07-06T12:07:23.137Z\",\"expiryDate\":\"2023-01-09T00:00:00Z\",\"files\":[{\"filename\":\"rs6-nms-2020-01.xml\",\"fileSize\":8758,\"mimeType\":\"application/xml\",\"hash\":\"mWZtPqCbWAqAqT4KPHV83w==\",\"attributes\":[],\"links\":{\"get\":{\"href\":\"/batch/e637f124-27bb-4f6a-a413-3b3f879725cf/files/rs6-nms-2020-01.xml\"}}}],\"allFilesZipSize\":3605391}],\"_links\":{ \"self\":{ \"href\":\"/batch?limit=100\"} }}"
            };

            customTableEntity.CacheExpiry = DateTime.UtcNow.AddMinutes(2);
            batchSearchResponseModel.batchSearchResponse = JsonConvert.DeserializeObject<BatchSearchResponse>(customTableEntity.Response);

            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(customTableEntity);

            BatchSearchResponseModel result = await _fileShareServiceCache.GetWeeklyBatchFilesFromCache("Public", 2022, 2, "");

            Assert.IsInstanceOf<BatchSearchResponseModel>(result);
            Assert.AreEqual(batchSearchResponseModel.batchSearchResponse.Count, result.batchSearchResponse.Count);
        }

        [Test]
        public async Task WhenFSSCacheCallsInsertEntityAsync_ThenReturnsBatchSearchResponse()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.InsertEntityAsync(A<CustomTableEntity>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .MustNotHaveHappened();

            await _fileShareServiceCache.InsertEntityAsync(new CustomTableEntity(), "");
        }
    }
}
