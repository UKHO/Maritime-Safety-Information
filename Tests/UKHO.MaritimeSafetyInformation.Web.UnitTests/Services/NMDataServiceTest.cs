using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.FileShareClient.Models;
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
        private NMDataService _fakeNMDataService;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakefileShareService = A.Fake<IFileShareService>();
            _fakeLogger = A.Fake<ILogger<NMDataService>>();
            _fakeAuthFssTokenProvider = A.Fake<IAuthFssTokenProvider>();

            _fakeNMDataService = new NMDataService(_fakefileShareService, _fakeLogger, _fakeAuthFssTokenProvider);
        }


        [Test]
        public async Task WhenGetNMBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> SearchResult = SetSearchResult();

            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, CorrelationId)).Returns(SearchResult);

            int ExpectedRecordCount = 4;

            List<ShowFilesResponseModel> ListshowFilesResponseModels = await _fakeNMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(ExpectedRecordCount, ListshowFilesResponseModels.Count);
        }

        [Test]
        public async Task WhenGetNMBatchFilesIsCalled_ThenShouldReturnZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync("", "", CorrelationId)).Returns(res);

            int ExpectedRecordCount = 0;

            List<ShowFilesResponseModel> ListshowFilesResponseModels = await _fakeNMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(ExpectedRecordCount, ListshowFilesResponseModels.Count);

        }

        [Test]
        public void WhenGetNMBatchFilesIsCalled_ThenShouldExecuteCatch()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync("", "", CorrelationId)).Returns(res);

            Task<List<ShowFilesResponseModel>> result = _fakeNMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.That(result.IsFaulted, Is.True);
        }

         [Test]
        public void WhenGetAllYearWeekIsCalledWithInvalidToken_ThenShouldReturnException()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssSearchAttributeAsync("", CorrelationId)).Returns(res);

            Task<List<YearWeekModel>> result = _fakeNMDataService.GetAllYearWeek(CorrelationId);
            Assert.IsTrue(result.IsFaulted);                     
        }

        [Test]
        public async Task WhenGetAllYearWeekIsCalledwithValidTokenandNodata_ThenShouldReturnCountZero()
        {
            int ExpectedRecordCount = 0;
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssSearchAttributeAsync("", CorrelationId)).Returns(res);

            List<YearWeekModel> result = await _fakeNMDataService.GetAllYearWeek(CorrelationId);
            Assert.IsEmpty(result);
            Assert.AreEqual(result.Count, ExpectedRecordCount);
        }

        [Test]
        public async Task WhenGetAllYearWeekIsCalledwithValidTokenNoYearWeekdata_ThenShouldReturnNoList()
        {
            int ExpectedRecordCount = 0;
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchNoYearWeekData();
            A.CallTo(() => _fakefileShareService.FssSearchAttributeAsync("", CorrelationId)).Returns(res);

            List<YearWeekModel> result = await _fakeNMDataService.GetAllYearWeek(CorrelationId);

            Assert.IsEmpty(result);
            Assert.AreEqual(result.Count, ExpectedRecordCount);
        }

        [Test]
        public async Task WhenGetAllYearWeekIsCalledwithValidToken_ThenShouldReturnYearWeekList()
        {
            int ExpectedRecordCount = 2;
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchAttributesSearchResponse> res = SetAttributeSearchResult();
            A.CallTo(() => _fakefileShareService.FssSearchAttributeAsync("", CorrelationId)).Returns(res);

            List<YearWeekModel> result = await _fakeNMDataService.GetAllYearWeek(CorrelationId);
            
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, ExpectedRecordCount);
        }
        

        private static Result<BatchSearchResponse> SetSearchResult()
        {
            Result<BatchSearchResponse> SearchResult = new()
            {
                Data = new BatchSearchResponse
                {
                    Count = 2,
                    Links = null,
                    Total = 0,
                    Entries = new List<BatchDetails>() {
                        new BatchDetails() {
                            BatchId = "1",
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

            return SearchResult;
        }

        private static Result<BatchAttributesSearchResponse> SetAttributeSearchResult()
        {
            Result<BatchAttributesSearchResponse> AttributeSearchResult = new()
            {
                Data = new BatchAttributesSearchResponse
                {
                    SearchBatchCount = 5,
                    BatchAttributes = new List<BatchAttributesSearchAttribute> { new BatchAttributesSearchAttribute() {  Key = "Frequency" , Values =  new List<string> { "Weekly","Daily"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Product Type" , Values = new List<string> {"NoticestoMariners"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Week Number", Values = new List<string> { "15", "16", "17", } },
                                                                                new BatchAttributesSearchAttribute() { Key = "Year", Values = new List<string> { "2021", "2022" } },
                                                                                new BatchAttributesSearchAttribute() { Key = "YEAR / WEEK", Values = new List<string> { "2022 / 14", "2022 / 16" } }
                  }
                }
            };
            return AttributeSearchResult;
        }

        private static Result<BatchAttributesSearchResponse> SetAttributeSearchNoYearWeekData()
        {
            Result<BatchAttributesSearchResponse> AttributeSearchResult = new()
            {
                Data = new BatchAttributesSearchResponse
                {
                    SearchBatchCount = 5,
                    BatchAttributes = new List<BatchAttributesSearchAttribute> { new BatchAttributesSearchAttribute() {  Key = "Frequency" , Values =  new List<string> { "Weekly","Daily"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Product Type" , Values = new List<string> {"NoticestoMariners"} },
                                                                                new BatchAttributesSearchAttribute() { Key = "Week Number", Values = new List<string> { "15", "16", "17", } },
                                                                                new BatchAttributesSearchAttribute() { Key = "Year", Values = new List<string> { "2021", "2022" } },
                                                                                new BatchAttributesSearchAttribute() { Key = "YEAR / WEEK", Values = new List<string> {  } }
                  }
                }
            };
            return AttributeSearchResult;
        }
    }
}
