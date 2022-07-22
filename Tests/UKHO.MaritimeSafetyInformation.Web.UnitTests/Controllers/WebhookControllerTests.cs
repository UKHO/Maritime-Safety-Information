using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Linq;
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
        private IWebhookService _fakeWebhookService;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<WebhookController>>();
            _fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeWebhookService = A.Fake<IWebhookService>();
            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            _controller = new WebhookController(_fakeContextAccessor, _fakeLogger, _fakeWebhookService);
        }

        [Test]
        public void WhenValidHeaderRequestedInNewFilesPublishedOptions_ThenReturnsOkResponse()
        {
            var context = new DefaultHttpContext();
            string requestHeaderValue = "test.example.com";
            context.Request.Headers["WebHook-Request-Origin"] = requestHeaderValue;

            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(context);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };

            var result = (OkObjectResult)_controller.NewFilesPublishedOptions();

            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("*", _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Rate").Select(b => b.Value).FirstOrDefault());
            Assert.AreEqual(requestHeaderValue, _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Origin").Select(b => b.Value).FirstOrDefault());
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
