using FakeItEasy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class AccountControllerTests
    {
        private AccountController _controller;
        private IUrlHelper _fakeUrlHelper;       

        [SetUp]
        public void Setup()
        {
            var ctx = new DefaultHttpContext();
            _fakeUrlHelper = A.Fake<IUrlHelper>();           
            _controller = new AccountController()
            {
                ControllerContext = new() { HttpContext = ctx },
                Url = _fakeUrlHelper,
            };
        }

        [Test]
        public void WhenSignOutActionCalled_ThenReturnsSignOutResult()
        {
            IActionResult result = _controller.SignOut();
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<SignOutResult>(result);
        }

        [Test]
        public void WhenSignOutActionCalled_ThenUsesRequiredAuthenticationScheme()
        {           
            IActionResult result = _controller.SignOut();

            SignOutResult signOutResult = ((SignOutResult)result);

            Assert.IsInstanceOf<SignOutResult>(result);           
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(OpenIdConnectDefaults.AuthenticationScheme));
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme));
        }

        [Test]
        public void WhenSignedOutActionCalled_ThenRedirects()
        {
            A.CallTo(() => _fakeUrlHelper.Content(A<string>.Ignored)).Returns("test");

            IActionResult result = _controller.SignedOut();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<RedirectResult>(result);            
        }
    }
}
