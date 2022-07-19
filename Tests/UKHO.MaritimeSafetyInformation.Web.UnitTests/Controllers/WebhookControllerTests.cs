using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class WebhookControllerTest
    {
        private WebhookController _controller;
        private ILogger<WebhookController> _fakeLogger;
        private IHttpContextAccessor _fakeContextAccessor;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<WebhookController>>();
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new WebhookController(_fakeContextAccessor, _fakeLogger);
        }

        [Test]
        public void WhenValidHeaderRequestedInNewFilesPublishedOptions_ThenReturnsOkResponse()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _fakeContextAccessor.HttpContext.Request.Headers.Add("WebHook-Request-Origin", "test.example.com");

            var result = (OkObjectResult)_controller.NewFilesPublishedOptions();

            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalled_ThenReturnsOkResponse()
        {
            var fakeCacheJson = JObject.Parse(@"{""Type"":""FilesPublished""}");

            var result = (OkObjectResult)await _controller.NewFilesPublished(fakeCacheJson);

            Assert.AreEqual(200, result.StatusCode);
        }
    }
}
