using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class FileShareServiceTest
    {
        private IHttpClientFactory _httpClientFactory;
        private IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private ILogger<FileShareService> _logger;
        private FileShareService _fileShareService;
        private IFileShareApiClient _fileShareApiClient;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";
        public const string FakeAccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUyNjg3OTM3LCJuYmYiOjE2NTI2ODc5MzcsImV4cCI6MTY1MjY5MTg4MCwiYWNyIjoiMSIsImFpbyI6IkFYUUFpLzhUQUFBQTVtN2xmWW5CTFhNMURycVI4ZU1DTVJSZGpGTUxBeTdhSjVtSm9OQ1RGNzNCZFNiQUZ2YlMrNGI3S1NKUXFqSlRHemhyR3RKTW5HTXcxQ1I3VWZndmgvck9aTVB0OTh3U1VaVnNBZmozWXU4VEhVQUhXRTFLbDN4ZmFGb25WRVRCdXNhYjJFRXVjdlRtbCtnZG40VHFCUT09IiwiYW1yIjpbInB3ZCIsInJzYSJdLCJhcHBpZCI6IjgwNWJlMDI0LWEyMDgtNDBmYi1hYjZmLTM5OWMyNjQ3ZDMzNCIsImFwcGlkYWNyIjoiMCIsImVtYWlsIjoiU2hpcmluMTQ5MjZAbWFzdGVrLmNvbSIsImZhbWlseV9uYW1lIjoiVGFsYXdkZWthciIsImdpdmVuX25hbWUiOiJTaGlyaW4iLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9hZGQxYzUwMC1hNmQ3LTRkYmQtYjg5MC03ZjhjYjZmN2Q4NjEvIiwiaXBhZGRyIjoiNDkuMzIuMTMyLjE0IiwibmFtZSI6IlNoaXJpbiBUYWxhd2Rla2FyIiwib2lkIjoiM2JjMTlhMzEtMGQ4Zi00ZmIwLWJjZTctYzkwOTcwYzAwOGU5IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FPVS4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiIzQWhSWENMS1lzZGZMNEtMdlZfb05SQUtXX3ZCdWY2N21yZVNwcXFKQmlJIiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJTaGlyaW4xNDkyNkBtYXN0ZWsuY29tIiwidXRpIjoicWN6MDNyVVRVa3FUcFJiZDVUZGtBQSIsInZlciI6IjEuMCJ9.MwYFvGm7ZrfCYdxDmFeocmTYhuqdcMBJJBAKoMlMLmG7HY-IoRFE3al1E2-WEy1zLzsawA9tqqzp0Pr7cYilOaYTylKKqOnaxQfjVdwzjinUtWl0E8y2YtVSS-SxdjuWz0f-wHpPDlm615PFXlkApUTAxrHRsZKUAR6yGDrrndL_lEnGxVIpFKt5-GSptLyzQmBXanxLpuQqvViUSRizOWFmKCeWsGqGDCkvdT9Mn3ogtGFxVd-pec0e323U55VPtk94JJCEumTIvMANXxWMJjtA2CPnuIKWROAY5HxPE2kgYCSdUfArX-5mNs3zuQrzerKyzcMD_tMQISfQ8Tr9lg";

        public static IConfiguration InitConfiguration()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            _logger = A.Fake<ILogger<FileShareService>>();
            IConfiguration config = InitConfiguration().GetSection("FileShareService");
            _fileShareServiceConfig = Options.Create(config.Get<FileShareServiceConfiguration>());
            _fileShareApiClient = A.Fake<IFileShareApiClient>();
            _fileShareService = new FileShareService(_httpClientFactory, _fileShareServiceConfig, _logger);

        }

        [Test]
        public void WhenFileShareServiceCallsFssBatchSearchAsync_ThenReturnsBatchSearchResponse()
        {
            string searchText = "";
            IResult<BatchSearchResponse> expected = new Result<BatchSearchResponse>();

            A.CallTo(() => _fileShareApiClient.Search("", 100, 0, CancellationToken.None)).Returns(expected);
            Task<IResult<BatchSearchResponse>> result = _fileShareService.FssBatchSearchAsync(searchText, FakeAccessToken, CorrelationId);
            Assert.IsInstanceOf<Task<IResult<BatchSearchResponse>>>(result);            
        }

        [Test]
        public void WhenFileShareServiceCallsFssBatchSearchAsyncWithInvalidData_ThenReturnsException()
        {
            _fileShareServiceConfig.Value.PageSize = -100;
            _fileShareServiceConfig.Value.BaseUrl = "https://www.test.com/";
            A.CallTo(() => _fileShareApiClient.Search(A<string>.Ignored, A<int>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored));

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>()
                   .And.Message.EqualTo("Page size must be greater than zero. (Parameter 'pageSize')")
                    , async delegate { await _fileShareService.FssBatchSearchAsync("", "", CorrelationId); });
        }

        [Test]
        public void WhenFileShareServiceCallsFssSearchAttributeAsync_ThenReturnsBatchSearchResponse()
        {
            int ExpectedStatusCode = 200;
            bool ExpectedStatus = true;
            _fileShareServiceConfig.Value.BaseUrl = "https://www.test.com/";
            IResult<BatchAttributesSearchResponse> expectedResponse = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => _fileShareApiClient.BatchAttributeSearch("", CancellationToken.None)).Returns(expectedResponse);

            Task<IResult<BatchAttributesSearchResponse>> result = _fileShareService.FssSearchAttributeAsync(FakeAccessToken, CorrelationId);

            Assert.IsInstanceOf<Task<IResult<BatchAttributesSearchResponse>>>(result);
            Assert.AreEqual(result.Result.StatusCode, ExpectedStatusCode);
            Assert.AreEqual(result.Result.IsSuccess, ExpectedStatus);
        }

        [Test]
        public void WhenFileShareServiceCallsFssSearchAttributeAsyncWithInvalidData_ThenReturnsException()
        {
            _fileShareServiceConfig.Value.BaseUrl = "www.test.com/";
            A.CallTo(() => _fileShareApiClient.BatchAttributeSearch(A<string>.Ignored, A<CancellationToken>.Ignored)).Throws(new UriFormatException("Invalid URI: The format of the URI could not be determined."));

            Assert.ThrowsAsync(Is.TypeOf<UriFormatException>()
                .And.Message.EqualTo("Invalid URI: The format of the URI could not be determined.")
                , async delegate { await _fileShareService.FssSearchAttributeAsync("", CorrelationId); });
        }
    }
}
