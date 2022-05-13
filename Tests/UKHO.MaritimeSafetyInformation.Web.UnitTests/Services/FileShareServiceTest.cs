using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.IO;
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
            string accessToken = "";

            IResult<BatchSearchResponse> expected = new Result<BatchSearchResponse>();
           
            A.CallTo(() => _fileShareApiClient.Search("", 100, 0, CancellationToken.None)).Returns(expected);
            Task<IResult<BatchSearchResponse>> result = _fileShareService.FssBatchSearchAsync(searchText, accessToken , CorrelationId);
            Assert.IsInstanceOf<Task<IResult<BatchSearchResponse>>>(result);
        }

        [Test]
        public void WhenFileShareServiceCallsFssBatchSearchAsyncWithInvalidData_ThenReturnsException()
        {
            _fileShareServiceConfig.Value.PageSize = -100;
            _fileShareServiceConfig.Value.BaseUrl =  "https://filesqa.admiralty.co.uk";
            A.CallTo(() => _fileShareApiClient.Search(A<string>.Ignored, A<int>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored));
            Task<IResult<BatchSearchResponse>> result =  _fileShareService.FssBatchSearchAsync("", "", CorrelationId);
            Assert.That(result.IsFaulted, Is.True);
        }

        [Test]
        public void WhenFSSDownloadFileAsyncIsCalled_ThenShouldReturnByteArray()
        {
                Stream stream = new System.IO.MemoryStream();

                A.CallTo(() => _fileShareApiClient.DownloadFileAsync(A<string>.Ignored, A<string>.Ignored)).Returns(stream);
                var result = _fileShareService.FSSDownloadFileAsync("", "","", CorrelationId);
                Assert.IsInstanceOf<Task<byte[]>>(result);

        }

        [Test]
        public void WhenFSSDownloadFileAsyncIsCalled_ThenShouldExecuteCatch()
        {
            Stream stream = Stream.Null;

            A.CallTo(() => _fileShareApiClient.DownloadFileAsync(A<string>.Ignored, A<string>.Ignored)).Returns(stream);
            var result = _fileShareService.FSSDownloadFileAsync("", "", "", CorrelationId);
            Assert.That(result.IsFaulted, Is.True);
        }
    }
}
