﻿using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningCreateAdminTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;
        private TempDataDictionary _tempData;
        private RadioNavigationalWarning _fakeRadioNavigationalWarning;

        private RadioNavigationalWarningsAdminController _controller;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _rnwRepository = new RNWRepository(FakeContext);
            _rnwService = new RNWService(_rnwRepository, FakeRadioNavigationalWarningConfiguration, FakeLoggerRnwService);
            _tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());

            _controller = new RadioNavigationalWarningsAdminController(FakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        [Test]
        public async Task WhenCallCreate_ThenReturnViewAsync()
        {
            IActionResult result = await _controller.Create();
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.IsNotNull(((ViewResult)result).ViewData["WarningType"]);
        }

        [Test]
        public async Task WhenCallAddRadioNavigationalWarnings_ThenNewRecordIsCreated()
        {
            _controller.TempData = _tempData;

            IActionResult result = await _controller.Create(GetFakeRadioNavigationalWarning());

            Assert.IsInstanceOf<IActionResult>(result);
            Assert.AreEqual("Record created successfully!", _controller.TempData["message"].ToString());
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual(1, FakeContext.RadioNavigationalWarnings.ToListAsync().Result.Count);
            Assert.IsTrue(FakeContext.RadioNavigationalWarnings.ToListAsync().Result[0].LastModified < DateTime.Now);
        }

        [Test]
        public void WhenAddRadioNavigationalWarningsWithInValidValue_ThenNewRecordIsNotCreated()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Reference = string.Empty;
            Task<IActionResult> result = _controller.Create(_fakeRadioNavigationalWarning);
            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public async Task WhenAddRadioNavigationWarningsWithInValidModel_ThenNewRecordIsNotCreated()
        {
            _controller.TempData = _tempData;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Create.cshtml";
            _controller.ModelState.AddModelError("WarningType", "In Valid WarningType Selected");
            IActionResult result = await _controller.Create(new RadioNavigationalWarning());
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidWarningType_ThenReturnInValidDataException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidReference_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Reference = string.Empty;
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidSummary_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Summary = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidContent_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Content = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}