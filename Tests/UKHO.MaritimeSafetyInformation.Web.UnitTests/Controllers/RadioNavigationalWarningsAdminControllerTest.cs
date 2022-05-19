using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsAdminControllerTest
    {
        private RadioNavigationalWarningsAdminController _controller;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWService _fakeRnwService;

       [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _fakeRnwService = A.Fake<IRNWService>();

            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _fakeRnwService);
        }

        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            Task<IActionResult> result = _controller.Index();
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        [Test]
        public void WhenICallCreateView_ThenReturnView()
        {
            Task<IActionResult> result = _controller.Create();
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsReturnTrueInRequest_ThenNewRecordIsCreated()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, A.Fake<ITempDataProvider>());
            _controller.TempData = tempData;

            A.CallTo(() => _fakeRnwService.CreateNewRadioNavigationWarningsRecord(A<RadioNavigationalWarnings>.Ignored, A<string>.Ignored)).Returns(true);
            Task<IActionResult> result = _controller.Create(new RadioNavigationalWarnings());

            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.AreEqual("Record created successfully!", _controller.TempData["message"].ToString());
        }
    }
}
