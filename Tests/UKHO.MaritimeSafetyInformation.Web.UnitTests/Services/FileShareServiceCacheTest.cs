using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
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
        public async Task WhenFSSCacheCallsGetAllYearsAndWeeksFromCache_ThenReturnsNullObject()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(new CustomTableEntity());

            BatchAttributesSearchModel result = await _fileShareServiceCache.GetAllYearsAndWeeksFromCache(string.Empty, string.Empty, string.Empty);

            Assert.IsInstanceOf<BatchAttributesSearchModel>(result);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task WhenExceptionInFSSCacheCallsGetAllYearsAndWeeksFromCache_ThenReturnsNullObject()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Throws(new Exception());

            BatchAttributesSearchModel result = await _fileShareServiceCache.GetAllYearsAndWeeksFromCache(string.Empty, string.Empty, string.Empty);

            Assert.IsInstanceOf<BatchAttributesSearchModel>(result);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task WhenFSSCacheReceivesExpiredCacheData_ThenReturnsEmptyBatchAttributesResponse()
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

            BatchAttributesSearchModel result = await _fileShareServiceCache.GetAllYearsAndWeeksFromCache(string.Empty, string.Empty, string.Empty);

            Assert.IsInstanceOf<BatchAttributesSearchModel>(result);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task WhenFSSCacheCallsGetAllYearsAndWeeksFromCache_ThenReturnsBatchAttributesResponse()
        {
            CustomTableEntity customTableEntity = new()
            {
                PartitionKey = "Public",
                RowKey = "",
                Response = "{\"IsSuccess\":true,\"StatusCode\":200,\"Errors\":[],\"Data\":{ \"searchBatchCount\":264,\"batchAttributes\":[{\"Key\":\"DATA DATE\",\"Values\":[\"2020 - 01 - 02\"]},{\"Key\":\"FREQUENCY\",\"Values\":[\"Weekly\"]},{\"Key\":\"PRODUCT TYPE\",\"Values\":[\"Notices to Mariners\"]},{\"Key\":\"WEEK NUMBER\",\"Values\":[\"1\"]},{\"Key\":\"YEAR\",\"Values\":[\"2020\",\"2021\",\"2022\"]},{\"Key\":\"YEAR / WEEK\",\"Values\":[\"2020 / 1\",\"2020 / 10\",\"2020 / 11\"]},{\"Key\":\"CONTENT\",\"Values\":[\"tracings\"]}]},\"AttributeYearAndWeekIsCache\":false}",
                CacheExpiry = DateTime.UtcNow.AddMinutes(2)
            };

            BatchAttributesSearchModel searchResult = JsonConvert.DeserializeObject<BatchAttributesSearchModel>(customTableEntity.Response);

            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(customTableEntity);

            BatchAttributesSearchModel result = await _fileShareServiceCache.GetAllYearsAndWeeksFromCache(string.Empty,string.Empty, string.Empty);

            Assert.IsInstanceOf<BatchAttributesSearchModel>(result);
            Assert.AreEqual(searchResult.Data.SearchBatchCount, result.Data.SearchBatchCount);
        }

        [Test]
        public async Task WhenFSSCacheCallsGetWeeklyBatchFilesFromCache_ThenReturnsNullObject()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(new CustomTableEntity());

            BatchSearchResponseModel result = await _fileShareServiceCache.GetWeeklyBatchResponseFromCache(2022, 2, "Public", string.Empty);

            Assert.IsInstanceOf<BatchSearchResponseModel>(result);
            Assert.IsNull(result.BatchSearchResponse);
        }

        [Test]
        public async Task WhenExceptionInFSSCacheCallsGetWeeklyBatchFilesFromCache_ThenReturnsNullObject()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Throws(new Exception());

            BatchSearchResponseModel result = await _fileShareServiceCache.GetWeeklyBatchResponseFromCache(2022, 2, "Public",string.Empty);

            Assert.IsInstanceOf<BatchSearchResponseModel>(result);
            Assert.IsNull(result.BatchSearchResponse);
        }

        [Test]
        public async Task WhenFSSCacheReceivesExpiredCacheData_ThenReturnsEmptyBatchSearchResponse()
        {
            BatchSearchResponseModel batchSearchResponseModel = new();
            CustomTableEntity customTableEntity = new()
            {
                PartitionKey = "Public",
                RowKey = "",
                Response = "{\"count\":2,\"total\":2,\"entries\":[{\"batchId\":\"e637f124-27bb-4f6a-a413-3b3f879725cf\",\"status\":\"Committed\",\"attributes\":[{\"key\":\"Content\",\"value\":\"tracings\"}],\"businessUnit\":\"MaritimeSafetyInformation\",\"batchPublishedDate\":\"2022-07-06T12:07:23.137Z\",\"expiryDate\":\"2023-01-09T00:00:00Z\",\"files\":[{\"filename\":\"rs6-nms-2020-01.xml\",\"fileSize\":8758,\"mimeType\":\"application/xml\",\"hash\":\"mWZtPqCbWAqAqT4KPHV83w==\",\"attributes\":[],\"links\":{\"get\":{\"href\":\"/batch/e637f124-27bb-4f6a-a413-3b3f879725cf/files/rs6-nms-2020-01.xml\"}}}],\"allFilesZipSize\":3605391}],\"_links\":{ \"self\":{ \"href\":\"/batch?limit=100\"} }}"
            };

            batchSearchResponseModel.BatchSearchResponse = JsonConvert.DeserializeObject<BatchSearchResponse>(customTableEntity.Response);

            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(customTableEntity);

            A.CallTo(() => _azureTableStorageClient.DeleteEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                          .MustNotHaveHappened();

            BatchSearchResponseModel result = await _fileShareServiceCache.GetWeeklyBatchResponseFromCache(2022, 2, "Public", string.Empty);

            Assert.IsInstanceOf<BatchSearchResponseModel>(result);
            Assert.IsNull(result.BatchSearchResponse);
        }

        [Test]
        public async Task WhenFSSCacheCallsGetWeeklyBatchFilesFromCache_ThenReturnsBatchSearchResponse()
        {
            BatchSearchResponseModel batchSearchResponseModel = new();
            CustomTableEntity customTableEntity = new()
            {
                PartitionKey = "Public",
                RowKey = "",
                Response = "{\"count\":2,\"total\":2,\"entries\":[{\"batchId\":\"e637f124-27bb-4f6a-a413-3b3f879725cf\",\"status\":\"Committed\",\"attributes\":[{\"key\":\"Content\",\"value\":\"tracings\"}],\"businessUnit\":\"MaritimeSafetyInformation\",\"batchPublishedDate\":\"2022-07-06T12:07:23.137Z\",\"expiryDate\":\"2023-01-09T00:00:00Z\",\"files\":[{\"filename\":\"rs6-nms-2020-01.xml\",\"fileSize\":8758,\"mimeType\":\"application/xml\",\"hash\":\"mWZtPqCbWAqAqT4KPHV83w==\",\"attributes\":[],\"links\":{\"get\":{\"href\":\"/batch/e637f124-27bb-4f6a-a413-3b3f879725cf/files/rs6-nms-2020-01.xml\"}}}],\"allFilesZipSize\":3605391}],\"_links\":{ \"self\":{ \"href\":\"/batch?limit=100\"} }}",
                CacheExpiry = DateTime.UtcNow.AddMinutes(2)
            };

            batchSearchResponseModel.BatchSearchResponse = JsonConvert.DeserializeObject<BatchSearchResponse>(customTableEntity.Response);

            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.GetEntityAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(customTableEntity);

            BatchSearchResponseModel result = await _fileShareServiceCache.GetWeeklyBatchResponseFromCache(2022, 2, "Public", string.Empty);

            Assert.IsInstanceOf<BatchSearchResponseModel>(result);
            Assert.AreEqual(batchSearchResponseModel.BatchSearchResponse.Count, result.BatchSearchResponse.Count);
        }

        [Test]
        public void WhenExecutionFailedForInsertEntityAsync()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.InsertEntityAsync(A<CustomTableEntity>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Throws(new Exception());

            Task response = _fileShareServiceCache.InsertCacheObject(new object(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            Assert.IsTrue(response.IsCompleted);
        }

        [Test]
        public void WhenExecutionSuccessfullyCompletedForInsertEntityAsync()
        {
            A.CallTo(() => _azureStorageService.GetStorageAccountConnectionString(A<string>.Ignored, A<string>.Ignored)).Returns("testConnectionString");

            A.CallTo(() => _azureTableStorageClient.InsertEntityAsync(A<CustomTableEntity>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .MustNotHaveHappened();

            Task response =  _fileShareServiceCache.InsertCacheObject(new object(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            Assert.IsTrue(response.IsCompleted);
        }
    }
}
