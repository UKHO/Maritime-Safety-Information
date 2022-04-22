using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using NUnit.Framework;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Web.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class FileShareServiceTest
    {
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        private FileShareServiceConfiguration _fileShareServiceConfig;
        private ILogger<FileShareService> _logger;
        private ILogger<NMDataService> _loggerNM;
        private FileShareService _fileShareService;
        private NMDataService _fakeNMDataService;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _fileShareServiceConfig = A.Fake<FileShareServiceConfiguration>();
            _configuration = A.Fake<IConfiguration>();
            _logger = A.Fake<ILogger<FileShareService>>();
            _fileShareService = new FileShareService(_httpClientFactory,_configuration,_logger);
            _fakeNMDataService = new NMDataService(_fileShareService, _httpClientFactory, _configuration, _loggerNM);

        }

        [Test]
        public async Task DoesFssWeeklySearchReturnBatchResponseAsync()
        {
            AuthFssTokenProvider authFssTokenProvider = new AuthFssTokenProvider();
            AuthenticationResult authentication = await authFssTokenProvider.GetAuthTokenAsync();
            string accessToken = authentication.AccessToken;
            int year = 2022, week = 16;
            string searchText = $"BusinessUnit eq 'Test' and $batch(Product Type) eq 'Notices to Mariners' and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";

            var result = _fileShareService.FssWeeklySearchAsync(searchText, accessToken);
            Assert.IsInstanceOf<BatchSearchResponse>(result);
        }
    }
}
