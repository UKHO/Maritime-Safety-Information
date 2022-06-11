using FakeItEasy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class MsiAccountControllerTests
    {
        private MsiAccountController _controller;

        [SetUp]
        public void Setup()
        {
            var ctx = new DefaultHttpContext();
            _controller = new MsiAccountController
            {
                ControllerContext = new() { HttpContext = ctx }
            };
        }

        [Test]
        public void WhenSignOutActionCalled_ThenReturnsSignOutResult()
        {
            _controller.Url = A.Fake<IUrlHelper>();
            IActionResult result = _controller.SignOut();

            Assert.IsInstanceOf<SignOutResult>(result);
        }

        [Test]
        public void WhenSignOutActionCalled_ThenUsesRequiredAuthenticationScheme()
        {
            _controller.Url = A.Fake<IUrlHelper>();
            IActionResult result = _controller.SignOut();

            SignOutResult signOutResult = ((SignOutResult)result);

            Assert.IsInstanceOf<SignOutResult>(result);
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(OpenIdConnectDefaults.AuthenticationScheme));
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme));
        }

        [Test]
        public void WhenSignedOutActionCalled_ThenSignout()
        {  
            _controller.Url = new UrlHelper(new ActionContext { RouteData = new RouteData() });
            IActionResult result = _controller.SignedOut();

            Assert.IsInstanceOf<RedirectResult>(result);
            Assert.IsTrue(result != null);
        }
    }
}
