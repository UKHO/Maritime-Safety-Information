extern alias MSIAdminProjectAlias;
using System.Security.Claims;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using NUnit.Framework;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class AdminHomeControllerTest
    {
        private HomeController controller;
        private IHttpContextAccessor fakeContextAccessor;
        private ILogger<HomeController> fakeLogger;
        private readonly ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Admin User"), }, "mock"));

        [SetUp]
        public void Setup()
        {
            fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            fakeLogger = A.Fake<ILogger<HomeController>>();
            A.CallTo(() => fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            controller = new HomeController(fakeContextAccessor, fakeLogger);
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        [Test]
        public void WhenErrorIsCalled_ThenShouldReturnsViewAndViewData()
        {
            IActionResult result = controller.Error();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
        }

        [Test]
        public void WhenAccessDeniedIsCalled_ThenShouldReturnsViewAndViewData()
        {
            IActionResult result = controller.AccessDenied();

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

    }
}
