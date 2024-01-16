using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class WebhookControllerTest
    {
        private WebhookController controller;
        private ILogger<WebhookController> fakeLogger;
        private IHttpContextAccessor fakeContextAccessor;
        private IWebhookService fakeWebhookService;

        private MemoryStream requestData;

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<WebhookController>>();
            fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            fakeWebhookService = A.Fake<IWebhookService>();
            A.CallTo(() => fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            controller = new WebhookController(fakeContextAccessor, fakeLogger, fakeWebhookService);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            string jsonString = GetRequestString();
            requestData = new(Encoding.UTF8.GetBytes(jsonString));
            controller.HttpContext.Request.Body = requestData;

        }

        [Test]
        public void WhenValidHeaderRequestedInNewFilesPublishedOptions_ThenShouldReturnOkResponse()
        {
            DefaultHttpContext context = new();
            string requestHeaderValue = "test.example.com";
            context.Request.Headers["WebHook-Request-Origin"] = requestHeaderValue;

            A.CallTo(() => fakeContextAccessor.HttpContext).Returns(context);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };

            OkObjectResult result = (OkObjectResult)controller.NewFilesPublishedOptions();

            Assert.That(200, Is.EqualTo(result.StatusCode));
            Assert.That("*", Is.EqualTo(controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Rate").Select(b => b.Value).FirstOrDefault()));
            Assert.That(requestHeaderValue, Is.EqualTo(controller.HttpContext.Response.Headers.Where(a => a.Key == "WebHook-Allowed-Origin").Select(b => b.Value).FirstOrDefault()));
        }


        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithEmptyPayload_ThenShouldNotDeleteCache()
        {
            controller.HttpContext.Request.Body = new MemoryStream();

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithEmptyPayloadData_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithNoData();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithEmptyPayloadDataString_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithEmptyDataString();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithNullPayloadData_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithNullData();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithValidData_ThenShouldReturnOkResponse()
        {
            A.CallTo(() => fakeWebhookService.ValidateNewFilesPublishedEventData(A<FSSNewFilesPublishedEventData>.Ignored)).Returns(new ValidationResult());
            A.CallTo(() => fakeWebhookService.DeleteBatchSearchResponseCacheData(A<FSSNewFilesPublishedEventData>.Ignored, A<string>.Ignored)).Returns(true);

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithValidDataAndDifferentBusinessUnit_ThenShouldReturnOkResponse()
        {
            A.CallTo(() => fakeWebhookService.ValidateNewFilesPublishedEventData(A<FSSNewFilesPublishedEventData>.Ignored)).Returns(new ValidationResult());
            A.CallTo(() => fakeWebhookService.DeleteBatchSearchResponseCacheData(A<FSSNewFilesPublishedEventData>.Ignored, A<string>.Ignored)).Returns(false);

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithInvalidData_ThenDeleteCallShouldNotHappen()
        {
            A.CallTo(() => fakeWebhookService.ValidateNewFilesPublishedEventData(A<FSSNewFilesPublishedEventData>.Ignored)).Returns(new ValidationResult(new List<ValidationFailure> { new ValidationFailure() }));
            A.CallTo(() => fakeWebhookService.DeleteBatchSearchResponseCacheData(A<FSSNewFilesPublishedEventData>.Ignored, A<string>.Ignored));

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithNullProductType_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithNullProductType();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithProductTypeAttributeMissing_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithProductTypeAttributeMissing();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithNoAttributes_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithNoAttributes();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithAttributeValueAsNull_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithAttributeValueAsNull();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        [Test]
        public async Task WhenNewFilesPublishedIsCalledWithAttributeMissing_ThenShouldNotDeleteCache()
        {
            string jsonString = GetRequestStringWithAttributeMissing();
            MemoryStream requestData = new(Encoding.UTF8.GetBytes(jsonString));

            controller.HttpContext.Request.Body = requestData;

            OkObjectResult result = (OkObjectResult)await controller.NewFilesPublished();

            Assert.That(200, Is.EqualTo(result.StatusCode));
        }

        private static string GetRequestString()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\": {"
                                +"\"BatchId\": \"83d08093-7a67-4b3a-b431-92ba42feqw12\","
                                + "\"BusinessUnit\": \"MaritimeSafetyInformation\","
                                +"\"Links\": {"
                                        +"\"BatchDetails\": {"
                                                        +"\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0\""
                                                  +"},"
                                        +"\"BatchStatus\": {"
                                                        +"\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status\""
                                                   +"}"
                                +"},"
                                + "\"Attributes\": ["
                                        + "{"
                                        + "\"key\": \"CellName\","
                                        + "\"value\": \"Notices to Mariners\""
                                        + "},"
                                        + "{"
                                        + "\"key\": \"Product Type\","
                                        + "\"value\": \"Notices to Mariners\""
                                        + "}"
                                    + "],"
                                +"\"BatchPublishedDate\": \"2022-04-04T11:22:18.2943076Z\","

                                +"\"Files\": ["
                                                +"{"
                                                +"\"FileName\": \"S631-1_Update_Wk45_21_Only.zip\","
                                                +"\"FileSize\": 99073923,"
                                                +"\"MIMEType\": \"application/zip\","
                                                +"\"Hash\": \"yNpJTWFKhD3iasV8B/ePKw==\","
                                                +"\"Attributes\": [],"
                                                +"\"Links\": {"
                                                                +"\"Get\": {"
                                                                            +"\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip\""
                                                                            +"}"
                                                            +"}"
                                                 +"}"
                                              +"]"

                             + "}"
                    
                    + "}";
        }

        private static string GetRequestStringWithNoData()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\": {"
                             + "}"
                    + "}";
        }

        private static string GetRequestStringWithEmptyDataString()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\":\"\""
                    + "}";
        }

        private static string GetRequestStringWithNullData()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "}";
        }

        private static string GetRequestStringWithNullProductType()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\": {"
                                + "\"BatchId\": \"83d08093-7a67-4b3a-b431-92ba42feqw12\","
                                + "\"BusinessUnit\": \"MaritimeSafetyInformation\","
                                + "\"Links\": {"
                                        + "\"BatchDetails\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0\""
                                                  + "},"
                                        + "\"BatchStatus\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status\""
                                                   + "}"
                                + "},"
                                + "\"Attributes\": ["
                                        + "{"
                                        + "\"key\": \"Product Type\","
                                        + "\"value\": null"
                                        + "}"
                                    + "],"
                                + "\"BatchPublishedDate\": \"2022-04-04T11:22:18.2943076Z\","

                                + "\"Files\": ["
                                                + "{"
                                                + "\"FileName\": \"S631-1_Update_Wk45_21_Only.zip\","
                                                + "\"FileSize\": 99073923,"
                                                + "\"MIMEType\": \"application/zip\","
                                                + "\"Hash\": \"yNpJTWFKhD3iasV8B/ePKw==\","
                                                + "\"Attributes\": [],"
                                                + "\"Links\": {"
                                                                + "\"Get\": {"
                                                                            + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip\""
                                                                            + "}"
                                                            + "}"
                                                 + "}"
                                              + "]"

                             + "}"

                    + "}";
        }

        private static string GetRequestStringWithProductTypeAttributeMissing()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\": {"
                                + "\"BatchId\": \"83d08093-7a67-4b3a-b431-92ba42feqw12\","
                                + "\"BusinessUnit\": \"MaritimeSafetyInformation\","
                                + "\"Links\": {"
                                        + "\"BatchDetails\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0\""
                                                  + "},"
                                        + "\"BatchStatus\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status\""
                                                   + "}"
                                + "},"
                                + "\"Attributes\": ["
                                        + "{"
                                        + "\"key\": \"Cell\","
                                        + "\"value\": \"Test\""
                                        + "}"
                                    + "],"
                                + "\"BatchPublishedDate\": \"2022-04-04T11:22:18.2943076Z\","

                                + "\"Files\": ["
                                                + "{"
                                                + "\"FileName\": \"S631-1_Update_Wk45_21_Only.zip\","
                                                + "\"FileSize\": 99073923,"
                                                + "\"MIMEType\": \"application/zip\","
                                                + "\"Hash\": \"yNpJTWFKhD3iasV8B/ePKw==\","
                                                + "\"Attributes\": [],"
                                                + "\"Links\": {"
                                                                + "\"Get\": {"
                                                                            + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip\""
                                                                            + "}"
                                                            + "}"
                                                 + "}"
                                              + "]"

                             + "}"

                    + "}";
        }

        private static string GetRequestStringWithNoAttributes()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\": {"
                                + "\"BatchId\": \"83d08093-7a67-4b3a-b431-92ba42feqw12\","
                                + "\"BusinessUnit\": \"MaritimeSafetyInformation\","
                                + "\"Links\": {"
                                        + "\"BatchDetails\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0\""
                                                  + "},"
                                        + "\"BatchStatus\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status\""
                                                   + "}"
                                + "},"
                                + "\"Attributes\": ["
                                    + "],"
                                + "\"BatchPublishedDate\": \"2022-04-04T11:22:18.2943076Z\","

                                + "\"Files\": ["
                                                + "{"
                                                + "\"FileName\": \"S631-1_Update_Wk45_21_Only.zip\","
                                                + "\"FileSize\": 99073923,"
                                                + "\"MIMEType\": \"application/zip\","
                                                + "\"Hash\": \"yNpJTWFKhD3iasV8B/ePKw==\","
                                                + "\"Attributes\": [],"
                                                + "\"Links\": {"
                                                                + "\"Get\": {"
                                                                            + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip\""
                                                                            + "}"
                                                            + "}"
                                                 + "}"
                                              + "]"

                             + "}"

                    + "}";
        }

        private static string GetRequestStringWithAttributeValueAsNull()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\": {"
                                + "\"BatchId\": \"83d08093-7a67-4b3a-b431-92ba42feqw12\","
                                + "\"BusinessUnit\": \"MaritimeSafetyInformation\","
                                + "\"Links\": {"
                                        + "\"BatchDetails\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0\""
                                                  + "},"
                                        + "\"BatchStatus\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status\""
                                                   + "}"
                                + "},"
                                + "\"Attributes\": null,"
                                + "\"BatchPublishedDate\": \"2022-04-04T11:22:18.2943076Z\","
                                + "\"Files\": ["
                                                + "{"
                                                + "\"FileName\": \"S631-1_Update_Wk45_21_Only.zip\","
                                                + "\"FileSize\": 99073923,"
                                                + "\"MIMEType\": \"application/zip\","
                                                + "\"Hash\": \"yNpJTWFKhD3iasV8B/ePKw==\","
                                                + "\"Attributes\": [],"
                                                + "\"Links\": {"
                                                                + "\"Get\": {"
                                                                            + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip\""
                                                                            + "}"
                                                            + "}"
                                                 + "}"
                                              + "]"

                             + "}"

                    + "}";
        }

        private static string GetRequestStringWithAttributeMissing()
        {
            return "{"
                    + "\"Type\": \"uk.gov.UKHO.FileShareService.NewFilesPublished.v1\","
                    + "\"Time\": \"2021-11-09T14:52:28+00:00\","
                    + "\"DataContentType\": \"application/json\","
                    + "\"DataSchema\": null,"
                    + "\"Subject\": \"83d08093-7a67-4b3a-b431-92ba42feaea0\","
                    + "\"Source\": \"https://files.admiralty.co.uk\","
                    + "\"Id\": \"49c67cca-9cca-4655-a38e-583693af55ea\","
                    + "\"Data\": {"
                                + "\"BatchId\": \"83d08093-7a67-4b3a-b431-92ba42feqw12\","
                                + "\"BusinessUnit\": \"MaritimeSafetyInformation\","
                                + "\"Links\": {"
                                        + "\"BatchDetails\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0\""
                                                  + "},"
                                        + "\"BatchStatus\": {"
                                                        + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/status\""
                                                   + "}"
                                + "},"
                                + "\"BatchPublishedDate\": \"2022-04-04T11:22:18.2943076Z\","
                                + "\"Files\": ["
                                                + "{"
                                                + "\"FileName\": \"S631-1_Update_Wk45_21_Only.zip\","
                                                + "\"FileSize\": 99073923,"
                                                + "\"MIMEType\": \"application/zip\","
                                                + "\"Hash\": \"yNpJTWFKhD3iasV8B/ePKw==\","
                                                + "\"Attributes\": [],"
                                                + "\"Links\": {"
                                                                + "\"Get\": {"
                                                                            + "\"Href\": \"https://filesqa.admiralty.co.uk/batch/83d08093-7a67-4b3a-b431-92ba42feaea0/files/S631-1_Update_Wk45_21_Only.zip\""
                                                                            + "}"
                                                            + "}"
                                                 + "}"
                                              + "]"

                             + "}"

                    + "}";
        }
    }
}
