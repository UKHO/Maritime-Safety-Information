using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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
        public void WhenFssWeeklySearchAsyncIsCalled_ThenShouldCheckTypeOfInstance()
        {
            string searchText = "";
            string accessToken = "";

            IResult<BatchSearchResponse> expected = new Result<BatchSearchResponse>();
            A.CallTo(() => _fileShareApiClient.Search("", 100, 0, CancellationToken.None)).Returns(expected);
            Task<IResult<BatchSearchResponse>> result = _fileShareService.FssWeeklySearchAsync(searchText, accessToken);
            Assert.IsInstanceOf<Task<IResult<BatchSearchResponse>>>(result);
        }

        [Test]
        public async Task WhenFssWeeklySearchAsyncIsCalled_ThenShouldExecuteCatch()
        {
            _fileShareServiceConfig.Value.PageSize = -100;
            A.CallTo(() => _fileShareApiClient.Search(A<string>.Ignored, A<int>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored));
            IResult<BatchSearchResponse> result = await _fileShareService.FssWeeklySearchAsync("", "");
            Assert.That(result.IsSuccess,Is.False);
        }
    }
}
