using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsControllerTest
    {
        private IHttpContextAccessor fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsController> fakeLogger;
        private IRNWService fakeRnwService;

        private RadioNavigationalWarningsController controller;

        [SetUp]
        public void Setup()
        {
            fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsController>>();
            fakeRnwService = A.Fake<IRNWService>();

            controller = new RadioNavigationalWarningsController(fakeHttpContextAccessor, fakeLogger, fakeRnwService);
        }

        [Test]
        public async Task WhenICallIndexView_ThenReturnView()
        {
            const string expectedView = "~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml";

            A.CallTo(() => fakeRnwService.GetRadioNavigationalWarningsData(A<string>.Ignored)).Returns(new List<RadioNavigationalWarningsData>());
            A.CallTo(() => fakeRnwService.GetRadioNavigationalWarningsLastModifiedDateTime(A<string>.Ignored)).Returns(DateTime.UtcNow.ToString());

            IActionResult result = await controller.Index();

            Assert.That(result, Is.InstanceOf<IActionResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(controller.ViewBag.LastModifiedDateTime, Is.Not.Empty);
            Assert.That(false, Is.EqualTo(controller.ViewBag.HasError));
        }

        [Test]
        public async Task WhenICallIndexViewAndExceptionThrownByService_ThenShouldReturnExpectedViewWithViewData()
        {
            const string expectedView = "~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml";

            A.CallTo(() => fakeRnwService.GetRadioNavigationalWarningsLastModifiedDateTime(A<string>.Ignored)).Returns(DateTime.UtcNow.ToString());
            A.CallTo(() => fakeRnwService.GetRadioNavigationalWarningsData(A<string>.Ignored)).Throws(new Exception());

            IActionResult result = await controller.Index();

            Assert.That(result, Is.InstanceOf<IActionResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
            Assert.That(controller.ViewBag.LastModifiedDateTime, Is.Not.Empty);
            Assert.That(true, Is.EqualTo(controller.ViewBag.HasError));
        }

        [Test]
        public async Task WhenCallAbout_ThenReturnView()
        {
            A.CallTo(() => fakeRnwService.GetRadioNavigationalWarningsLastModifiedDateTime(A<string>.Ignored)).Returns(DateTime.UtcNow.ToString());

            IActionResult result = await controller.About();
            Assert.That(controller.ViewBag.LastModifiedDateTime, Is.Not.Empty);
            Assert.That(result, Is.InstanceOf<IActionResult>());
        }

        [Test]
        public async Task WhenCallShowSelection_ThenReturnView()
        {
            DefaultHttpContext httpContext = new();
            const string expectedView = "~/Views/RadioNavigationalWarnings/ShowSelection.cshtml";
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"showSelectionId", "1,2,3" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => fakeRnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty)).Returns(new List<RadioNavigationalWarningsData>());

            IActionResult result = await controller.ShowSelection();

            Assert.That(result, Is.InstanceOf<IActionResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(false, Is.EqualTo(controller.ViewBag.HasError));
        }

        [Test]
        public async Task WhenCallShowSelectionAndExceptionThrown_ThenReturnView()
        {
            DefaultHttpContext httpContext = new();
            const string expectedView = "~/Views/RadioNavigationalWarnings/ShowSelection.cshtml";

            A.CallTo(() => fakeRnwService.GetRadioNavigationalWarningsLastModifiedDateTime(A<string>.Ignored)).Returns(DateTime.UtcNow.ToString());
            A.CallTo(() => fakeRnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty)).Throws(new Exception());

            IActionResult result = await controller.ShowSelection();

            Assert.That(result, Is.InstanceOf<IActionResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(true, Is.EqualTo(controller.ViewBag.HasError));
            Assert.That(controller.ViewBag.LastModifiedDateTime, Is.Not.Empty);
        }
    }
}
