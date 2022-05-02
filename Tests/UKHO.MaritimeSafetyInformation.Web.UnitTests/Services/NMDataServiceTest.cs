using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class NMDataServiceTest
    {
        private IFileShareService _fakefileShareService;
        private IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private ILogger<NMDataService> _fakeLogger;
        private IAuthFssTokenProvider _fakeAuthFssTokenProvider;
        private NMDataService _fakeNMDataService;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakefileShareService = A.Fake<IFileShareService>();
            _fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            _fakeLogger = A.Fake<ILogger<NMDataService>>();
            _fakeAuthFssTokenProvider = A.Fake<IAuthFssTokenProvider>();

            _fakeNMDataService = new NMDataService(_fakefileShareService, _fakeLogger, _fakeAuthFssTokenProvider);
        }


        [Test]
        public void WhenGetNMBatchFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

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

            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, CorrelationId)).Returns(SearchResult);

            string expectedstatus = "RanToCompletion";

            Task<List<ShowFilesResponseModel>> ListshowFilesResponseModels = _fakeNMDataService.GetNMBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedstatus, ListshowFilesResponseModels.Status.ToString());
        }

        [Test]
        public void WhenGetNMBatchFilesIsCalled_ThenShouldReturnZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync("", "", CorrelationId)).Returns(res);

            string expectedstatus = "RanToCompletion";

            Task<List<ShowFilesResponseModel>> ListshowFilesResponseModels = _fakeNMDataService.GetNMBatchFiles(year, week, CorrelationId);

            Assert.AreEqual(expectedstatus, ListshowFilesResponseModels.Status.ToString());

        }

        [Test]
        public void WhenGetNMBatchFilesIsCalled_ThenShouldExecuteCatch()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored)).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync("", "", CorrelationId)).Returns(res);

            Task<List<ShowFilesResponseModel>> result = _fakeNMDataService.GetNMBatchFiles(year, week, CorrelationId);

            Assert.That(result.IsFaulted, Is.True);
        }


        [Test]
        public void WhenGetAllWeeksofYearIsCalled_ThenForCurrentYearShouldReturnWeeksPassedTillNow()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

            Calendar cal = dfi.Calendar;

            int year = DateTime.Now.Year;

            int totalWeeks = cal.GetWeekOfYear(new DateTime(year, DateTime.Now.Month, DateTime.Now.Day), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year, CorrelationId);

            Assert.AreEqual(totalWeeks + 1, result.Count);
        }
        [Test]
        public void WhenGetAllWeeksofYearIsCalled_ThenForPastYearShouldReturnAllWeeksThatYear()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

            Calendar cal = dfi.Calendar;

            int year = DateTime.Now.Year - 1;

            DateTime lastdate = new(year, 12, 31);

            int totalWeeks = cal.GetWeekOfYear(lastdate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year, CorrelationId);

            Assert.AreEqual(totalWeeks + 1, result.Count);
        }

        [Test]
        public void WhenGetPastYearsIsCalled_ThenShouldReturn4Records()
        {
            int yearsCount = 4;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetPastYears(CorrelationId);
            Assert.AreEqual(yearsCount, result.Count);
        }

        [Test]
        public void WhenGetPastYearsIsCalled_ThenShouldCheckMinYear()
        {
            int minYear = DateTime.Now.Year - 2;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetPastYears(CorrelationId);
            Assert.AreEqual(minYear.ToString(), result.LastOrDefault().Value);
        }

        [Test]
        public void WhenGetDailyBatchDetailsFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            A.CallTo(() => _fakeAuthFssTokenProvider.GenerateADAccessToken(A<string>.Ignored));

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

            A.CallTo(() => _fakefileShareService.FssBatchSearchAsync(A<string>.Ignored, A<string>.Ignored, CorrelationId)).Returns(SearchResult);

            string expectedstatus = "RanToCompletion";

            Task<List<ShowDailyFilesResponseModel>> ListshowFilesResponseModels = _fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId);

            Assert.AreEqual(expectedstatus, ListshowFilesResponseModels.Status.ToString());
        }
    }
}
