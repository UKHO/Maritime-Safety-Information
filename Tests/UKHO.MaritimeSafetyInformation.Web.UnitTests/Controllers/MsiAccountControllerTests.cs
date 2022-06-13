using FakeItEasy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class MsiAccountControllerTests
    {
        private MsiAccountController _controller;
        private IUrlHelper _fakeUrlHelper;

        [SetUp]
        public void Setup()
        {
            var ctx = new DefaultHttpContext();
            _fakeUrlHelper = A.Fake<IUrlHelper>();

            _controller = new MsiAccountController
            {
                ControllerContext = new() { HttpContext = ctx },
                Url = _fakeUrlHelper
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
            string callbackUrl = "https://www.abc.com/";
            A.CallTo(() => _fakeUrlHelper.Action(A<UrlActionContext>.Ignored)).Returns(callbackUrl);

            IActionResult result = _controller.SignOut();

            SignOutResult signOutResult = ((SignOutResult)result);

            Assert.IsInstanceOf<SignOutResult>(result);
            Assert.AreEqual(callbackUrl, signOutResult.Properties.RedirectUri);
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(OpenIdConnectDefaults.AuthenticationScheme));
            Assert.IsTrue(signOutResult.AuthenticationSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme));
        }

        [Test]
        public void WhenSignedOutActionCalled_ThenRedirectResult()
        {
            A.CallTo(() => _fakeUrlHelper.Content(A<string>.Ignored)).Returns("test");

            IActionResult result = _controller.SignedOut();

            Assert.IsTrue(result != null);
            Assert.IsInstanceOf<RedirectResult>(result);            
        }
    }
}
