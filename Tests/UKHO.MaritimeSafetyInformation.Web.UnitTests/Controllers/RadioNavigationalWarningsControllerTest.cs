using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsControllerTest
    {
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsController> _fakeLogger;
        private IRNWService _fakeRnwService;
        private IMSIBannerNotificationService _fakeMSIBannerNotificationService;

        private RadioNavigationalWarningsController _controller;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsController>>();
            _fakeRnwService = A.Fake<IRNWService>();
            _fakeMSIBannerNotificationService = A.Fake<IMSIBannerNotificationService>();

            _controller = new RadioNavigationalWarningsController(_fakeHttpContextAccessor, _fakeLogger, _fakeRnwService, _fakeMSIBannerNotificationService);
        }

        [Test]
        public async Task WhenICallIndexView_ThenReturnView()
        {
            const string expectedView = "~/Views/RadioNavigationalWarnings/ShowRadioNavigationalWarnings.cshtml";

            A.CallTo(() => _fakeRnwService.GetRadioNavigationalWarningsData(A<string>.Ignored)).Returns( new List<RadioNavigationalWarningsData>());

            IActionResult result = await _controller.Index();

            Assert.IsInstanceOf<IActionResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenICallIndexView_ThenReturnException()
        {
            A.CallTo(() => _fakeRnwService.GetRadioNavigationalWarningsData(A<string>.Ignored)).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _controller.Index(); });
        }

        [Test]
        public async Task WhenCallAbout_ThenReturnView()
        {
            A.CallTo(() => _fakeRnwService.GetRadioNavigationalWarningsData(A<string>.Ignored)).Returns(new List<RadioNavigationalWarningsData>());

            IActionResult result = await _controller.About();

            Assert.IsInstanceOf<IActionResult>(result);
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
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _fakeRnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty)).Returns(new List<RadioNavigationalWarningsData>());
            
            IActionResult result = await _controller.ShowSelection();

            Assert.IsInstanceOf<IActionResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }
    }
}
