using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.Configuration;
using Microsoft.Extensions.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

using System.Globalization;
using UKHO.MaritimeSafetyInformation.Web.Models;
using Azure.Core;
using System.Threading;
using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class NMDataServiceTest
    {
        private IHttpClientFactory _fakehttpClientFactory;
        private IFileShareService _fakefileShareService;
        private IConfiguration _fakeconfiguration;
        private ILogger<NMDataService> _fakeLogger;
        private FileShareServiceConfiguration _fakefileShareServiceConfig;
        private ILogger<NMDataService> fakeLogger;
        private TokenCredential _faketokenCredential;

        private NMDataService _fakeNMDataService;

        [SetUp]
        public void Setup()
        {
            _fakehttpClientFactory = A.Fake<IHttpClientFactory>();
            _fakefileShareService = A.Fake<IFileShareService>();
            _fakeconfiguration = A.Fake<IConfiguration>();
            _fakefileShareServiceConfig = A.Fake<FileShareServiceConfiguration>();
            _fakeLogger = A.Fake<ILogger<NMDataService>>();
            _faketokenCredential = A.Fake<TokenCredential>();

            _fakeNMDataService = new NMDataService(_fakefileShareService,_fakehttpClientFactory,_fakeconfiguration,_fakeLogger,_faketokenCredential);
        }

        [Test]
        public void GetShowFilesResponses_CheckConversionisProper()
        {
            BatchSearchResponse SearchResult = new BatchSearchResponse() {
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

            List<ShowFilesResponseModel> result = _fakeNMDataService.GetShowFilesResponses(SearchResult);

            Assert.Multiple(() =>
            {
                for (int i = 0; i < result.Count; i++)
                {
                    Assert.AreEqual(expected[i].BatchId, result[i].BatchId);
                    Assert.AreEqual(expected[i].Filename, result[i].Filename);
                    Assert.AreEqual(expected[i].FileDescription, result[i].FileDescription);
                    Assert.AreEqual(expected[i].FileExtension, result[i].FileExtension);
                    Assert.AreEqual(expected[i].FileSize, result[i].FileSize);
                    Assert.AreEqual(expected[i].FileSizeinKB, result[i].FileSizeinKB);
                    Assert.AreEqual(expected[i].MimeType, result[i].MimeType);
                }
            });
        }

        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        [Test]
        public void FormatSize_SizeLessthan1024()
        {
            long bytes = 100;
            string expected = "100.0 Bytes";
            string result = _fakeNMDataService.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void FormatSize_SizeInKB()
        {
            long bytes = 1232;
            string expected = "1.2 KB";
            string result = _fakeNMDataService.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void FormatSize_SizeInMB()
        {
            long bytes = 1234567;
            string expected = "1.2 MB";
            string result = _fakeNMDataService.FormatSize(bytes);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetNewAuthToken_ForNotNullResource()
        {
            string resource = "MSI";

            AccessTokenItem accessTokenItem = new AccessTokenItem
            {
                ExpiresIn = DateTime.Now.AddHours(1)/*accessToken.ExpiresOn.UtcDateTime*/,
                AccessToken =  "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjQ5MjM4OTIwLCJuYmYiOjE2NDkyMzg5MjAsImV4cCI6MTY0OTI0NDU2OSwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQW5YVFVTMWtDL3l4ZTJ5R1Nlc1JBVkk5NkJkNXFnTThYNDNkWlorQ1l4eGFGWFFySWpVSkVGVkJsVVMrZDJaUkJOQ3JrNXpMaEVXOW5XK2s4aElHSE9YckU1V1FsNnR1YlhDSURiUTZTZkpRPSIsImFtciI6WyJwd2QiLCJyc2EiXSwiYXBwaWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6Im1vaGFtbWUxNTMxNUBtYXN0ZWsuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYWRkMWM1MDAtYTZkNy00ZGJkLWI4OTAtN2Y4Y2I2ZjdkODYxLyIsImlwYWRkciI6IjIyMy4xODQuMjU0LjE1NCIsIm5hbWUiOiJNb2hhbW1lZCBLaGFuIiwib2lkIjoiMDEzMDU1Y2ItZWQ2Mi00NmQyLWFkZTgtMzhmY2NkNjQwYWE2IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FBNC4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiJnZHBHdUE3dVNmT0djRG5LZWZmTjUxdkFQNldraEo2V3Fsd05pWlIyT2o0IiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJtb2hhbW1lMTUzMTVAbWFzdGVrLmNvbSIsInV0aSI6IlcxeHZYbllfLVVpeS1POVZTTGU4QUEiLCJ2ZXIiOiIxLjAifQ.l0I0fST2hJoNKAZrNiCWINcCJf9E9odSTVPAegqF9ra2AHYS3Ba4WFHxP6KwT6KhreVc3nsRDQkASlmUOvqBxKhP0c5Xrl2l6w4I_6MmqqT81z1D3p9zbYKF7x4zUMfBlvzX6LW5czjTiocGC4iU42Mnil_H4ufVOPbXeu8dOfm05LZ2Rl8YKbyzRwg2V0l9XePXhWQpe9uFoKyDSfplmf2aeHETv1OwtY3sDVnjEXK5fuS5N9KsXM8eNfnq930IkszLAy11lj05yUXoQa7TTe8VZBN2mo9KTFGG6EYzDE4OFbGcRgQCORjT9ifr606p0Kc-fc2U9ayX4h0_Nvb9eg" 
            };

            CancellationToken cancellationToken = new CancellationToken();
            TokenRequestContext tokenRequestContext = new TokenRequestContext(scopes: new string[] { resource + "/.default" }) { };
            AccessToken accessToken = new AccessToken();
            A.CallTo(() => _faketokenCredential.GetTokenAsync(tokenRequestContext, cancellationToken)).Returns(accessToken);

            Task<AccessTokenItem> result = _fakeNMDataService.GetNewAuthToken(resource);

            Assert.AreEqual(accessTokenItem.AccessToken, result.Result.AccessToken);
        }

        [Test]
        public void GetNewAuthToken_ForNullResource()
        {
            string resource = null;

            AccessTokenItem accessTokenItem = new AccessTokenItem
            {
                ExpiresIn = DateTime.Now.AddHours(1)/*accessToken.ExpiresOn.UtcDateTime*/,
                AccessToken  = null
            };

            CancellationToken cancellationToken = new CancellationToken();
            TokenRequestContext tokenRequestContext = new TokenRequestContext(scopes: new string[] { resource + "/.default" }) { };
            AccessToken accessToken = new AccessToken();
            A.CallTo(() => _faketokenCredential.GetTokenAsync(tokenRequestContext, cancellationToken)).Returns(accessToken);

            Task<AccessTokenItem> result = _fakeNMDataService.GetNewAuthToken(resource);

            Assert.AreEqual(accessTokenItem.AccessToken,result.Result.AccessToken);
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
