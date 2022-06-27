extern alias MSIAdminProjectAlias;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using NUnit.Framework;
using System.Security.Claims;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class AdminHomeControllerTest
    {
        private HomeController _controller;
        private IHttpContextAccessor _fakeContextAccessor;
        private ILogger<HomeController> _fakeLogger;
        private readonly ClaimsPrincipal _user = new(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Admin User"), }, "mock"));

        [SetUp]
        public void Setup()
        {
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<HomeController>>();
            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new HomeController(_fakeContextAccessor, _fakeLogger);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = _user };
        }

        [Test]
        public void WhenErrorIsCalled_ThenShouldReturnsViewAndViewData()
        {
            IActionResult result = _controller.Error();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
        }

        [Test]
        public void WhenAccessDeniedIsCalled_ThenShouldReturnsViewAndViewData()
        {
            IActionResult result = _controller.AccessDenied();

            Assert.IsInstanceOf<RedirectResult>(result);
        }

    }
}
