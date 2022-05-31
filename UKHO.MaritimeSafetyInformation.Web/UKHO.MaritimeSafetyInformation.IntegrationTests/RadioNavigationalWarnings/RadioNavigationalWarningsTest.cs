using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings
{
    [TestFixture]
    internal class RadioNavigationalWarningsTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;

        private RadioNavigationalWarningsController _controller;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await SeedRadioNavigationalWarnings(GetFakeRadioNavigationalWarnings());
            await SeedWarningType(GetFakeWarningTypes());
        }

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsController>>();
            _rnwRepository = new RNWRepository(FakeContext);
            _rnwService = new RNWService(_rnwRepository, FakeRadioNavigationalWarningConfiguration, FakeLoggerRnwService);

            _controller = new RadioNavigationalWarningsController(FakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            IActionResult result = await _controller.Index();
            List<RadioNavigationalWarningsData> warningsData = (List<RadioNavigationalWarningsData>)((ViewResult)result).Model;
            Assert.AreEqual(6, warningsData.Count);
            Assert.AreEqual("RnwAdminListReference", warningsData[2].Reference);
            Assert.AreEqual("RnwAdminListSummary", warningsData[2].Description);
            Assert.AreEqual(new DateTime(2022, 1, 1), warningsData[2].DateTimeGroup);
            Assert.AreEqual("011200 UTC Jan 22", warningsData[2].DateTimeGroupRnwFormat);
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}
