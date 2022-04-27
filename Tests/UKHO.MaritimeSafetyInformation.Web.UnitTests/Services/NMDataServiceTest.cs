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

        [SetUp]
        public void Setup()
        {
            _fakefileShareService = A.Fake<IFileShareService>();
            _fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            _fakeLogger = A.Fake<ILogger<NMDataService>>();
            _fakeAuthFssTokenProvider = A.Fake<IAuthFssTokenProvider>();

            _fakeNMDataService = new NMDataService(_fakefileShareService, _fileShareServiceConfig, _fakeLogger, _fakeAuthFssTokenProvider);
        }


        [Test]
        public  void WhenGetBatchDetailsFilesIsCalled_ThenShouldReturnsMoreThanZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GetAuthTokenAsync());

            Result<BatchSearchResponse> SearchResult = new()
            {
                Data= new BatchSearchResponse
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

            A.CallTo(() => _fakefileShareService.FssWeeklySearchAsync(A<string>.Ignored, A<string>.Ignored)).Returns(SearchResult);

            string expectedstatus = "RanToCompletion";

            Task< List<ShowFilesResponseModel>> ListshowFilesResponseModels = _fakeNMDataService.GetBatchDetailsFiles(year, week);

            Assert.AreEqual(expectedstatus, ListshowFilesResponseModels.Status.ToString());
        }

        [Test]
        public void WhenGetBatchDetailsFilesIsCalled_ThenShouldReturnZeroFiles()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GetAuthTokenAsync());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssWeeklySearchAsync("", "")).Returns(res);

            string expectedstatus = "RanToCompletion";

            Task<List<ShowFilesResponseModel>> ListshowFilesResponseModels = _fakeNMDataService.GetBatchDetailsFiles(year, week);

            Assert.AreEqual(expectedstatus, ListshowFilesResponseModels.Status.ToString());

        }

        [Test]
        public void WhenGetBatchDetailsFilesIsCalled_ThenShouldExecuteCatch()
        {
            int year = 2022;
            int week = 15;

            A.CallTo(() => _fakeAuthFssTokenProvider.GetAuthTokenAsync()).Throws(new Exception());

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssWeeklySearchAsync("", "")).Returns(res);

            Task<List<ShowFilesResponseModel>> result = _fakeNMDataService.GetBatchDetailsFiles(year, week);
            
            Assert.That(result.IsFaulted,Is.True);
        }


        [Test]
        public void WhenGetAllWeeksofYearIsCalled_ThenForCurrentYearShouldReturnWeeksPassedTillNow()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

            Calendar cal = dfi.Calendar;

            int year = DateTime.Now.Year;

            int totalWeeks = cal.GetWeekOfYear(new DateTime(year, DateTime.Now.Month, DateTime.Now.Day), dfi.CalendarWeekRule,dfi.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year);

            Assert.AreEqual(totalWeeks + 1,result.Count);
        }
        [Test]
        public void WhenGetAllWeeksofYearIsCalled_ThenForPastYearShouldReturnAllWeeksThatYear()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;

            Calendar cal = dfi.Calendar;

            int year = DateTime.Now.Year-1;

            DateTime lastdate = new(year, 12, 31);

            int totalWeeks = cal.GetWeekOfYear(lastdate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year);

            Assert.AreEqual(totalWeeks + 1, result.Count);
        }

        [Test]
        public void WhenGetPastYearsIsCalled_ThenShouldReturn4Records()
        {
            int yearsCount = 4;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetPastYears();
            Assert.AreEqual(yearsCount, result.Count);
        }

        [Test]
        public void WhenGetPastYearsIsCalled_ThenShouldCheckMinYear()
        {
            int minYear = DateTime.Now.Year-2;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetPastYears();
            Assert.AreEqual(minYear.ToString(), result.LastOrDefault().Value);
        }
    }
}
