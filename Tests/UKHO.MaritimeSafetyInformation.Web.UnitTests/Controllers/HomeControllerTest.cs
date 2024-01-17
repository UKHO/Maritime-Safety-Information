using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        private HomeController controller;
        private IHttpContextAccessor fakeContextAccessor;
        private ILogger<HomeController> fakeLogger;
        private IOptions<AzureAdB2C> fakeOptions;
        private Endpoint endpoint;

        [SetUp]
        public void Setup()
        {
            fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            fakeLogger = A.Fake<ILogger<HomeController>>();
            fakeOptions = A.Fake<IOptions<AzureAdB2C>>();
            IAuthenticationService fakeAuthService = A.Fake<IAuthenticationService>();
            A.CallTo(() => fakeAuthService.SignInAsync(A<HttpContext>.Ignored, A<string>.Ignored, A<ClaimsPrincipal>.Ignored, A<AuthenticationProperties>.Ignored)).Returns(Task.FromResult((object)null));
            IServiceProvider fakeServiceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => fakeServiceProvider.GetService(typeof(IAuthenticationService))).Returns(fakeAuthService);

            A.CallTo(() => fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext { RequestServices = fakeServiceProvider });

            RequestDelegate requestDelegate = null;
            ControllerActionDescriptor controllerActionDescriptor = new() { ActionName = "Index", ControllerName = "TestController" };
            EndpointMetadataCollection endpointMetadataCollection = new(controllerActionDescriptor);
            endpoint = new(requestDelegate, endpointMetadataCollection, "Test Eror");

            controller = new HomeController(fakeContextAccessor, fakeLogger, fakeOptions);
        }

        [Test]
        public void WhenIndexIsCalled_ThenShouldReturnsView()
        {
            IActionResult result = controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task WhenErrorIsCalled_ThenShouldReturnsViewAndViewDataAsync()
        {
            Exception innerException = new();
            IExceptionHandlerPathFeature exceptionHandlerFeature = new ExceptionHandlerFeature() { Error = new("Test Error", innerException), Endpoint = endpoint };
            FeatureCollection collection = new();
            collection.Set(exceptionHandlerFeature);
            fakeContextAccessor.HttpContext.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);

            IActionResult result = await controller.ErrorAsync();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
            Assert.That(((ViewResult)result).ViewData.ContainsKey("ControllerName"));
            Assert.That(((ViewResult)result).ViewData.ContainsKey("ActionName"));
        }

        [Test]
        public async Task WhenErrorIsCalledWithInnerExceptionMsalUiRequiredException_ThenShouldRedirectToLogin()
        {
            MsalUiRequiredException innerException = new("1001", "MsalUiRequiredException");
            IExceptionHandlerPathFeature exceptionHandlerFeature = new ExceptionHandlerFeature() { Error = new("Test Error", innerException), Endpoint = endpoint };
            FeatureCollection collection = new();
            collection.Set(exceptionHandlerFeature);
            fakeContextAccessor.HttpContext.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);

            IActionResult result = await controller.ErrorAsync();

            Assert.That(result, Is.InstanceOf<RedirectResult>());
            Assert.That("/MicrosoftIdentity/Account/SignIn", Is.EqualTo(((RedirectResult)result).Url));
        }
    }
}
