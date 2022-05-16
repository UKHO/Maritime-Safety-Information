using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;
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
        private ITempDataProvider _fakeTempDataProvider;
        private INMDataService _fakeNMDataService;

        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeNMService = A.Fake<INMDataService>();
            _fakeLogger = A.Fake<ILogger<NoticesToMarinersController>>();
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeTempDataProvider = A.Fake<ITempDataProvider>();
            _fakeNMDataService = A.Fake<INMDataService>();
            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new NoticesToMarinersController(_fakeNMService, _fakeContextAccessor, _fakeLogger);
        }

        [Test]
        public async Task WhenIndexIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";

            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored));

            IActionResult result = await _controller.IndexAsync();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public async Task WhenIndexPostIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022, week = 20;
            A.CallTo(() => _fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored));
            _controller.TempData = new TempDataDictionary(_fakeContextAccessor.HttpContext, _fakeTempDataProvider);
            IActionResult result = await _controller.IndexAsync(year, week);
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
            string actualView = ((ViewResult)result).ViewName;
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

            A.CallTo(() => _fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId));

            IActionResult result = await _controller.ShowDailyFilesAsync();
            Assert.IsInstanceOf<PartialViewResult>(result);
            string actualView = ((PartialViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }
    }
}
