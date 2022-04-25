using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.Configuration;
using Microsoft.Extensions.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using Microsoft.Extensions.Options;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class NMDataServiceTest
    {
        private IHttpClientFactory _httpClientFactory;
        private IFileShareService _fileShareService;
        private IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private ILogger<NMDataService> Logger;
        private NMDataService _NMDataService;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _fileShareService = A.Fake<IFileShareService>();
            _fileShareServiceConfig = A.Fake<IOptions<FileShareServiceConfiguration>>();
            Logger = A.Fake<ILogger<NMDataService>>();
            _NMDataService = new NMDataService(_fileShareService,_httpClientFactory,_fileShareServiceConfig, Logger);
        }


    }
}
