using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class NoticesToMarinersControllerTest
    {
        private NoticesToMarinersController _controller;
        private ILogger<NoticesToMarinersController> _fakeLogger;
        private IHttpContextAccessor _fakeContextAccessor;
        private INMDataService _fakeNMDataService;
        private IUserService _fakeUserService;
        private ITokenAcquisition _fakeTokenAcquisition;

        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<NoticesToMarinersController>>();
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeNMDataService = A.Fake<INMDataService>();
            _fakeUserService = A.Fake<IUserService>();
            _fakeTokenAcquisition = A.Fake<ITokenAcquisition>();
            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new NoticesToMarinersController(_fakeNMDataService, _fakeContextAccessor, _fakeLogger, _fakeUserService);
        }

        [Test]
        public async Task WhenIndexIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";

            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored));

            IActionResult result = await _controller.Index();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
            Assert.AreEqual(false, _controller.ViewBag.IsDistributor);
        }


        [Test]
        public void WhenIndexIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = _controller.Index();

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenIndexPostIsCalled_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022;
            const int week = 20;

            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(SetResultForShowWeeklyFilesResponseModel());

            IActionResult result = await _controller.Index(year, week);
            Assert.IsInstanceOf<ViewResult>(result);

            string actualView = ((ViewResult)result).ViewName;

            Assert.AreEqual(expectedView, actualView);
            Assert.AreEqual(year, Convert.ToInt32(((ViewResult)result).ViewData["Year"]));
            Assert.AreEqual(week, Convert.ToInt32(((ViewResult)result).ViewData["Week"]));
            Assert.AreEqual(false, _controller.ViewBag.IsDistributor);
        }

        [Test]
        public async Task WhenIndexPostIsCalledWithYearAndWeekZero_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 0;
            const int week = 0;
            const int expectedViewCount = 3;

            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored));

            IActionResult result = await _controller.Index(year, week);
            Assert.IsInstanceOf<ViewResult>(result);

            string actualView = ((ViewResult)result).ViewName;

            Assert.AreEqual(expectedView, actualView);
            Assert.AreEqual(expectedViewCount, ((ViewResult)result).ViewData.Count);
        }

        [Test]
        public async Task WhenIndexPostIsCalledWithWeekOrYearZero_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022;
            const int week = 0;

            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(SetResultForShowWeeklyFilesResponseModel());

            IActionResult result = await _controller.Index(year, week);
            Assert.IsInstanceOf<ViewResult>(result);

            string actualView = ((ViewResult)result).ViewName;

            Assert.AreEqual(expectedView, actualView);
            Assert.AreEqual(year, Convert.ToInt32(((ViewResult)result).ViewData["Year"]));
            Assert.AreEqual(week, Convert.ToInt32(((ViewResult)result).ViewData["Week"]));
        }

        [Test]
        public void WhenIndexPostIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            const int year = -1;
            const int week = -1;

            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = _controller.Index(year, week);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenShowWeeklyFilesAsyncIsCalled_ThenShouldReturnsExpectedPartialView()
        {
            const int year = 2022;
            const int week = 15;

            const string expectedView = "~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml";

            A.CallTo(() => _fakeNMDataService.GetWeeklyBatchFiles(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored));

            IActionResult result = await _controller.ShowWeeklyFilesAsync(year, week);
            Assert.IsInstanceOf<PartialViewResult>(result);
            string actualView = ((PartialViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public async Task WhenShowDailyFilesAsyncIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/ShowDailyFiles.cshtml";

            A.CallTo(() => _fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId));

            IActionResult result = await _controller.ShowDailyFilesAsync();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
            Assert.AreEqual(false, _controller.ViewBag.IsDistributor);
        }

        [Test]
        public void WhenDownloadFileIsCalledWithNullBatchID_ThenShouldReturnArgumentNullException()
        {
            const string batchId = null;
            const string fileName = "testfile.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter BatchId"),
                async delegate { await _controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithEmptyBatchID_ThenShouldReturnArgumentNullException()
        {
            const string batchId = "";
            const string fileName = "testfile.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter BatchId"),
                async delegate { await _controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithNullFileName_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = null;
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter FileName"),
                async delegate { await _controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithEmptyFileName_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter FileName"),
               async delegate { await _controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithNullMimeType_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = null;
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter MimeType"),
               async delegate { await _controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithEmptyMimeType_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = "";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter MimeType"),
               async delegate { await _controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public async Task WhenDownloadFileIsCalled_ThenShouldReturnFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            A.CallTo(() => _fakeNMDataService.DownloadFssFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));
            IActionResult result = await _controller.DownloadFile(batchId, fileName, mimeType, frequency);

            Assert.IsInstanceOf<FileResult>(result);
        }

        [Test]
        public void WhenDownloadFileIsCalled_ThenShouldExecuteCatch()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = "wrongmime";
            const string frequency = "Weekly";

            A.CallTo(() => _fakeNMDataService.DownloadFssFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());
            _fakeContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "Test");

            Task<FileResult> result = _controller.DownloadFile(batchId, fileName, mimeType, frequency);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenDownloadDailyFileIsCalled_ThenShouldReturnFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            const string mimeType = "application/gzip";

            A.CallTo(() => _fakeNMDataService.DownloadFSSZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));
            IActionResult result = await _controller.DownloadDailyFile(batchId, fileName, mimeType);

            Assert.IsInstanceOf<FileResult>(result);
        }

        [Test]
        public void WhenDownloadDailyFileIsCalled_ThenShouldReturnShowDailyFilesAction()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            const string mimeType = "application/gzip";

            A.CallTo(() => _fakeNMDataService.DownloadFSSZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());

            _fakeContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "Test");

            Task<FileResult> result = _controller.DownloadDailyFile(batchId, fileName, mimeType);

            Assert.IsTrue(result.IsFaulted);

        }

        [Test]
        public async Task WhenLeisureIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Leisure.cshtml";

            A.CallTo(() => _fakeNMDataService.GetLeisureFilesAsync(A<string>.Ignored));

            IActionResult result = await _controller.Leisure();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenLeisureIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            A.CallTo(() => _fakeNMDataService.GetLeisureFilesAsync(A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = _controller.Leisure();

            Assert.IsTrue(result.IsFaulted);
        }

        [TestCase(null, "Daily 16-05-22.zip", "application/gzip", Description = "When Download Daily File Is Called With Null BatchID Then Should Throw Exception")]
        [TestCase("", "Daily 16-05-22.zip", "application/gzip", Description = "When Download Daily File Is Called With Empty BatchID Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d1", "Daily 16-05-22.zip", null, Description = "When Download Daily File Is Called With Null Mime Type Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d2", "Daily 16-05-22.zip", "", Description = "When Download Daily File Is Called With Empty Mime Type Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d3", null, "application/gzip", Description = "When Download Daily File Is Called With Null File Name Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d4", "", "application/gzip", Description = "When Download Daily File Is Called With Empty File Name Then Should Then Should Throw Exception")]
        public void WhenDownloadDailyFileIsCalledWithEmptyValue_ThenShouldThrowException(string batchId, string fileName, string mimeType)
        {
            Task<FileResult> result = _controller.DownloadDailyFile(batchId, fileName, mimeType);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenAboutIsCalled_ThenReturnView()
        {
            const string expectedView = "~/Views/NoticesToMariners/About.cshtml";

            IActionResult result = _controller.About();
            Assert.IsInstanceOf<IActionResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public async Task WhenCumulativeFilesAsyncIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Cumulative.cshtml";

            A.CallTo(() => _fakeNMDataService.GetCumulativeBatchFiles(CorrelationId));

            IActionResult result = await _controller.Cumulative();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenCumulativeIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            A.CallTo(() => _fakeNMDataService.GetCumulativeBatchFiles(A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = _controller.Cumulative();

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenShowWeeklyFilesAsyncIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => _fakeNMDataService.GetWeeklyBatchFiles(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = _controller.ShowWeeklyFilesAsync(year, week);

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenShowDailyFilesAsyncIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            A.CallTo(() => _fakeNMDataService.GetDailyBatchDetailsFiles(A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = _controller.ShowDailyFilesAsync();

            Assert.IsTrue(result.IsFaulted);
        }

        private static ShowWeeklyFilesResponseModel SetResultForShowWeeklyFilesResponseModel()
        {
            return new()
            {
                ShowFilesResponseList = new()
                {
                    new ShowFilesResponseModel()
                    {
                        BatchId = "1",
                        Filename = "aaa.pdf",
                        FileDescription = "aaa",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    },
                    new ShowFilesResponseModel()
                    {
                        BatchId = "1",
                        Filename = "bbb.pdf",
                        FileDescription = "bbb",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    },
                    new ShowFilesResponseModel()
                    {
                        BatchId = "2",
                        Filename = "ccc.pdf",
                        FileDescription = "ccc",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    },
                    new ShowFilesResponseModel()
                    {
                        BatchId = "2",
                        Filename = "ddd.pdf",
                        FileDescription = "ddd",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    }
                },
                YearAndWeekList = new()
                {
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 17
                    },
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 18
                    },
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 19
                    },
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 20
                    }
                }
            };
        }
    }
}
