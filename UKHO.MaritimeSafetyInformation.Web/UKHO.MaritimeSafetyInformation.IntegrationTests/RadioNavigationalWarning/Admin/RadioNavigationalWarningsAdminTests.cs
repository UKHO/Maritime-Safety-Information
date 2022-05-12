using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning.Admin
{
    [TestFixture()]
    public class RadioNavigationalWarningsAdminTests : RnwTestsHelper
    {
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private ILogger<RnwRepository> _fakeLoggerRnwRepository;
        private ILogger<RnwService> _fakeLoggerRnwService;
        private IRnwRepository _rnwRepository;
        private IRnwService _rnwService;

        private RadioNavigationalWarningsAdminController _controller;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _fakeLoggerRnwRepository = A.Fake<ILogger<RnwRepository>>();
            _fakeLoggerRnwService = A.Fake<ILogger<RnwService>>();

            SeedRadioNavigationalWarnings(GetRadioNavigationalWarnings());
            SeedWarningType(GetWarningTypes());
            //DeSeedRadioNavigationalWarnings();

            _rnwRepository = new RnwRepository(_fakeContext, _fakeLoggerRnwRepository);
            _rnwService = new RnwService(_rnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLoggerRnwService);

            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        //[Test]
        //public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        //{
        //    IActionResult result = await _controller.Index(1, null, null, true);
        //    RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;

        //    Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 8);
        //    Assert.IsTrue(adminListFilter.PageCount == 2);
        //    Assert.IsTrue(adminListFilter.SrNo == 0);
        //    Assert.IsTrue(adminListFilter.CurrentPageIndex == 1);
        //}
    }
}
