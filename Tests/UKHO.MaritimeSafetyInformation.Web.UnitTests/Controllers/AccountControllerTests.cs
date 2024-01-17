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
    public class AccountControllerTests
    {
        private AccountController controller;
        private IUrlHelper fakeUrlHelper;

        [SetUp]
        public void Setup()
        {
            var ctx = new DefaultHttpContext();
            fakeUrlHelper = A.Fake<IUrlHelper>();

            controller = new AccountController()
            {
                ControllerContext = new() { HttpContext = ctx },
                Url = fakeUrlHelper
            };
        }

        [Test]
        public void WhenSignOutActionCalled_ThenReturnsSignOutResult()
        {
            IActionResult result = controller.SignOut();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<SignOutResult>());
        }

        [Test]
        public void WhenSignOutActionCalled_ThenUsesRequiredAuthenticationScheme()
        {
            IActionResult result = controller.SignOut();

            SignOutResult signOutResult = ((SignOutResult)result);

            Assert.That(result, Is.InstanceOf<SignOutResult>());
            Assert.That(signOutResult.AuthenticationSchemes.Contains(OpenIdConnectDefaults.AuthenticationScheme));
            Assert.That(signOutResult.AuthenticationSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme));
        }

        [Test]
        public void WhenSignedOutActionCalled_ThenRedirects()
        {
            A.CallTo(() => fakeUrlHelper.Content(A<string>.Ignored)).Returns("test");

            IActionResult result = controller.SignedOut();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }
    }
}
