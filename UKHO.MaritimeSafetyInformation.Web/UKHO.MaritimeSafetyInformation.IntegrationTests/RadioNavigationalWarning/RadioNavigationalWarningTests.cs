﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning
{
    [TestFixture]
    public class RadioNavigationalWarningTests : RNWTestHelper
    {
        public ILogger<RadioNavigationalWarningsController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;

        private RadioNavigationalWarningsController _controller;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await SeedRadioNavigationalWarnings(GetRadioNavigationalWarnings());
            await SeedWarningType(GetWarningTypes());
        }

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsController>>();
            _rnwRepository = new RNWRepository(_fakeContext);
            _rnwService = new RNWService(_rnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLoggerRnwService);

            _controller = new RadioNavigationalWarningsController(_fakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            IActionResult result = await _controller.Index();
            List<RadioNavigationalWarningsData> warningsData = (List<RadioNavigationalWarningsData>)((ViewResult)result).Model;
            Assert.AreEqual(6, warningsData.Count);
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}