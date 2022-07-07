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
            _nMDataService = new NMDataService(_fakefileShareService, _fakeLogger, _fakeAuthFssTokenProvider, _httpClientFactory, _fileShareServiceConfig);
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

            List<ShowFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
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

            Task<List<ShowFilesResponseModel>> result = _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

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

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForDuplicateWeeklyFiles();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 3;

            List<ShowFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
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

            Task<List<YearWeekModel>> result = _nMDataService.GetAllYearWeek(CorrelationId);
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
            const int ExpectedRecordCount = 3;
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FSSSearchAttributeAsync(A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            List<YearWeekModel> result = await _nMDataService.GetAllYearWeek(CorrelationId);

            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, ExpectedRecordCount);
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
        public void WhenGetCumulativeBatchFilesIsCalled_ThenShouldExecuteCatch()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(res);

            Task<List<ShowFilesResponseModel>> result = _nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenGetCumulativeBatchFilesIsCalled_ThenShouldReturnsRecordFromLastThreeYearsOnly()
        {

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> searchResult = SetSearchResultForCumulative();
            searchResult.Data.Entries[0].Attributes[0].Value = new DateTime(2015,01,01).ToString();
            searchResult.Data.Entries[1].Attributes[0].Value = new DateTime(2015, 01, 01).ToString();

            A.CallTo(() => _fakefileShareService.FSSBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<IFileShareApiClient>.Ignored)).Returns(searchResult);

            const int expectedRecordCount = 2;

            List<ShowFilesResponseModel> listShowFilesResponseModels = await _nMDataService.GetCumulativeBatchFiles(CorrelationId);

            Assert.AreEqual(expectedRecordCount, listShowFilesResponseModels.Count);
            Assert.AreEqual("NP234(B) 2021", listShowFilesResponseModels[0].FileDescription);
            Assert.AreEqual("NP234(A) 2021", listShowFilesResponseModels[1].FileDescription);
        }

        private static Result<BatchSearchResponse> SetSearchResultForWeekly()
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
                }
            };

            return searchResult;
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
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
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
                                new BatchDetailsAttributes("Data Date","2022-04-21"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
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
                                new BatchDetailsAttributes("Data Date","2022-04-20"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

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
                                new BatchDetailsAttributes("Data Date","2022-04-19"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

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
                                new BatchDetailsAttributes("Data Date","2022-04-22"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
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
                                new BatchDetailsAttributes("Data Date","2022-04-21"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
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
                                new BatchDetailsAttributes("Data Date","2022-04-20"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

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
                            BatchId = "3",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-20"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

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
                                new BatchDetailsAttributes("Data Date","2022-04-19"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

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
                            BatchId = "4",
                            Attributes = new List<BatchDetailsAttributes>()
                            {
                                new BatchDetailsAttributes("Data Date","2022-04-19"),
                                new BatchDetailsAttributes("Frequency","Comulative"),
                                new BatchDetailsAttributes("Product Type","Notices to Mariners"),
                                new BatchDetailsAttributes("Year","2022"),

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
    }
}
