using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class FileShareServiceTest
    {
        private IOptions<FileShareServiceConfiguration> fileShareServiceConfig;
        private ILogger<FileShareService> logger;
        private IFileShareApiClient fileShareApiClient;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";
        private const string FakeAccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUyNjg3OTM3LCJuYmYiOjE2NTI2ODc5MzcsImV4cCI6MTY1MjY5MTg4MCwiYWNyIjoiMSIsImFpbyI6IkFYUUFpLzhUQUFBQTVtN2xmWW5CTFhNMURycVI4ZU1DTVJSZGpGTUxBeTdhSjVtSm9OQ1RGNzNCZFNiQUZ2YlMrNGI3S1NKUXFqSlRHemhyR3RKTW5HTXcxQ1I3VWZndmgvck9aTVB0OTh3U1VaVnNBZmozWXU4VEhVQUhXRTFLbDN4ZmFGb25WRVRCdXNhYjJFRXVjdlRtbCtnZG40VHFCUT09IiwiYW1yIjpbInB3ZCIsInJzYSJdLCJhcHBpZCI6IjgwNWJlMDI0LWEyMDgtNDBmYi1hYjZmLTM5OWMyNjQ3ZDMzNCIsImFwcGlkYWNyIjoiMCIsImVtYWlsIjoiU2hpcmluMTQ5MjZAbWFzdGVrLmNvbSIsImZhbWlseV9uYW1lIjoiVGFsYXdkZWthciIsImdpdmVuX25hbWUiOiJTaGlyaW4iLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9hZGQxYzUwMC1hNmQ3LTRkYmQtYjg5MC03ZjhjYjZmN2Q4NjEvIiwiaXBhZGRyIjoiNDkuMzIuMTMyLjE0IiwibmFtZSI6IlNoaXJpbiBUYWxhd2Rla2FyIiwib2lkIjoiM2JjMTlhMzEtMGQ4Zi00ZmIwLWJjZTctYzkwOTcwYzAwOGU5IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FPVS4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiIzQWhSWENMS1lzZGZMNEtMdlZfb05SQUtXX3ZCdWY2N21yZVNwcXFKQmlJIiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJTaGlyaW4xNDkyNkBtYXN0ZWsuY29tIiwidXRpIjoicWN6MDNyVVRVa3FUcFJiZDVUZGtBQSIsInZlciI6IjEuMCJ9.MwYFvGm7ZrfCYdxDmFeocmTYhuqdcMBJJBAKoMlMLmG7HY-IoRFE3al1E2-WEy1zLzsawA9tqqzp0Pr7cYilOaYTylKKqOnaxQfjVdwzjinUtWl0E8y2YtVSS-SxdjuWz0f-wHpPDlm615PFXlkApUTAxrHRsZKUAR6yGDrrndL_lEnGxVIpFKt5-GSptLyzQmBXanxLpuQqvViUSRizOWFmKCeWsGqGDCkvdT9Mn3ogtGFxVd-pec0e323U55VPtk94JJCEumTIvMANXxWMJjtA2CPnuIKWROAY5HxPE2kgYCSdUfArX-5mNs3zuQrzerKyzcMD_tMQISfQ8Tr9lg";

        private IFileShareService fileShareService;

        [SetUp]
        public void Setup()
        {
            fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            logger = A.Fake<ILogger<FileShareService>>();
            fileShareApiClient = A.Fake<IFileShareApiClient>();
            fileShareService = new FileShareService(fileShareServiceConfig, logger);

        }

        [Test]
        public void WhenFileShareServiceCallsFssBatchSearchAsync_ThenReturnsBatchSearchResponse()
        {
            const string searchText = "";
            IResult<BatchSearchResponse> expected = new Result<BatchSearchResponse>();

            A.CallTo(() => fileShareApiClient.SearchAsync("", 100, 0, CancellationToken.None)).Returns(expected);
            Task<IResult<BatchSearchResponse>> result = fileShareService.FSSBatchSearchAsync(searchText, FakeAccessToken, CorrelationId, fileShareApiClient);
            Assert.That(result, Is.InstanceOf<Task<IResult<BatchSearchResponse>>>());
        }

        [Test]
        public void WhenFileShareServiceCallsFssBatchSearchAsyncWithInvalidData_ThenReturnsException()
        {
            fileShareServiceConfig.Value.PageSize = -100;
            fileShareServiceConfig.Value.BaseUrl = "https://www.test.com/";
            A.CallTo(() => fileShareApiClient.SearchAsync(A<string>.Ignored, A<int>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored)).Throws(new ArgumentException());

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>()
                   .And.Message.EqualTo("Value does not fall within the expected range.")
                    , async delegate { await fileShareService.FSSBatchSearchAsync("", "", CorrelationId, fileShareApiClient); });
        }

        [Test]
        public void WhenFileShareServiceCallsFssSearchAttributeAsync_ThenReturnsBatchSearchResponse()
        {
            const int expectedStatusCode = 200;
            const bool expectedStatus = true;
            fileShareServiceConfig.Value.BaseUrl = "https://www.test.com/";
            IResult<BatchAttributesSearchResponse> expectedResponse = new Result<BatchAttributesSearchResponse>()
            {
                IsSuccess = true,
                StatusCode = 200
            };

            A.CallTo(() => fileShareApiClient.BatchAttributeSearchAsync(A<string>.Ignored, A<int>.Ignored, CancellationToken.None)).Returns(expectedResponse);

            Task<IResult<BatchAttributesSearchResponse>> actualResult = fileShareService.FSSSearchAttributeAsync(FakeAccessToken, CorrelationId, fileShareApiClient);

            Assert.That(actualResult, Is.InstanceOf<Task<IResult<BatchAttributesSearchResponse>>>());
            Assert.That(expectedStatusCode, Is.EqualTo(actualResult.Result.StatusCode));
            Assert.That(expectedStatus, Is.EqualTo(actualResult.Result.IsSuccess));
        }

        [Test]
        public void WhenFileShareServiceCallsFssSearchAttributeAsyncWithInvalidData_ThenReturnsException()
        {
            fileShareServiceConfig.Value.BaseUrl = "www.test.com/";
            A.CallTo(() => fileShareApiClient.BatchAttributeSearchAsync(A<string>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored)).Throws(new UriFormatException("Invalid URI: The format of the URI could not be determined."));

            Assert.ThrowsAsync(Is.TypeOf<UriFormatException>()
                .And.Message.EqualTo("Invalid URI: The format of the URI could not be determined.")
                , async delegate { await fileShareService.FSSSearchAttributeAsync("", CorrelationId, fileShareApiClient); });
        }

        [Test]
        public void WhenFSSDownloadFileAsyncIsCalled_ThenShouldReturnByteArray()
        {

            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("test stream"));

            fileShareServiceConfig.Value.BaseUrl = "https://www.test.com/";

            A.CallTo(() => fileShareApiClient.DownloadFileAsync(batchId, fileName))
                .Returns(stream);
            Task<Stream> result = fileShareService.FSSDownloadFileAsync(batchId, fileName, "", CorrelationId, fileShareApiClient, "");
            Assert.That(result, Is.InstanceOf<Task<Stream>>());

        }

        [Test]
        public void WhenFSSDownloadFileAsyncThrowsException_ThenShouldExecuteCatch()
        {
            fileShareServiceConfig.Value.BaseUrl = null;
            A.CallTo(() => fileShareApiClient.DownloadFileAsync(A<string>.Ignored, A<string>.Ignored)).Throws(new Exception());
            Task<Stream> result = fileShareService.FSSDownloadFileAsync("", "", "", CorrelationId, fileShareApiClient, "");
            Assert.That(result.IsFaulted);
        }

        [Test]
        public void WhenFSSDownloadZipFileAsyncIsCalled_ThenShouldReturnByteArray()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";

            IResult<Stream> stream = new Result<Stream>
            {
                Data = new MemoryStream(Encoding.UTF8.GetBytes("test stream")),
                IsSuccess = true
            };

            fileShareServiceConfig.Value.BaseUrl = "https://filesqa.admiralty.co.uk";

            A.CallTo(() => fileShareApiClient.DownloadZipFileAsync(batchId, CancellationToken.None)).Returns(stream);

            Task<Stream> result = fileShareService.FSSDownloadZipFileAsync(batchId, fileName, FakeAccessToken, CorrelationId, fileShareApiClient);

            Assert.That(result, Is.InstanceOf<Task<Stream>>());
        }

        [Test]
        public void WhenFSSDownloadZipFileAsyncThrowsException_ThenShouldExecuteCatch()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";

            fileShareServiceConfig.Value.BaseUrl = null;

            A.CallTo(() => fileShareApiClient.DownloadZipFileAsync(A<string>.Ignored, CancellationToken.None)).Throws(new Exception());

            Task<Stream> result = fileShareService.FSSDownloadZipFileAsync(batchId, fileName, FakeAccessToken, CorrelationId, fileShareApiClient);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public void WhenFSSDownloadZipFileAsyncIsCalled_ThenShouldReturnNull()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";

            fileShareServiceConfig.Value.BaseUrl = null;

            A.CallTo(() => fileShareApiClient.DownloadZipFileAsync(A<string>.Ignored, CancellationToken.None));

            Task<Stream> result = fileShareService.FSSDownloadZipFileAsync(batchId, fileName, FakeAccessToken, CorrelationId, fileShareApiClient);

            Assert.That(result.Result, Is.Null);
        }

        [Test]
        public void WhenFSSDownloadZipFileAsyncIsCalled_ThenShouldThrowArgumentException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";

            IResult<Stream> stream = new Result<Stream>
            {
                Data = new MemoryStream(Encoding.UTF8.GetBytes("test stream")),
                IsSuccess = false
            };
            stream.Errors = new List<Error>() { new Error() { Source = "Test", Description = "Test" } };

            StringBuilder error = new();
            foreach (Error item in stream.Errors)
            {
                error.AppendLine(item.Description);
            }

            A.CallTo(() => fileShareApiClient.DownloadZipFileAsync(A<string>.Ignored, CancellationToken.None)).Returns(stream);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo(error.ToString()),
                async delegate { await fileShareService.FSSDownloadZipFileAsync(batchId, fileName, FakeAccessToken, CorrelationId, fileShareApiClient); });
        }
    }
}
