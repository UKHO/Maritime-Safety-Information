using FakeItEasy;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;
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
        public void WhenValidHeaderRequestedInNewFilesPublishedOptions_ThenShouldReturnOkResponse()
        {
            DefaultHttpContext context = new();
            string requestHeaderValue = "test.example.com";
            context.Request.Headers["WebHook-Request-Origin"] = requestHeaderValue;

            A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(context);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };

            OkObjectResult result = (OkObjectResult)_controller.NewFilesPublishedOptions();

            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("*", _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Rate").Select(b => b.Value).FirstOrDefault());
            Assert.AreEqual(requestHeaderValue, _controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Origin").Select(b => b.Value).FirstOrDefault());
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithValidData_ThenShouldReturnOkResponse()
        {
            string jsonString = GetRequestString();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = requestData;

            A.CallTo(() => _fakeWebhookService.ValidateEventGridCacheDataRequest(A<EnterpriseEventCacheDataRequest>.Ignored)).Returns(new ValidationResult());
            A.CallTo(() => _fakeWebhookService.DeleteSearchAndDownloadCacheData(A<EnterpriseEventCacheDataRequest>.Ignored, A<string>.Ignored)).Returns(true);

            OkObjectResult result = (OkObjectResult)await _controller.NewFilesPublished();

            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithValidDataAndDifferentBusinessUnit_ThenShouldReturnOkResponse()
        {
            string jsonString = GetRequestString();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = requestData;

            A.CallTo(() => _fakeWebhookService.ValidateEventGridCacheDataRequest(A<EnterpriseEventCacheDataRequest>.Ignored)).Returns(new ValidationResult());
            A.CallTo(() => _fakeWebhookService.DeleteSearchAndDownloadCacheData(A<EnterpriseEventCacheDataRequest>.Ignored, A<string>.Ignored)).Returns(false);

            OkObjectResult result = (OkObjectResult)await _controller.NewFilesPublished();

            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithInvalidData_ThenDeleteCallShouldNotHappen()
        {
            string jsonString = GetRequestString();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Request.Body = requestData;
            
            A.CallTo(() => _fakeWebhookService.ValidateEventGridCacheDataRequest(A<EnterpriseEventCacheDataRequest>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure> { new ValidationFailure() }));
            A.CallTo(() => _fakeWebhookService.DeleteSearchAndDownloadCacheData(A<EnterpriseEventCacheDataRequest>.Ignored, A<string>.Ignored));

            OkObjectResult result = (OkObjectResult)await _controller.NewFilesPublished();

            Assert.AreEqual(200, result.StatusCode);
        }

        private static string GetRequestString()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"Data\": {"
                                + "\"BusinessUnit\": \"MaritimeSafetyInformation\","
                    + "\"Attributes\": ["
                                        + "{"
                                        + "\"key\": \"CellName\","
                                        + "\"value\": \"Notices to Mariners\""
                                        + "},"
                                        + "{"
                                        + "\"key\": \"ProductCode\","
                                        + "\"value\": \"Notices to Mariners\""
                                        + "}"
                                    + "]"
                    + "},"
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\""
                    + "}";
        }
    }
}
