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

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class NMDataServiceTest
    {
        private IHttpClientFactory _fakehttpClientFactory;
        private IFileShareService _fakefileShareService;
        private IConfiguration _fakeconfiguration;
        private FileShareServiceConfiguration _fakefileShareServiceConfig;
        private ILogger<NMDataService> fakeLogger;
        private NMDataService _fakeNMDataService;

        [SetUp]
        public void Setup()
        {
            _fakehttpClientFactory = A.Fake<IHttpClientFactory>();
            _fakefileShareService = A.Fake<IFileShareService>();
            _fakeconfiguration = A.Fake<IConfiguration>();
            _fakefileShareServiceConfig = A.Fake<FileShareServiceConfiguration>();
            fakeLogger = A.Fake<ILogger<NMDataService>>();
            _fakeNMDataService = new NMDataService(_fakefileShareService,_fakehttpClientFactory,_fakeconfiguration, fakeLogger);
        }


    }
}
