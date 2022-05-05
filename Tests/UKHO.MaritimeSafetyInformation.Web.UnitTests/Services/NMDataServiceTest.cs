using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task WhenGetWeeklyBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> SearchResult = SetSearchResultForWeekly();

            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, CorrelationId)).Returns(SearchResult);

            int expectedRecordCount = 4;

            List<ShowFilesResponseModel> ListshowFilesResponseModels = await _fakeNMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, ListshowFilesResponseModels.Count);
        }

        [Test]
        public async Task WhenGetWeeklyBatchFilesIsCalled_ThenShouldReturnZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync("", "", CorrelationId)).Returns(res);

            int expectedRecordCount = 0;

            List<ShowFilesResponseModel> ListshowFilesResponseModels = await _fakeNMDataService.GetWeeklyBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedRecordCount, ListshowFilesResponseModels.Count);

        }

        [Test]
        public void WhenGetWeeklyBatchFilesIsCalled_ThenShouldExecuteCatch()
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
        public async Task WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            Result<BatchSearchResponse> SearchResult = SetSearchResultForDaily();

            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, CorrelationId)).Returns(SearchResult);

            int expectedRecordCount = 1;
            int dailyFilesDataCount = 2;

            List<ShowDailyFilesResponseModel> ListshowFilesResponseModels = await _fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.AreEqual(expectedRecordCount, ListshowFilesResponseModels.Count);
            Assert.AreEqual(dailyFilesDataCount, ListshowFilesResponseModels.FirstOrDefault().DailyFilesData.Count);

        }

        [Test]
        public async Task WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldReturnZeroFiles()
        {
            
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync("", "", CorrelationId)).Returns(res);

            int expectedRecordCount = 0;

            List<ShowDailyFilesResponseModel> ListshowFilesResponseModels = await _fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.AreEqual(expectedRecordCount, ListshowFilesResponseModels.Count);

        }

        [Test]
        public void WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldExecuteCatch()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync("", "", CorrelationId)).Returns(res);

            Task<List<ShowDailyFilesResponseModel>> result = _fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.That(result.IsFaulted, Is.True);
        }

        [Test]
        public void WhenGetAllWeeksofYearIsCalled_ThenForCurrentYearShouldReturnWeeksPassedTillNow()
        {
            DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;

            Calendar calender = dateTimeFormatInfo.Calendar;

            int year = DateTime.Now.Year;

            int totalWeeks = calender.GetWeekOfYear(new DateTime(year, DateTime.Now.Month, DateTime.Now.Day), dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year, CorrelationId);

            Assert.AreEqual(totalWeeks + 1, result.Count);
        }
        [Test]
        public void WhenGetAllWeeksofYearIsCalled_ThenForPastYearShouldReturnAllWeeksThatYear()
        {
            DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;

            Calendar calender = dateTimeFormatInfo.Calendar;

            int year = DateTime.Now.Year - 1;

            DateTime lastdate = new(year, 12, 31);

            int totalWeeks = calender.GetWeekOfYear(lastdate, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year, CorrelationId);

            Assert.AreEqual(totalWeeks + 1, result.Count);
        }

        [Test]
        public void WhenGetAllYearsIsCalled_ThenShouldReturn4Records()
        {
            int yearsCount = 4;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllYears(CorrelationId);
            Assert.AreEqual(yearsCount, result.Count);
        }

        [Test]
        public void WhenGetAllYearsIsCalled_ThenShouldCheckMinYear()
        {
            int minYear = DateTime.Now.Year - 2;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllYears(CorrelationId);
            Assert.AreEqual(minYear.ToString(), result.LastOrDefault().Value);
        }

        private static Result<BatchSearchResponse> SetSearchResultForWeekly()
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

        private static Result<BatchSearchResponse> SetSearchResultForDaily()
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

            return SearchResult;
        }

    }
}
