﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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
        private IFileShareService fakeFileShareService;
        private ILogger<NMDataService> fakeLogger;
        private IAuthFssTokenProvider fakeAuthFssTokenProvider;
        private IHttpClientFactory httpClientFactory;
        private IFileShareServiceCache fakeFileShareServiceCache;
        private IOptions<CacheConfiguration> fakeCacheConfiguration;
        private IOptions<FileShareServiceConfiguration> fileShareServiceConfig;
        private IUserService fakeUserService;
        private NMDataService nMDataService;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            fakeFileShareService = A.Fake<IFileShareService>();
            fakeLogger = A.Fake<ILogger<NMDataService>>();
            fakeAuthFssTokenProvider = A.Fake<IAuthFssTokenProvider>();
            httpClientFactory = A.Fake<IHttpClientFactory>();
            fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            fakeFileShareServiceCache = A.Fake<IFileShareServiceCache>();
            fakeCacheConfiguration = A.Fake<IOptions<CacheConfiguration>>();
            fakeUserService = A.Fake<IUserService>();
            nMDataService = new NMDataService(fakeFileShareService, fakeLogger, fakeAuthFssTokenProvider, httpClientFactory, fileShareServiceConfig, fakeFileShareServiceCache,
                                               fakeCacheConfiguration, fakeUserService);
            fileShareServiceConfig.Value.BaseUrl = "http://www.test.com";
        }

        [Test]
        public async Task WhenGetWeeklyBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForWeekly();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 2;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
        }

        [Test]
        public async Task WhenGetWeeklyBatchFilesIsCalledWithNoData_ThenShouldReturnEmptyShowFilesResponseModel()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.That(0, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
            Assert.That(false, Is.EqualTo(showNMFilesResponseModel.IsBatchResponseCached));
        }

        [Test]
        public void WhenGetWeeklyBatchFilesIsCalled_ThenShouldExecuteCatch()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Throws(new Exception());

            Task<ShowNMFilesResponseModel> result = nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDaily();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 1;
            const int dailyFilesDataCount = 2;

            ShowDailyFilesResponseListModel showDailyFilesResponseListModel = await nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.Count));
            Assert.That(dailyFilesDataCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.FirstOrDefault().DailyFilesData.Count));
        }

        [Test]
        public async Task WhenGetDailyBatchDetailsFilesIsCalledWithDuplicateData_ThenShouldReturnLatestFiles()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateDailyFiles();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 1;
            const int expectedDailyFilesDataCount = 2;

            ShowDailyFilesResponseListModel showDailyFilesResponseListModel = await nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.Count));
            Assert.That(expectedDailyFilesDataCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.FirstOrDefault().DailyFilesData.Count));
        }

        [Test]
        public async Task WhenCacheEnabledForGetDailyBatchDetailsFiles_ThenCacheReturnResponse()
        {
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            BatchSearchResponseModel batchSearchResponseModel = new()
            {
                BatchSearchResponse = GetDailyBatchSearchResponse()
            };

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            A.CallTo(() => fakeFileShareServiceCache.GetBatchResponseFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(batchSearchResponseModel);

            const int expectedRecordCount = 1;
            const int expectedDailyFilesDataCount = 2;

            ShowDailyFilesResponseListModel showDailyFilesResponseListModel = await nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.Count));
            Assert.That(expectedDailyFilesDataCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.FirstOrDefault().DailyFilesData.Count));
            Assert.That(showDailyFilesResponseListModel.IsDailyFilesResponseCached);
        }

        [Test]
        public async Task WhenCacheEnabledForGetDailyBatchDetailsButDataNotInTable_ThenFSSReturnResponse()
        {
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;
            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateDailyFiles();

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            A.CallTo(() => fakeFileShareServiceCache.GetBatchResponseFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new BatchSearchResponseModel());

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 1;
            const int expectedDailyFilesDataCount = 2;

            ShowDailyFilesResponseListModel showDailyFilesResponseListModel = await nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.Count));
            Assert.That(expectedDailyFilesDataCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.FirstOrDefault().DailyFilesData.Count));
            Assert.That(showDailyFilesResponseListModel.IsDailyFilesResponseCached, Is.False);
        }

        [Test]
        public async Task WhenGetWeeklyBatchFilesIsCalledWithDuplicateData_ThenShouldReturnLatestFiles()
        {
            const int year = 2022;
            const int week = 07;
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateWeeklyFiles();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);
            A.CallTo(() => fakeFileShareServiceCache.GetWeeklyBatchResponseFromCache(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new BatchSearchResponseModel());
            A.CallTo(() => fakeFileShareServiceCache.InsertCacheObject(A<object>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            const int expectedRecordCount = 3;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
        }

        [Test]
        public async Task WhenCacheEnabledForGetWeeklyBatchFiles_ThenCacheReturnResponse()
        {
            const int year = 2022;
            const int week = 07;
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            BatchSearchResponseModel batchSearchResponseModel = new();
            batchSearchResponseModel.BatchSearchResponse = GetBatchSearchResponse();

            A.CallTo(() => fakeFileShareServiceCache.GetWeeklyBatchResponseFromCache(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(batchSearchResponseModel);

            const int expectedRecordCount = 2;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
            Assert.That(showNMFilesResponseModel.IsBatchResponseCached);
        }

        [Test]
        public async Task WhenCacheEnabledForBatchFilesButDataNotInTable_ThenFSSReturnResponse()
        {
            const int year = 2022;
            const int week = 07;
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateWeeklyFiles();

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);
            A.CallTo(() => fakeFileShareServiceCache.GetWeeklyBatchResponseFromCache(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new BatchSearchResponseModel());

            const int expectedRecordCount = 3;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
            Assert.That(showNMFilesResponseModel.IsBatchResponseCached, Is.False);
        }

        [Test]
        public void WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldExecuteCatch()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<ShowDailyFilesResponseListModel> result = nMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public void WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for daily NM files"),
                async delegate { await nMDataService.GetDailyBatchDetailsFiles(CorrelationId); });
        }

        [Test]
        public void WhenGetAllYearWeekIsCalledWithInvalidToken_ThenShouldReturnException()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored)).Throws(new Exception());

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<YearWeekResponseDataModel> result = nMDataService.GetAllYearWeek(CorrelationId);
            Assert.That(result.IsFaulted);
        }

        [Test]
        public void WhenGetAllYearWeekIsCalled_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("No Data received from File Share Service for request to search attribute year and week"),
                async delegate { await nMDataService.GetAllYearWeek(CorrelationId); });
        }

        [Test]
        public void WhenGetAllYearWeekIsCalledWithValidTokenNoYearWeekdata_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchNoYearWeekData();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("No data received from File Share Service for request to search attribute year and week"),
                async delegate { await nMDataService.GetAllYearWeek(CorrelationId); });
        }

        [Test]
        public async Task WhenGetAllYearWeekIsCalledWithValidToken_ThenShouldReturnYearWeekList()
        {
            const int expectedRecordCount = 3;
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);
            A.CallTo(() => fakeFileShareServiceCache.GetAllYearsAndWeeksFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new BatchAttributesSearchModel());
            A.CallTo(() => fakeFileShareServiceCache.InsertCacheObject(A<object>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            YearWeekResponseDataModel result = await nMDataService.GetAllYearWeek(CorrelationId);

            Assert.That(result.YearWeekModel?.Count > 0);
            Assert.That(result.YearWeekModel.Count, Is.EqualTo(expectedRecordCount));
        }

        [Test]
        public async Task WhenCacheEnabledForYearWeek_ThenCacheReturnYearWeekList()
        {
            const int expectedRecordCount = 3;
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);
            A.CallTo(() => fakeFileShareServiceCache.GetAllYearsAndWeeksFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(GetBatchAttributesSearchModel());

            YearWeekResponseDataModel result = await nMDataService.GetAllYearWeek(CorrelationId);

            Assert.That(result.YearWeekModel?.Count > 0);
            Assert.That(result.YearWeekModel.Count, Is.EqualTo(expectedRecordCount));
            Assert.That(result.IsYearAndWeekAttributesCached);
        }

        [Test]
        public async Task WhenCacheEnabledForYearWeekButDataNotInTable_ThenFSSReturnYearWeekList()
        {
            const int expectedRecordCount = 3;
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);
            A.CallTo(() => fakeFileShareServiceCache.GetAllYearsAndWeeksFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new BatchAttributesSearchModel());

            YearWeekResponseDataModel result = await nMDataService.GetAllYearWeek(CorrelationId);

            Assert.That(result.YearWeekModel?.Count > 0);
            Assert.That(result.YearWeekModel.Count, Is.EqualTo(expectedRecordCount));
            Assert.That(result.IsYearAndWeekAttributesCached, Is.False);
        }

        [Test]
        public async Task WhenGetWeeklyFilesResponseModelsAsyncIsCalled_ThenShouldReturnsShowWeeklyFilesResponseModelCount()
        {
            const int year = 2022;
            const int week = 15;
            const int expectedShowFilesResponseModelRecordCount = 2;
            const int expectedYearAndWeekRecordCount = 3;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDaily();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            ShowWeeklyFilesResponseModel showWeeklyFilesResponseModel = await nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId);

            Assert.That(expectedYearAndWeekRecordCount, Is.EqualTo(showWeeklyFilesResponseModel.YearAndWeekList.Count));
            Assert.That(expectedShowFilesResponseModelRecordCount, Is.EqualTo(showWeeklyFilesResponseModel.ShowFilesResponseList.Count));
        }

        [Test]
        public async Task WhenGetWeeklyFilesResponseModelsAsyncWithZeroIsCalled_ThenShouldReturnsShowWeeklyFilesResponseModelCount()
        {
            const int year = 0;
            const int week = 0;
            const int expectedShowFilesResponseModelRecordCount = 2;
            const int expectedYearAndWeekRecordCount = 3;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDaily();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            ShowWeeklyFilesResponseModel showWeeklyFilesResponseModel = await nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId);

            Assert.That(expectedYearAndWeekRecordCount, Is.EqualTo(showWeeklyFilesResponseModel.YearAndWeekList.Count));
            Assert.That(expectedShowFilesResponseModelRecordCount, Is.EqualTo(showWeeklyFilesResponseModel.ShowFilesResponseList.Count));
        }

        [Test]
        public async Task WhenGetWeeklyFilesResponseModelsAsyncWithZeroIsCalled_ThenShouldReturnEmptyShowFilesResponseList()
        {
            const int year = 2022;
            const int week = 0;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = new();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            ShowWeeklyFilesResponseModel showWeeklyFilesResponseModel = await nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId);

            Assert.That(0, Is.EqualTo(showWeeklyFilesResponseModel.ShowFilesResponseList.Count));
            Assert.That(false, Is.EqualTo(showWeeklyFilesResponseModel.IsWeeklyBatchResponseCached));
            Assert.That(false, Is.EqualTo(showWeeklyFilesResponseModel.IsYearAndWeekAttributesCached));
            Assert.That(3, Is.EqualTo(showWeeklyFilesResponseModel.YearAndWeekList.Count));

        }

        [Test]
        public void WhenGetWeeklyFilesResponseModelsAsyncWithZeroIsCalled_ThenShouldReturnException()
        {
            const int year = 0;
            const int week = 0;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Result<BatchSearchResponse> searchResult = new();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<ShowWeeklyFilesResponseModel> result = nMDataService.GetWeeklyFilesResponseModelsAsync(year, week, CorrelationId);

            Assert.That(result.IsFaulted, Is.True);
        }

        [Test]
        public void WhenDownloadFssFileAsyncIsCalled_ThenShouldReturnByteArray()
        {
            const string batchId = "";
            const string filename = "";
            const string frequency = "";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test stream"));

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));
            A.CallTo(() => fakeFileShareService.FSSDownloadFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored, A<string>.Ignored)).Returns(stream);
            Task<byte[]> result = nMDataService.DownloadFssFileAsync(batchId, filename, CorrelationId, frequency);
            Assert.That(result, Is.InstanceOf<Task<byte[]>>());
        }

        [Test]
        public void WhenDownloadFssFileAsyncThrowsException_ThenShouldExecuteCatch()
        {
            const string batchId = "";
            const string filename = "";
            const string frequency = "";

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));
            A.CallTo(() => fakeFileShareService.FSSDownloadFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());
            Task<byte[]> result = nMDataService.DownloadFssFileAsync(batchId, filename, CorrelationId, frequency);
            Assert.That(result.IsFaulted);
        }

        [Test]
        public void WhenDownloadFSSZipFileAsyncIsCalled_ThenShouldReturnFileByteArray()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            fileShareServiceConfig.Value.BaseUrl = "http://www.test.com";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test stream"));

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));
            A.CallTo(() => fakeFileShareService.FSSDownloadZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(stream);

            Task<byte[]> result = nMDataService.DownloadFSSZipFileAsync(batchId, fileName, CorrelationId);

            Assert.That(result, Is.InstanceOf<Task<byte[]>>());
        }

        [Test]
        public void WhenDownloadFSSZipFileAsyncThrowsException_ThenShouldExecuteCatch()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            fileShareServiceConfig.Value.BaseUrl = "http://www.test.com";

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored)).Throws(new Exception());
            A.CallTo(() => fakeFileShareService.FSSDownloadZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Throws(new Exception());

            Task<byte[]> result = nMDataService.DownloadFSSZipFileAsync(batchId, fileName, CorrelationId);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenGetCumulativeBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFilesOrderByFileName()
        {

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForCumulative();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 4;

            ShowNMFilesResponseModel showNMFiles = await nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFiles.ShowFilesResponseModel.Count));
            Assert.That("NP234(B) 2022", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That("NP234(A) 2022", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[2].FileDescription));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[3].FileDescription));
        }

        [Test]
        public async Task WhenGetCumulativeBatchFilesIsCalledWithDuplicateData_ThenShouldReturnsLatestFile()
        {

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateCumulative();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 4;

            ShowNMFilesResponseModel showNMFiles = await nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFiles.ShowFilesResponseModel.Count));
            Assert.That("NP234(B) 2022", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That("NP234(A) 2022", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[2].FileDescription));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[3].FileDescription));
            Assert.That("2", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].BatchId));
            Assert.That("1", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].BatchId));
            Assert.That("5", Is.EqualTo(showNMFiles.ShowFilesResponseModel[2].BatchId));
            Assert.That("4", Is.EqualTo(showNMFiles.ShowFilesResponseModel[3].BatchId));
        }

        [Test]
        public void WhenGetCumulativeBatchFilesIsCalledWithNoData_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for cumulative NM files"),
                async delegate { await nMDataService.GetCumulativeBatchFiles(CorrelationId); });
        }

        [Test]
        public void WhenGetCumulativeBatchFilesIsCalledThrowException_ThenShouldExecuteCatch()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<ShowNMFilesResponseModel> result = nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenCacheEnabledForGetCumulativeBatchFiles_ThenCacheReturnResponse()
        {
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            A.CallTo(() => fakeFileShareServiceCache.GetBatchResponseFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(new BatchSearchResponseModel { BatchSearchResponse = SetSearchResultForCumulative().Data });

            const int expectedRecordCount = 4;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
            Assert.That(showNMFilesResponseModel.IsBatchResponseCached);
        }

        [Test]
        public async Task WhenCacheEnabledForCumulativeBatchFilesButDataNotInTable_ThenFSSReturnResponse()
        {
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateCumulative();

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);
            A.CallTo(() => fakeFileShareServiceCache.GetBatchResponseFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new BatchSearchResponseModel());

            const int expectedRecordCount = 4;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
            Assert.That(showNMFilesResponseModel.IsBatchResponseCached, Is.False);
        }

        [Test]
        public async Task WhenGetDailyBatchDetailsFilesIsCalledForDistributor_ThenShouldReturnsMoreThanZeroFiles()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
               {
                    new Claim(ClaimTypes.Name, "Distributor"),
                   new Claim(ClaimTypes.Role, "Distributor")
               }, "mock"));

            DefaultHttpContext httpContext = new()
            {
                User = user
            };

            HttpContextAccessor mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new(mockHttpContextAccessor);

            NMDataService _nMDataServiceDistributor = new(fakeFileShareService, fakeLogger, fakeAuthFssTokenProvider, httpClientFactory, fileShareServiceConfig, fakeFileShareServiceCache,
                                               fakeCacheConfiguration, userService);

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDailyDistributorTracings();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 1;
            const int dailyFilesDataCount = 1;

            ShowDailyFilesResponseListModel showDailyFilesResponseListModel = await _nMDataServiceDistributor.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.That(true, Is.EqualTo(userService.IsDistributorUser));
            Assert.That(expectedRecordCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.Count));
            Assert.That(dailyFilesDataCount, Is.EqualTo(showDailyFilesResponseListModel.ShowDailyFilesResponseModel.FirstOrDefault().DailyFilesData.Count));
        }

        [Test]
        public async Task WhenGetAnnualBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForAnnual();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 6;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetAnnualBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
        }
        [Test]
        public async Task WhenGetAnnualBatchFilesIsCalledWithDuplicateData_ThenShouldReturnLatestFiles()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForAnnualDuplicateData();

            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 6;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetAnnualBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
        }

        [Test]
        public void WhenGetAnnualBatchFilesIsCalled_ThenShouldThrowInvalidDataException()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchSearchResponse> searchResult = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for annual NM files"),
                async delegate { await nMDataService.GetAnnualBatchFiles(CorrelationId); });
        }

        [Test]
        public void WhenGetAnnualBatchFilesIsCalled_ThenShouldExecuteCatch()
        {
            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            IResult<BatchSearchResponse> searchResult = new Result<BatchSearchResponse>();
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Throws(new Exception());

            Task<ShowNMFilesResponseModel> result = nMDataService.GetAnnualBatchFiles(CorrelationId);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenCacheEnabledForGetAnnualBatchFiles_ThenCacheReturnResponse()
        {
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));

            A.CallTo(() => fakeFileShareServiceCache.GetBatchResponseFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                           .Returns(new BatchSearchResponseModel { BatchSearchResponse = SetSearchResultForAnnual().Data });

            const int expectedRecordCount = 6;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetAnnualBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
            Assert.That(showNMFilesResponseModel.IsBatchResponseCached);
        }

        [Test]
        public async Task WhenCacheEnabledForAnnualBatchFilesButDataNotInTable_ThenFSSReturnResponse()
        {
            fakeCacheConfiguration.Value.IsFssCacheEnabled = true;

            Result<BatchSearchResponse> searchResult = SetSearchResultForAnnualDuplicateData();

            A.CallTo(() => fakeAuthFssTokenProvider.GenerateADAccessToken(A<bool>.Ignored, A<string>.Ignored));
            A.CallTo(() => fakeFileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);
            A.CallTo(() => fakeFileShareServiceCache.GetBatchResponseFromCache(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(new BatchSearchResponseModel());

            const int expectedRecordCount = 6;

            ShowNMFilesResponseModel showNMFilesResponseModel = await nMDataService.GetAnnualBatchFiles(CorrelationId);

            Assert.That(expectedRecordCount, Is.EqualTo(showNMFilesResponseModel.ShowFilesResponseModel.Count));
            Assert.That(showNMFilesResponseModel.IsBatchResponseCached, Is.False);
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
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),
                            },
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
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Daily"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Week Number","17"),
                                new BatchDetailsAttributes("Year","2022"),
                                new BatchDetailsAttributes("Year / Week","2022 / 17"),
                            },
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

        private static BatchSearchResponse GetDailyBatchSearchResponse()
        {
            return new BatchSearchResponse
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
            };
        }

        private static Result<BatchSearchResponse> SetSearchResultForDaily()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = GetDailyBatchSearchResponse()
            };

            return searchResult;
        }

        private static Result<BatchSearchResponse> SetSearchResultForDailyDistributorTracings()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 1,
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
                                new BatchDetailsAttributes("Content","tracings")
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
                                },
                                new BatchDetailsFiles () {
                                    Filename = "tracings.pdf",
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
                            Attributes = new List<BatchDetailsAttributes> { new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
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
                            Attributes = new List<BatchDetailsAttributes> { new BatchDetailsAttributes() {  Key = "Data Date" , Value =  "2022-04-08" },
                                                                                new BatchDetailsAttributes() { Key = "Frequency" , Value =  "Weekly" },
                                                                                new BatchDetailsAttributes() { Key = "Product Type" , Value = "NMTest" },
                                                                                new BatchDetailsAttributes() { Key = "Week Number", Value = "14" },
                                                                                new BatchDetailsAttributes() { Key = "Year", Value =  "2022"  },
                                                                                new BatchDetailsAttributes() { Key = "YEAR/WEEK", Value =  "2022 / 14"  } },
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

        private static Result<BatchSearchResponse> SetSearchResultForAnnual()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 15,
                    Links = null,
                    Total = 15,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "28 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "27 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "26 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "01 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "02 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "00 NP234(A) 2022.pdf",
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

        private static Result<BatchSearchResponse> SetSearchResultForAnnualDuplicateData()
        {
            Result<BatchSearchResponse> searchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 15,
                    Links = null,
                    Total = 15,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now,
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "28 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "27 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "26 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "01 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "02 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "00 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                }
                            }
                        },
                        new BatchDetails() {
                            BatchId = "1",
                              Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-01-22"),
                                new BatchDetailsAttributes("Frequency","Annual"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

                            },
                            BatchPublishedDate = DateTime.Now.AddDays(-2),
                            Files = new List<BatchDetailsFiles>() {
                                new BatchDetailsFiles () {
                                    Filename = "28 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "27 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "26 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "01 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "02 NP234(A) 2022.pdf",
                                    FileSize=1232,
                                    MimeType = "PDF",
                                    Links = null
                                },
                                new BatchDetailsFiles () {
                                    Filename = "00 NP234(A) 2022.pdf",
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


    }
}
