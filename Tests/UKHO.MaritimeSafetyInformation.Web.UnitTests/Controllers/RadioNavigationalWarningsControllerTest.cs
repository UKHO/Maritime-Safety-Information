using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
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
        private IRnwService _fakeRnwService;

        private RadioNavigationalWarningsController _controller;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsController>>();
            _fakeRnwService = A.Fake<IRnwService>();

            _controller = new RadioNavigationalWarningsController(_fakeHttpContextAccessor, _fakeLogger, _fakeRnwService);
        }

        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            A.CallTo(() => _fakeRnwService.GetRadioNavigationalWarningsData(A<string>.Ignored)).Returns( new List<RadioNavigationalWarningsData>());

            Task<IActionResult> result = _controller.Index();

            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }
    }
}
