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
using System;
using System.Security.Claims;
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
        private IOptions<AzureAdB2C> _fakeOptions;
        private Endpoint _endpoint;

        [SetUp]
        public void Setup()
        {
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<HomeController>>();
            _fakeOptions = A.Fake<IOptions<AzureAdB2C>>();
            IAuthenticationService fakeAuthService = A.Fake<IAuthenticationService>();
            A.CallTo(() => fakeAuthService.SignInAsync(A<HttpContext>.Ignored, A<string>.Ignored, A<ClaimsPrincipal>.Ignored, A<AuthenticationProperties>.Ignored)).Returns(Task.FromResult((object)null));
            IServiceProvider fakeServiceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => fakeServiceProvider.GetService(typeof(IAuthenticationService))).Returns(fakeAuthService);

            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext { RequestServices = fakeServiceProvider });

            RequestDelegate requestDelegate = null;
            ControllerActionDescriptor controllerActionDescriptor = new() { ActionName = "Index", ControllerName = "TestController" };
            EndpointMetadataCollection endpointMetadataCollection = new(controllerActionDescriptor);
            _endpoint = new(requestDelegate, endpointMetadataCollection, "Test Eror");

            _controller = new HomeController(_fakeContextAccessor, _fakeLogger, _fakeOptions);
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
            Exception innerException = new();
            IExceptionHandlerPathFeature exceptionHandlerFeature = new ExceptionHandlerFeature() { Error = new("Test Error", innerException), Endpoint = _endpoint };
            FeatureCollection collection = new();
            collection.Set(exceptionHandlerFeature);
            _fakeContextAccessor.HttpContext.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);

            IActionResult result = await _controller.ErrorAsync();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsTrue(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
            Assert.IsTrue(((ViewResult)result).ViewData.ContainsKey("ControllerName"));
            Assert.IsTrue(((ViewResult)result).ViewData.ContainsKey("ActionName"));
        }

        [Test]
        public async Task WhenErrorIsCalledWithInnerExceptionMsalUiRequiredException_ThenShouldRedirectToLogin()
        {
            MsalUiRequiredException innerException = new("1001", "MsalUiRequiredException");
            IExceptionHandlerPathFeature exceptionHandlerFeature = new ExceptionHandlerFeature() { Error = new("Test Error", innerException), Endpoint = _endpoint };
            FeatureCollection collection = new();
            collection.Set(exceptionHandlerFeature);
            _fakeContextAccessor.HttpContext.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);

            IActionResult result = await _controller.ErrorAsync();

            Assert.IsInstanceOf<RedirectResult>(result);
            Assert.AreEqual("/MicrosoftIdentity/Account/SignIn", ((RedirectResult)result).Url);
        }
    }
}
