using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        private HomeController _controller;
        private IHttpContextAccessor _fakeContextAccessor;
        private ILogger<HomeController> _fakeLogger;
        private IOptions<AzureAdB2C> _options;

        [SetUp]
        public void Setup()
        {
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<HomeController>>();
            _options = A.Fake<IOptions<AzureAdB2C>>();
            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new HomeController(_fakeContextAccessor, _fakeLogger, _options);
        }

        [Test]
        public void WhenIndexIsCalled_ThenShouldReturnsView()
        {
            IActionResult result = _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task WhenErrorIsCalled_ThenShouldReturnsViewAndViewDataAsync()
        {
            IActionResult result = await _controller.ErrorAsync();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
        }

    }
}
