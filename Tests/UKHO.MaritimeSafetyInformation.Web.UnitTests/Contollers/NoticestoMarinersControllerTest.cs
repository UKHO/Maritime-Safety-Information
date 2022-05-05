using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Contollers
{
    [TestFixture]
    public class NoticesToMarinersControllerTest
    {
        private NoticesToMarinersController _controller;
        private INMDataService _nMService;
        private ILogger<NoticesToMarinersController> _logger;
        private IHttpContextAccessor _contextAccessor;
        private IFileShareService _fileShareService;

        [SetUp]
        public void Setup()
        {
            _nMService = A.Fake<INMDataService>();
            _logger = A.Fake<ILogger<NoticesToMarinersController>>();
            _contextAccessor = A.Fake<IHttpContextAccessor>();
            _fileShareService = A.Fake<IFileShareService>();
            A.CallTo(() => _contextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new NoticesToMarinersController(_nMService, _contextAccessor, _logger, _fileShareService);
        }

        [Test]
        public void WhenIndexIsCalled_ThenShouldReturnView()
        {
            IActionResult result = _controller.Index();
            Assert.IsInstanceOf<ViewResult>(result);
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
        public async Task WhenShowWeeklyFilesAsyncIsCalled_ThenShouldReturnPartialView()
        {
            int year = 2022;
            int week = 16;
            IActionResult result = await _controller.ShowWeeklyFilesAsync(year,week);
            Assert.IsInstanceOf<PartialViewResult>(result);
        }
    }
}
