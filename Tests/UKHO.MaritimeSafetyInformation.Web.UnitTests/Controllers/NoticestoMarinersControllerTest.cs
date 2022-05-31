using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private INMDataService _fakeNMService;
        private ILogger<NoticesToMarinersController> _fakeLogger;
        private IHttpContextAccessor _fakeContextAccessor;
        private INMDataService _fakeNMDataService;

        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeNMService = A.Fake<INMDataService>();
            _fakeLogger = A.Fake<ILogger<NoticesToMarinersController>>();
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeNMDataService = A.Fake<INMDataService>();
            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new NoticesToMarinersController(_fakeNMService, _fakeContextAccessor, _fakeLogger);
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
        }

        [Test]
        public async Task WhenIndexPostIsCalledWithYearAndWeekZero_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 0;
            const int week = 0;
            const int expectedViewCount = 2;

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
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithNullBatchID_ThenShouldRetunException()
        {
            string batchId = null;
            string fileName = "testfile.pdf";
            string mimeType = "application/pdf";
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithEmptyBatchID_ThenShouldRetunException()
        {
            string batchId = String.Empty;
            string fileName = "testfile.pdf";
            string mimeType = "application/pdf";
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithNullFileName_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = null;
            string mimeType = "application/pdf";
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithEmptyFileName_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = String.Empty;
            string mimeType = "application/pdf";
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithNullMimeType_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = "testfile.pdf";
            string mimeType = null;
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithEmptyMimeType_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = "testfile.pdf";
            string mimeType = String.Empty;
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenDownloadWeeklyFileIsCalled_ThenShouldRetunFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = "testfile.pdf";
            string mimeType = "application/pdf";

            ShowFilesResponseModel showFilesResponseModel = new() { MimeType = mimeType };

            A.CallTo(() => _fakeNMDataService.DownloadFssFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));
            IActionResult result = await _controller.DownloadWeeklyFile(batchId, fileName, mimeType);

            Assert.IsInstanceOf<FileResult>(result);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalled_ThenShouldExecuteCatch()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = "testfile.pdf";
            string mimeType = "application/pdf";

            ShowFilesResponseModel showFilesResponseModel = new() { MimeType = mimeType };

            A.CallTo(() => _fakeNMDataService.DownloadFssFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());
            _fakeContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "Test");
            var result = _controller.DownloadWeeklyFile(batchId, fileName, "wrongmime");

            Assert.IsTrue(result.IsFaulted);
        }

        [Test]
        public async Task WhenDownloadDailyFileIsCalled_ThenShouldRetunFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            const string mimeType = "application/gzip";

            ShowFilesResponseModel showFilesResponseModel = new() { MimeType = mimeType };

            A.CallTo(() => _fakeNMDataService.DownloadZipFssFile(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));
            IActionResult result = await _controller.DownloadDailyFile(batchId, fileName, mimeType);

            Assert.IsInstanceOf<FileResult>(result);
        }

        [Test]
        public async Task WhenDownloadDailyFileIsCalled_ThenShouldReturnShowDailyFilesAction()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            const string mimeType = "application/gzip";
            const string expected = "ShowDailyFiles";

            ShowFilesResponseModel showFilesResponseModel = new() { MimeType = mimeType };

            A.CallTo(() => _fakeNMDataService.DownloadZipFssFile(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());

            _fakeContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "Test");

            IActionResult result = await _controller.DownloadDailyFile(batchId, fileName, mimeType);

            string actualView = ((RedirectToActionResult)result).ActionName;

            Assert.AreEqual(expected, actualView);

        }

        [TestCase(null, "Daily 16-05-22.zip", "application/gzip", ExpectedResult = "ShowDailyFiles", Description = "When Download Daily File Is Called With Null BatchID Then Should Return ShowDailyFiles Action")]
        [TestCase("", "Daily 16-05-22.zip", "application/gzip", ExpectedResult = "ShowDailyFiles", Description = "When Download Daily File Is Called With Empty BatchID Then Should Return ShowDailyFiles Action")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d1", "Daily 16-05-22.zip", null, ExpectedResult = "ShowDailyFiles", Description = "When Download Daily File Is Called With Null Mime Type Then Should Return ShowDailyFiles Action")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d2", "Daily 16-05-22.zip", "", ExpectedResult = "ShowDailyFiles", Description = "When Download Daily File Is Called With Empty Mime Type Then Should Return ShowDailyFiles Action")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d3", null, "application/gzip", ExpectedResult = "ShowDailyFiles", Description = "When Download Daily File Is Called With Null File Name Then Should Return ShowDailyFiles Action")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d4", "", "application/gzip", ExpectedResult = "ShowDailyFiles", Description = "When Download Daily File Is Called With Empty File Name Then Should Return ShowDailyFiles Action")]
        public async Task<string> WhenDownloadDailyFileIsCalledWithEmptyBatchID_ThenShouldReturnShowDailyFilesAction(string batchId, string fileName, string mimeType)
        {
            IActionResult result = await _controller.DownloadDailyFile(batchId, fileName, mimeType);

            return ((RedirectToActionResult)result).ActionName;
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
