using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using Microsoft.Extensions.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Globalization;
using UKHO.MaritimeSafetyInformation.Common.Models;
using Azure.Core;
using System.Threading;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Helper;
using UKHO.MaritimeSafetyInformation.Common;
using Microsoft.Identity.Client;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class NMDataServiceTest
    {
        private IHttpClientFactory _fakehttpClientFactory;
        private IFileShareService _fakefileShareService;
        private IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private ILogger<NMDataService> _fakeLogger;
        private ILogger<NMDataService> fakeLogger;
        private NMHelper _fakenMHelper;
        private IAuthFssTokenProvider _fakeAuthFssTokenProvider;

        private NMDataService _fakeNMDataService;

        [SetUp]
        public void Setup()
        {
            _fakehttpClientFactory = A.Fake<IHttpClientFactory>();
            _fakefileShareService = A.Fake<IFileShareService>();
            _fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            fakeLogger = A.Fake<ILogger<NMDataService>>();
            _fakeLogger = A.Fake<ILogger<NMDataService>>();
            _fakenMHelper = A.Fake<NMHelper>();
            _fakeAuthFssTokenProvider = A.Fake<AuthFssTokenProvider>();

            _fakeNMDataService = new NMDataService(_fakefileShareService, _fakehttpClientFactory, _fileShareServiceConfig, fakeLogger, _fakeAuthFssTokenProvider);
        }


        [Test]
        public  void GetBatchDetailsFiles()
        {
            int year = 2022; int week = 15;
            


            Task<AuthenticationResult> authentication = _fakeAuthFssTokenProvider.GetAuthTokenAsync();
            BatchSearchResponse SearchResult = new BatchSearchResponse()
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
            };

            IResult<BatchSearchResponse> res = new Result<BatchSearchResponse>();
            A.CallTo(() => _fakefileShareService.FssWeeklySearchAsync("","")).Returns(res);

            

            List<ShowFilesResponseModel> expected = new List<ShowFilesResponseModel>() {
                    new ShowFilesResponseModel() {
                            BatchId = "1",
                            Filename = "aaa.pdf",
                            FileDescription = "aaa",
                            FileExtension = ".pdf",
                            FileSize = 1232,
                            FileSizeinKB = "1.2 KB",
                            MimeType = "PDF",
                            Links = null
                    },
                    new ShowFilesResponseModel() {
                            BatchId = "1",
                            Filename = "bbb.pdf",
                            FileDescription = "bbb",
                            FileExtension = ".pdf",
                            FileSize = 1232,
                            FileSizeinKB = "1.2 KB",
                            MimeType = "PDF",
                            Links = null
                    },
                    new ShowFilesResponseModel() {
                            BatchId = "2",
                            Filename = "ccc.pdf",
                            FileDescription = "ccc",
                            FileExtension = ".pdf",
                            FileSize = 1232,
                            FileSizeinKB = "1.2 KB",
                            MimeType = "PDF",
                            Links = null
                    },
                    new ShowFilesResponseModel() {
                            BatchId = "2",
                            Filename = "ddd.pdf",
                            FileDescription = "ddd",
                            FileExtension = ".pdf",
                            FileSize = 1232,
                            FileSizeinKB = "1.2 KB",
                            MimeType = "PDF",
                            Links = null
                    }
                };
            string expectedstatus = "RanToCompletion";
           Task< List<ShowFilesResponseModel>> ListshowFilesResponseModels = _fakeNMDataService.GetBatchDetailsFiles(year, week);
            Assert.AreEqual(expectedstatus, ListshowFilesResponseModels.Status.ToString());

        }

        [Test]
        public void GetAllWeeksofYear_ForCurrentYearShouldReturnWeeksPassedTillNow()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            int year = DateTime.Now.Year;

            int totalWeeks = cal.GetWeekOfYear(new DateTime(year, DateTime.Now.Month, DateTime.Now.Day), dfi.CalendarWeekRule,dfi.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year);

            Assert.AreEqual(totalWeeks + 1,result.Count); // +1 for default value like Select Week
        }
        [Test]
        public void GetAllWeeksofYear_ForPastYearShouldReturnAllWeeksThatYear()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            int year = DateTime.Now.Year-1;
            DateTime lastdate = new DateTime(year, 12, 31);

            int totalWeeks = cal.GetWeekOfYear(lastdate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetAllWeeksofYear(year);

            Assert.AreEqual(totalWeeks + 1, result.Count); // +1 for default value like Select Week
        }
        [Test]
        public void GetPastYears_ShouldReturn4Records()
        {
            int yearsCount = 4;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetPastYears();
            Assert.AreEqual(yearsCount, result.Count); // +1 for default value like Select Week
        }

        [Test]
        public void GetPastYears_CheckMinYear()
        {
            int minYear = DateTime.Now.Year-2;
            List<KeyValuePair<string, string>> result = _fakeNMDataService.GetPastYears();
            Assert.AreEqual(minYear.ToString(), result.LastOrDefault().Value); // +1 for default value like Select Week
        }

    }
}
