using FakeItEasy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            _controller = new MsiAccountController();
            _controller.ControllerContext = new() { HttpContext = ctx };
            _controller.Url = A.Fake<IUrlHelper>();
        }

        [Test]
        public void SignOutAction_ReturnsSignOutResult()
        {
            IActionResult result = _controller.SignOut();

            Assert.IsInstanceOf<SignOutResult>(result);
        }

        [Test]
        public void SignOutAction_UsesRequiredAuthenticationScheme()
        {
            IActionResult result = _controller.SignOut();
            
            SignOutResult signOutResult = ((SignOutResult)result);

            Assert.IsInstanceOf<SignOutResult>(result);
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(OpenIdConnectDefaults.AuthenticationScheme));
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme));
        }
    }
}
