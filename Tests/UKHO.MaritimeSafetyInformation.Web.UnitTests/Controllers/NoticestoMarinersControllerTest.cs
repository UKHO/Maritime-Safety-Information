using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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

        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

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
            const int year = 2022, week = 20;

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
            const int year = 0, week = 0, expectedViewCount = 2;

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
            const int year = 2022, week = 0;

            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(SetResultForShowWeeklyFilesResponseModel());

            IActionResult result = await _controller.Index(year, week);
            Assert.IsInstanceOf<ViewResult>(result);

            string actualView = ((ViewResult)result).ViewName;

            Assert.AreEqual(expectedView, actualView);
            Assert.AreEqual(year, Convert.ToInt32(((ViewResult)result).ViewData["Year"]));
            Assert.AreEqual(week, Convert.ToInt32(((ViewResult)result).ViewData["Week"]));
        }

        [Test]
        public void WhenDailyFilesIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/ShowDailyFiles.cshtml";
            IActionResult result = _controller.DailyFiles();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
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

            A.CallTo(() => _fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId));

            IActionResult result = await _controller.ShowDailyFilesAsync();
            Assert.IsInstanceOf<PartialViewResult>(result);
            string actualView = ((PartialViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        private static ShowWeeklyFilesResponseModel SetResultForShowWeeklyFilesResponseModel()
        {
            return new()
            {
                ShowFilesResponseModel = new()
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
                YearAndWeek = new()
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
