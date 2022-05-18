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

        private const string _correlationId = "7b838400-7d73-4a64-982b-f426bddc1296";

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
        public void WhenIndexIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/FilterWeeklyFiles.cshtml";
            IActionResult result = _controller.Index();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenDailyFilesIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/ShowDailyFiles.cshtml";
            IActionResult result = _controller.DailyFiles();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult) result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenLoadYearsIsCalled_ThenShouldReturnJson()
        {
            IActionResult result = _controller.LoadYears();
            Assert.IsInstanceOf<JsonResult>(result);
        }

        [Test]
        public void WhenLoadWeeksIsCalled_ThenShouldReturnJson()
        {
            const int year = 2022;
            IActionResult result = _controller.LoadWeeks(year);
            Assert.IsInstanceOf<JsonResult>(result);
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
        public async Task WhenShowDailyFilesAsyncIsCalled_ThenShouldReturnsExpectedPartialView()
        {
            const string expectedView = "~/Views/NoticesToMariners/ShowDailyFilesList.cshtml";

            A.CallTo(() => _fakeNMDataService.GetDailyBatchDetailsFiles(_correlationId));

            IActionResult result = await _controller.ShowDailyFilesAsync();
            Assert.IsInstanceOf<PartialViewResult>(result);
            string actualView = ((PartialViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithNullBatchID_ThenShouldRetunException()
        {
            string batchId = null;
            string fileName = "testfile.pdf";
            string mimeType = "application/pdf";
            var result =  _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.That(result.IsFaulted, Is.True);
        }


        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithEmptyBatchID_ThenShouldRetunException()
        {
            string batchId = String.Empty;
            string fileName = "testfile.pdf";
            string mimeType = "application/pdf";
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.That(result.IsFaulted, Is.True);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithNullFileName_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = null;
            string mimeType = "application/pdf";
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.That(result.IsFaulted, Is.True);
        }


        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithEmptyFileName_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = String.Empty;
            string mimeType = "application/pdf";
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.That(result.IsFaulted, Is.True);
        }

        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithNullMimeType_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = "testfile.pdf";
            string mimeType = null;
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.That(result.IsFaulted, Is.True);
        }


        [Test]
        public void WhenDownloadWeeklyFileIsCalledWithEmptyMimeType_ThenShouldRetunException()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = "testfile.pdf";
            string mimeType = String.Empty;
            var result = _controller.DownloadWeeklyFile(batchId, fileName, mimeType);
            Assert.That(result.IsFaulted, Is.True);
        }

        [Test]
        public async Task WhenDownloadWeeklyFileIsCalled_ThenShouldRetunFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            string fileName = "testfile.pdf";
            string mimeType = "application/pdf";

            ShowFilesResponseModel showFilesResponseModel = new() { MimeType= mimeType };

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
            var result =  _controller.DownloadWeeklyFile(batchId, fileName, "wrongmime");
            
            Assert.That(result.IsFaulted, Is.True);
        }
    }
}
