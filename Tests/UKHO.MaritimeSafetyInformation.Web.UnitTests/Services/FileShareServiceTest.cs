using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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
        private const string FakeAccessToken = "eyJ0eXAiKiJKV1QiLCJkbGciOiJSUzI1NiIsIog1dCI6IgpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUyMDczNjc4LCJuYmYiOjE2NTIwNzM2NzgsImV4cCI6MTY1MjA3ODI4NCwiYWNyIjoiMSIsImFpbyI6IkFYUUFpLzhUQUFBQWlZR3RXQmpZa1p1cWlSa3RFQzlUbkF2RkZrK016aTV4Y3g0UG9Lc0RwTUJWaDZIeWlBd094OENBLzZ1MHZwSDVkYlZHMzVoTkJrL25MRUdETC9hdnNTOUtNU2k0T29tUzFNNzdxTDN6azZCVHQrNUtFYm13bWlQTXgzeUxqdmRoQmRwckplbWZ6dks4bFV2b1BMQTNpZz09IiwiYW1yIjpbInB3ZCIsInJzYSJdLCJhcHBpZCI6IjgwNWJlMDI0LWEyMDgtNDBmYi1hYjZmLTM5OWMyNjQ3ZDMzNCIsImFwcGlkYWNyIjoiMCIsImVtYWlsIjoiU2hpcmluMTQ5MjZAbWFzdGVrLmNvbSIsImZhbWlseV9uYW1lIjoiVGFsYXdkZWthciIsImdpdmVuX25hbWUiOiJTaGlyaW4iLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9hZGQxYzUwMC1hNmQ3LTRkYmQtYjg5MC03ZjhjYjZmN2Q4NjEvIiwiaXBhZGRyIjoiMjcuNjAuMTYxLjE2IiwibmFtZSI6IlNoaXJpbiBUYWxhd2Rla2FyIiwib2lkIjoiM2JjMTlhMzEtMGQ4Zi00ZmIwLWJjZTctYzkwOTcwYzAwOGU5IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FPVS4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiIzQWhSWENMS1lzZGZMNEtMdlZfb05SQUtXX3ZCdWY2N21yZVNwcXFKQmlJIiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJTaGlyaW4xNDkyNkBtYXN0ZWsuY29tIiwidXRpIjoiYnZOZDUzTy0zRXltWm5CWkxJSk9BQSIsInZlciI6IjEuMCJ9.YR1GZ5RqefQPBxPy6JP1tno4ZVCrnsk6gpOo_xi5BQITLgxnx1-WmVBBsqVWtI-CpUurEbPjz16xPBfxTohLEDUlih5RROzANfdzvIUBgZbU5273c-HqyE-DfHNW6w8nzIMqtCq1gKluKIBe_VOPv5O6P7atmt5503UOe6rZPVbUixxnsNsoZnI0NI9lofa44WWTHriRC3QkewvSvS21yP-uQxBaW2ierktpYrYyBn5DmVzW08i5HcWf6SxRQO9uku8eXmHcLITFkEvqKNCQr5LyYSjCfZTw-5ticAe0IdZ7wzN15uYdfMJi7xl30emwa5IVem0GNLnj2WpeWxPxxA";
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
        public async Task WhenFileShareServiceCallsFssBatchSearchAsyncWithInvalidData_ThenReturnsException()
        {
            _fileShareServiceConfig.Value.PageSize = -100;
            A.CallTo(() => _fileShareApiClient.Search(A<string>.Ignored, A<int>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored));
            IResult<BatchSearchResponse> result = await _fileShareService.FssBatchSearchAsync("", "", CorrelationId);
            Assert.That(result.IsSuccess,Is.False);
        }

        [Test]
        public void WhenFileShareServiceCallsFssSearchAttributeAsync_ThenReturnsBatchSearchResponse()
        {                       
            IResult<BatchAttributesSearchResponse> expectedResponse = new Result<BatchAttributesSearchResponse>();
            A.CallTo(() => _fileShareApiClient.BatchAttributeSearch("", CancellationToken.None)).Returns(expectedResponse);
            Task<IResult<BatchAttributesSearchResponse>> result = _fileShareService.FssSearchAttributeAsync(FakeAccessToken, CorrelationId);
            Assert.IsInstanceOf<Task<IResult<BatchAttributesSearchResponse>>>(result);
        }

        [Test]
        public async Task WhenFileShareServiceCallsFssSearchAttributeAsyncWithInvalidData_ThenReturnsException()
        {
            _fileShareServiceConfig.Value.BaseUrl = "www.test.com/";
            A.CallTo(() => _fileShareApiClient.BatchAttributeSearch(A<string>.Ignored, A<CancellationToken>.Ignored));
            IResult<BatchAttributesSearchResponse> result = await _fileShareService.FssSearchAttributeAsync("", CorrelationId);
            Assert.That(result.IsSuccess, Is.False);            
        }
    }
}
