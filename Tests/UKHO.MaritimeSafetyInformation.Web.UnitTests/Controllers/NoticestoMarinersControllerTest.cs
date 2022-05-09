using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private INMDataService _nMService;
        private ILogger<NoticesToMarinersController> _logger;
        private IHttpContextAccessor _contextAccessor;
        private INMDataService _nMDataService;

        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _nMService = A.Fake<INMDataService>();
            _logger = A.Fake<ILogger<NoticesToMarinersController>>();
            _contextAccessor = A.Fake<IHttpContextAccessor>();
            _nMDataService = A.Fake<INMDataService>();
            A.CallTo(() => _contextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new NoticesToMarinersController(_nMService, _contextAccessor, _logger);
        }

        [Test]
        public void WhenIndexIsCalled_ThenShouldReturnsExpectedView()
        {
            string expectedView = "~/Views/NoticesToMariners/FilterWeeklyFiles.cshtml";
            IActionResult result = _controller.Index();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenDailyFilesIsCalled_ThenShouldReturnsExpectedView()
        {
            string expectedView = "~/Views/NoticesToMariners/ShowDailyFiles.cshtml";
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
            int year = 2022;
            IActionResult result = _controller.LoadWeeks(year);
            Assert.IsInstanceOf<JsonResult>(result);
        }

        [Test]
        public async Task WhenShowWeeklyFilesAsyncIsCalled_ThenShouldReturnsExpectedPartialView()
        {
            int year = 2022;
            int week = 15;

            string expectedView = "~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml";

            A.CallTo(() => _nMDataService.GetWeeklyBatchFiles(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored));

            IActionResult result = await _controller.ShowWeeklyFilesAsync(year, week);
            Assert.IsInstanceOf<PartialViewResult>(result);
            string actualView = ((PartialViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public async Task WhenShowDailyFilesAsyncIsCalled_ThenShouldReturnsExpectedPartialView()
        {
            string expectedView = "~/Views/NoticesToMariners/ShowDailyFilesList.cshtml";

            A.CallTo(() => _nMDataService.GetDailyBatchDetailsFiles(CorrelationId));

            IActionResult result = await _controller.ShowDailyFilesAsync();
            Assert.IsInstanceOf<PartialViewResult>(result);
            string actualView = ((PartialViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }
    }
}
