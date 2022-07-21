using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class NMDataServiceTest
    {
        private IFileShareService _fakefileShareService;
        private ILogger<NMDataService> _fakeLogger;
        private IAuthFssTokenProvider _fakeAuthFssTokenProvider;
        private IHttpClientFactory _httpClientFactory;
        private IFileShareServiceCache _fakeFileShareServiceCache;
        private IOptions<CacheConfiguration> _fakeCacheConfiguration;

        private IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        private NMDataService _nMDataService;

        [SetUp]
        public void Setup()
        {
            _fakefileShareService = A.Fake<IFileShareService>();
            _fakeLogger = A.Fake<ILogger<NMDataService>>();
            _fakeAuthFssTokenProvider = A.Fake<IAuthFssTokenProvider>();
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            _fakeFileShareServiceCache = A.Fake<IFileShareServiceCache>();
            _fakeCacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();

            _nMDataService = new NMDataService(_fakefileShareService, _fakeLogger, _fakeAuthFssTokenProvider, _httpClientFactory, _fileShareServiceConfig, _fakeFileShareServiceCache,
                                               _fakeCacheConfiguration);
            _fileShareServiceConfig.Value.BaseUrl = "http://www.test.com";
        }

        [Test]
        public async Task WhenGetWeeklyBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForWeekly();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 2;

            ShowNMFilesResponseModel showNMFilesResponseModel = await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, showNMFilesResponseModel.ShowFilesResponseModel.Count);
        }

        [Test]
        public void WhenGetWeeklyBatchFilesIsCalledWithNoData_ThenShouldThrowInvalidDataException()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for weekly NM files"),
                async delegate { await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId); });
        }

        [Test]
        public void WhenGetWeeklyBatchFilesIsCalled_ThenShouldExecuteCatch()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<ShowNMFilesResponseModel> result = _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDaily();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 1;
            const int dailyFilesDataCount = 2;

            List<ShowDailyFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
            Assert.AreEqual(dailyFilesDataCount, listShowFilesResponseModels.FirstOrDefault().DailyFilesData.Count);
        }

        [Test]
        public async Task WhenGetDailyBatchDetailsFilesIsCalledWithDuplicateData_ThenShouldReturnLatestFiles()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateDailyFiles();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 1;
            const int expectedDailyFilesDataCount = 2;

            List<ShowDailyFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
            Assert.AreEqual(expectedDailyFilesDataCount, listShowFilesResponseModels.FirstOrDefault().DailyFilesData.Count);
        }

        [Test]
        public async Task WhenGetWeeklyBatchFilesIsCalledWithDuplicateData_ThenShouldReturnLatestFiles()
        {
            const int year = 2022;
            const int week = 07;
            _fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            BatchSearchResponseModel batchSearchResponseModel = new();
            batchSearchResponseModel.batchSearchResponse = GetBatchSearchResponse();

            A.CallTo(() => _fakeFileShareServiceCache.GetWeeklyBatchResponseFromCache(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(batchSearchResponseModel);

            const int expectedRecordCount = 2;

            ShowNMFilesResponseModel showNMFilesResponseModel = await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, showNMFilesResponseModel.ShowFilesResponseModel.Count);
            Assert.IsTrue(showNMFilesResponseModel.IsWeeklyBatchResponseCached);
        }

        [Test]
        public async Task WhenCacheEnabledForBatchFilesButDataNotInTable_ThenFSSReturnResponse()
        {
            const int year = 2022;
            const int week = 07;
            _fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateWeeklyFiles();

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);
            A.CallTo(() => _fakeFileShareServiceCache.GetWeeklyBatchResponseFromCache(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(new BatchSearchResponseModel());
            A.CallTo(() => _fakeFileShareServiceCache.InsertCacheObject(A<object>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            const int expectedRecordCount = 3;

            ShowNMFilesResponseModel showNMFilesResponseModel = await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, showNMFilesResponseModel.ShowFilesResponseModel.Count);
        }

        [Test]
        public async Task WhenCacheEnabledForGetWeeklyBatchFiles_ThenCacheReturnResponse()
        {
            const int year = 2022;
            const int week = 07;
            _fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            BatchSearchResponseModel batchSearchResponseModel = new();
            batchSearchResponseModel.BatchSearchResponse = GetBatchSearchResponse();

            A.CallTo(() => _fakeFileShareServiceCache.GetWeeklyBatchResponseFromCache(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(batchSearchResponseModel);

            const int expectedRecordCount = 2;

            ShowNMFilesResponseModel showNMFilesResponseModel = await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, showNMFilesResponseModel.ShowFilesResponseModel.Count);
            Assert.IsTrue(showNMFilesResponseModel.IsWeeklyBatchResponseCached);
        }

        [Test]
        public async Task WhenCacheEnabledForBatchFilesButDataNotInTable_ThenFSSReturnResponse()
        {
            const int year = 2022;
            const int week = 07;
            _fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateWeeklyFiles();

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);
            A.CallTo(() => _fakeFileShareServiceCache.GetWeeklyBatchResponseFromCache(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(new BatchSearchResponseModel());

            const int expectedRecordCount = 3;

            ShowNMFilesResponseModel showNMFilesResponseModel = await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, showNMFilesResponseModel.ShowFilesResponseModel.Count);
            Assert.IsFalse(showNMFilesResponseModel.IsWeeklyBatchResponseCached);
        }

        [Test]
        public void WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldExecuteCatch()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<List<ShowDailyFilesResponseModel>> result = _nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for daily NM files"),
                async delegate { await _nMDataService.GetDailyBatchDetailsFiles(CorrelationId); });
        }

        [Test]
        public void WhenGetAllYearWeekIsCalledWithInvalidToken_ThenShouldReturnException()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<YearWeekResponseDataModel> result = _nMDataService.GetAllYearWeek(CorrelationId);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenGetAllYearWeekIsCalled_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("No Data received from File Share Service for request to search attribute year and week"),
                async delegate { await _nMDataService.GetAllYearWeek(CorrelationId); });
        }

        [Test]
        public void WhenGetAllYearWeekIsCalledWithValidTokenNoYearWeekdata_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchNoYearWeekData();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("No data received from File Share Service for request to search attribute year and week"),
                async delegate { await _nMDataService.GetAllYearWeek(CorrelationId); });
        }

        [Test]
        public async Task WhenGetAllYearWeekIsCalledWithValidToken_ThenShouldReturnYearWeekList()
        {
            const int expectedRecordCount = 3;
            _fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);
            A.CallTo(() => _fakeFileShareServiceCache.GetAllYearsAndWeeksFromCache(A<string>.Ignored, A<string>.Ignored)).Returns(new BatchAttributesSearchModel());
            A.CallTo(() => _fakeFileShareServiceCache.InsertCacheObject(A<object>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            YearWeekResponseDataModel result = await _nMDataService.GetAllYearWeek(CorrelationId);

            Assert.IsNotEmpty(result.YearWeekModel);
            Assert.AreEqual(result.YearWeekModel.Count, expectedRecordCount);
        }

        [Test]
        public async Task WhenCacheEnabledForYearWeek_ThenCacheReturnYearWeekList()
        {
            const int expectedRecordCount = 3;
            _fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);
            A.CallTo(() => _fakeFileShareServiceCache.GetAllYearsAndWeeksFromCache(A<string>.Ignored, A<string>.Ignored)).Returns(GetBatchAttributesSearchModel());

            YearWeekResponseDataModel result = await _nMDataService.GetAllYearWeek(CorrelationId);

            Assert.IsNotEmpty(result.YearWeekModel);
            Assert.AreEqual(result.YearWeekModel.Count, expectedRecordCount);
            Assert.IsTrue(result.IsYearAndWeekAttributesCached);
        }

        [Test]
        public async Task WhenCacheEnabledForYearWeekButDataNotInTable_ThenFSSReturnYearWeekList()
        {
            const int expectedRecordCount = 3;
            _fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);
            A.CallTo(() => _fakeFileShareServiceCache.GetAllYearsAndWeeksFromCache(A<string>.Ignored, A<string>.Ignored)).Returns(new BatchAttributesSearchModel());

            YearWeekResponseDataModel result = await _nMDataService.GetAllYearWeek(CorrelationId);

            Assert.IsNotEmpty(result.YearWeekModel);
            Assert.AreEqual(result.YearWeekModel.Count, expectedRecordCount);
            Assert.IsFalse(result.IsYearAndWeekAttributesCached);
        }

        [Test]
        public async Task WhenGetWeeklyFilesResponseModelsAsyncIsCalled_ThenShouldReturnsShowWeeklyFilesResponseModelCount()
        {
            const int year = 2022;
            const int week = 15;
            const int expectedShowFilesResponseModelRecordCount = 2;
            const int expectedYearAndWeekRecordCount = 3;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDaily();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            ShowWeeklyFilesResponseModel showWeeklyFilesResponseModel = await _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId);

            Assert.AreEqual(expectedYearAndWeekRecordCount, showWeeklyFilesResponseModel.YearAndWeekList.Count);
            Assert.AreEqual(expectedShowFilesResponseModelRecordCount, showWeeklyFilesResponseModel.ShowFilesResponseList.Count);
        }

        [Test]
        public async Task WhenGetWeeklyFilesResponseModelsAsyncWithZeroIsCalled_ThenShouldReturnsShowWeeklyFilesResponseModelCount()
        {
            const int year = 0;
            const int week = 0;
            const int expectedShowFilesResponseModelRecordCount = 2;
            const int expectedYearAndWeekRecordCount = 3;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDaily();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            ShowWeeklyFilesResponseModel showWeeklyFilesResponseModel = await _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId);

            Assert.AreEqual(expectedYearAndWeekRecordCount, showWeeklyFilesResponseModel.YearAndWeekList.Count);
            Assert.AreEqual(expectedShowFilesResponseModelRecordCount, showWeeklyFilesResponseModel.ShowFilesResponseList.Count);
        }

        [Test]
        public void WhenGetWeeklyFilesResponseModelsAsyncWithZeroIsCalled_ThenShouldThrowInvalidDataException()
        {
            const int year = 2022;
            const int week = 0;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = new();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for weekly NM files"),
                async delegate { await _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId); });
        }

        [Test]
        public void WhenGetWeeklyFilesResponseModelsAsyncWithZeroIsCalled_ThenShouldReturnException()
        {
            const int year = 0;
            const int week = 0;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            Result<BatchSearchResponse> searchResult = new();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<ShowWeeklyFilesResponseModel> result = _nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId);

            Assert.That(result.IsFaulted, Is.True);
        }

        [Test]
        public void WhenDownloadFssFileAsyncIsCalled_ThenShouldReturnByteArray()
        {
            const string batchId = "";
            const string filename = "";
            const string frequency = "";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test stream"));

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));
            A.CallTo(() => _fakefileShareService.FSSDownloadFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored, A<string>.Ignored)).Returns(stream);
            Task<byte[]> result = _nMDataService.DownloadFssFileAsync(batchId, filename, CorrelationId, frequency);
            Assert.IsInstanceOf<Task<byte[]>>(result);
        }

        [Test]
        public void WhenDownloadFssFileAsyncThrowsException_ThenShouldExecuteCatch()
        {
            const string batchId = "";
            const string filename = "";
            const string frequency = "";

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));
            A.CallTo(() => _fakefileShareService.FSSDownloadFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());
            Task<byte[]> result = _nMDataService.DownloadFssFileAsync(batchId, filename, CorrelationId, frequency);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenDownloadFSSZipFileAsyncIsCalled_ThenShouldReturnFileByteArray()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            _fileShareServiceConfig.Value.BaseUrl = "http://www.test.com";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test stream"));

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));
            A.CallTo(() => _fakefileShareService.FSSDownloadZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(stream);

            Task<byte[]> result = _nMDataService.DownloadFSSZipFileAsync(batchId, fileName, CorrelationId);

            Assert.IsInstanceOf<Task<byte[]>>(result);
        }

        [Test]
        public void WhenDownloadFSSZipFileAsyncThrowsException_ThenShouldExecuteCatch()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            _fileShareServiceConfig.Value.BaseUrl = "http://www.test.com";

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());
            A.CallTo(() => _fakefileShareService.FSSDownloadZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Throws(new Exception());

            Task<byte[]> result = _nMDataService.DownloadFSSZipFileAsync(batchId, fileName, CorrelationId);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenGetCumulativeBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFilesOrderByFileName()
        {

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForCumulative();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 4;

            List<ShowFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
            Assert.AreEqual("NP234(B) 2022", listShowFilesResponseModels[0].FileDescription);
            Assert.AreEqual("NP234(A) 2022", listShowFilesResponseModels[1].FileDescription);
            Assert.AreEqual("NP234(B) 2021", listShowFilesResponseModels[2].FileDescription);
            Assert.AreEqual("NP234(A) 2021", listShowFilesResponseModels[3].FileDescription);
        }

        [Test]
        public async Task WhenGetCumulativeBatchFilesIsCalledWithDuplicateData_ThenShouldReturnsLatestFile()
        {

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateCumulative();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 4;

            List<ShowFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
            Assert.AreEqual("NP234(B) 2022", listShowFilesResponseModels[0].FileDescription);
            Assert.AreEqual("NP234(A) 2022", listShowFilesResponseModels[1].FileDescription);
            Assert.AreEqual("NP234(B) 2021", listShowFilesResponseModels[2].FileDescription);
            Assert.AreEqual("NP234(A) 2021", listShowFilesResponseModels[3].FileDescription);
            Assert.AreEqual("2", listShowFilesResponseModels[0].BatchId);
            Assert.AreEqual("1", listShowFilesResponseModels[1].BatchId);
            Assert.AreEqual("5", listShowFilesResponseModels[2].BatchId);
            Assert.AreEqual("4", listShowFilesResponseModels[3].BatchId);
        }

        [Test]
        public void WhenGetCumulativeBatchFilesIsCalledWithNoData_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for cumulative NM files"),
                async delegate { await _nMDataService.GetCumulativeBatchFiles(CorrelationId); });
        }

        [Test]
        public void WhenGetCumulativeBatchFilesIsCalledThrowException_ThenShouldExecuteCatch()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<List<ShowFilesResponseModel>> result = _nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenGetLeisureFilesAsyncIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForLeisure();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 2;

            List<ShowFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetLeisureFilesAsync(CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
        }

        [Test]
        public async Task WhenGetLeisureFilesAsyncIsCalledWithDuplicateData_ThenShouldReturnLatestFiles()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateLeisure();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 2;

            List<ShowFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetLeisureFilesAsync(CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
        }

        [Test]
        public void WhenGetLeisureFilesAsyncIsCalled_ThenShouldExecuteCatch()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> searchResult = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            Task<List<ShowFilesResponseModel>> result = _nMDataService.GetLeisureFilesAsync(CorrelationId);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenGetLeisureFilesAsyncIsCalled_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> searchResult = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for leisure files"),
                async delegate { await _nMDataService.GetLeisureFilesAsync(CorrelationId); });
        }

        private static BatchSearchResponse GetBatchSearchResponse()
        {
            return new BatchSearchResponse
            {
                Count = 2,
                Links = null,
                Total = 0,
                Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                            Attributes = new(),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "aaa.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "bbb.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "2",
                            Attributes = new(),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "ccc.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "ddd.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        }
                    }
            };
        }

        private static Result<BatchSearchResponse> SetSearchResultForWeekly()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = GetBatchSearchResponse()
            };

            return searchResult;
        }

        private static BatchAttributesSearchModel GetBatchAttributesSearchModel()
        {
            return new BatchAttributesSearchModel()
            {
                Data = new BatchAttributesSearchResponse
                {
                    SearchBatchCount = 5,
                    BatchAttributes = new List<BatchAttributesSearchAttribute> { new BatchAttributesSearchAttribute() {  Key = "Frequency" , Values =  new List<string> { "Weekly","Daily"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Product Type" , Values = new List<string> {"NMTest"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Week Number", Values = new List<string> { "14", "16", "17", } },
                                                                                new BatchAttributesSearchAttribute() { Key = "Year", Values = new List<string> { "2021", "2022" } },
                                                                                new BatchAttributesSearchAttribute() { Key = "YEAR/WEEK", Values = new List<string> { "2022 / 14", "2022 / 16", "2021 / 15" ,"..." } }
                  }
                }
            };
        }

        private static Result<BatchSearchResponse> SetSearchResultForDaily()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 2,
                    Links = null,
                    Total = 0,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "aaa.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "bbb.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "68970ffc-4820-47eb-be76-aaa3209eb3b6",
                            AllFilesZipSize=299170,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-21"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "ccc.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "ddd.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        }
                    }
                }
            };

            return searchResult;
        }

        private static Result<BatchSearchResponse> SetSearchResultForDuplicateDailyFiles()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 3,
                    Links = null,
                    Total = 0,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "aaa.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "bbb.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "1ca861e4-d1e6-3a5d-70gb-5f60fkdec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now.AddMinutes(-10),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "aaa.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "bbb.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "23134ffc-4450-47eb-be36-aaa1204eb3b3",
                            AllFilesZipSize=299170,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-21"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),

                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate = DateTime.Now,
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "ccc.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "ddd.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        }
                    }
                }
            };
            return searchResult;
        }

        private static Result<BatchAttributesSearchResponse> SetAttributeSearchResult()
        {
            Result<BatchAttributesSearchResponse> attributeSearchResult = new()
            {
                Data = new BatchAttributesSearchResponse
                {
                    SearchBatchCount = 5,
                    BatchAttributes = new List<BatchAttributesSearchAttribute> { new BatchAttributesSearchAttribute() {  Key = "Frequency" , Values =  new List<string> { "Weekly","Daily"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Product Type" , Values = new List<string> {"NMTest"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Week Number", Values = new List<string> { "14", "16", "17", } },
                                                                                new BatchAttributesSearchAttribute() { Key = "Year", Values = new List<string> { "2021", "2022" } },
                                                                                new BatchAttributesSearchAttribute() { Key = "YEAR/WEEK", Values = new List<string> { "2022 / 14", "2022 / 16", "2021 / 15" ,"..." } }
                  }
                }
            };
            return attributeSearchResult;
        }

        private static Result<BatchAttributesSearchResponse> SetAttributeSearchNoYearWeekData()
        {
            Result<BatchAttributesSearchResponse> attributeSearchResult = new()
            {
                Data = new BatchAttributesSearchResponse
                {
                    SearchBatchCount = 5,
                    BatchAttributes = new List<BatchAttributesSearchAttribute> { new BatchAttributesSearchAttribute() {  Key = "Frequency" , Values =  new List<string> { "Weekly","Daily"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Product Type" , Values = new List<string> {"NMTest"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Week Number", Values = new List<string> { "15", "14", "17", } },
                                                                                new BatchAttributesSearchAttribute() { Key = "Year", Values = new List<string> { "2020", "2022" } },
                                                                                new BatchAttributesSearchAttribute() { Key = "YEAR/WEEK", Values = new List<string> {  } }
                  }
                }
            };
            return attributeSearchResult;
        }

        private static Result<BatchSearchResponse> SetSearchResultForDuplicateWeeklyFiles()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 3,
                    Links = null,
                    Total = 0,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                            Attributes = new(),
                            BatchPublishedDate =DateTime.UtcNow.AddMinutes(-10),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "xxx.pdf",
                                    FileSize=5555,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "yyy.jpg",
                                    FileSize=2222,
                                    MimeType = "jpg",
                                    Links = null
                                },
                                 new BatchDetailsFiles () {
                                    Filename = "xyz.txt",
                                    FileSize=2332,
                                    MimeType = "txt",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "2",
                            Attributes = new(),
                            BatchPublishedDate = DateTime.UtcNow.AddMinutes(-20),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "aaa.pdf",
                                    FileSize=987,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "ddd.pdf",
                                    FileSize=654,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }

                        }
                    }
                }
            };

            return searchResult;
        }

        private static Result<BatchSearchResponse> SetSearchResultForCumulative()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 4,
                    Links = null,
                    Total = 0,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                           new BatchDetails() {
                            BatchId = "2",
                             Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-06-21"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "3",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-01-20"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "4",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-06-19"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        }
                    }
                }
            };

            return searchResult;
        }

        private static Result<BatchSearchResponse> SetSearchResultForDuplicateCumulative()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 6,
                    Links = null,
                    Total = 0,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                           new BatchDetails() {
                            BatchId = "2",
                             Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-06-21"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "3",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-01-20"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now.AddMinutes(-10),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                            new BatchDetails() {
                            BatchId = "4",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-01-20"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                             BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(A) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "5",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-06-19"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                         new BatchDetails() {
                            BatchId = "6",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2021-06-19"),
                                new BatchDetailsAttributes("Frequency","Cumulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2021"),

                            },
                            BatchPublishedDate = DateTime.Now.AddMinutes(-10),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "NP234(B) 2021.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        }
                    }
                }
            };

            return searchResult;
        }

        private static Result<BatchSearchResponse> SetSearchResultForLeisure()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 2,
                    Links = null,
                    Total = 2,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5623"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("05-07-2022 13:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5623 Ireland - South West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "e22bf7c7-4c1c-424a-8aa2-8594ce98e233",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5622"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("05-07-2022 14:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5623 Ireland - West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        }
                    }
                }
            };

            return searchResult;
        }

        private static Result<BatchSearchResponse> SetSearchResultForDuplicateLeisure()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 3,
                    Links = null,
                    Total = 3,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5623"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("05-07-2022 13:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5623 Ireland - South West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "e22bf7c7-4c1c-424a-8aa2-8594ce98e233",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5622"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("05-07-2022 14:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5623 Ireland - West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        },
                        new BatchDetails() {
                            BatchId = "2cd869e1-a1e2-4a7d-94bb-1f60fddec9fe",
                            AllFilesZipSize=346040,
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Chart","SC5623"),
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","leisure"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022")
                            },
                            BusinessUnit = "TEST",
                            BatchPublishedDate =Convert.ToDateTime("06-07-2022 13:25:35"),
                            ExpiryDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "SC5623 Ireland - South West Coast.pdf",
                                    FileSize=636436,
                                    MimeType = "application/pdf",
                                    Links = null
                                }
                            }

                        },
                    }
                }
            };

            return searchResult;
        }
    }
}
