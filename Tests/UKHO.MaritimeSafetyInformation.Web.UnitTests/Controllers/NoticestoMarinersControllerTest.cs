using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
    }
}
