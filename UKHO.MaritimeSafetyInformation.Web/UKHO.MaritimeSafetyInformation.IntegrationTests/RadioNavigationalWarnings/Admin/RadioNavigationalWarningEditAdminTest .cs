﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningEditAdminTest : BaseRNWTest
    {
        public ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;
        private TempDataDictionary _tempData;
        private EditRadioNavigationalWarningsAdmin _fakeEditRadioNavigationalWarningsAdmin;
        private RadioNavigationalWarningsAdminController _controller;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await SeedWarningType(GetFakeWarningTypes());
            await SeedRadioNavigationalWarnings(new List<RadioNavigationalWarning>() { GetFakeCreateRadioNavigationalWarning() });
        }

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
        public async Task WhenCallEdit_ThenReturnViewAsync()
        {
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            IActionResult result = await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin.Id);
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.IsNotNull(((ViewResult)result).ViewData["WarningType"]);
        }

        [Test]
        public async Task WhenCallUpdateRadioNavigationWarnings_ThenRecordIsUpdated()
        {
            _controller.TempData = _tempData;
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            IActionResult result = await _controller.Edit(9, _fakeEditRadioNavigationalWarningsAdmin);
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.AreEqual("Record updated successfully!", _controller.TempData["message"].ToString());
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            
        }


        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidWarningType_ThenReturnInValidDataException()
        {
            _controller.TempData = _tempData;
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>(),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin.Id, _fakeEditRadioNavigationalWarningsAdmin); });
        }


        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidReference_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.Reference = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin.Id, _fakeEditRadioNavigationalWarningsAdmin); });
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidSummary_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.Summary = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin.Id, _fakeEditRadioNavigationalWarningsAdmin); });
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidContent_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.Content = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin.Id, _fakeEditRadioNavigationalWarningsAdmin); });
        }

        public static EditRadioNavigationalWarningsAdmin GetFakeEditRadioNavigationalWarningsAdmin()
        {
            return new EditRadioNavigationalWarningsAdmin()
            {
                Id = 9,
                WarningType = 1,
                Reference = "EditReferenceTest",
                DateTimeGroup = new DateTime(2022, 12, 30),
                Summary = "EditSummaryTest",
                Content = "EditContentTest",
                ExpiryDate = new DateTime(2099, 1, 1),
                IsDeleted = "true"
            };
        }

        public static RadioNavigationalWarning GetFakeCreateRadioNavigationalWarning()
        {
            return new RadioNavigationalWarning()
            {
                Id = 9,
                WarningType = 1,
                Reference = "CreateReferenceTest",
                DateTimeGroup = new DateTime(2022, 12, 30),
                Summary = "CreateSummaryTest",
                Content = "CreateContentTest",
                ExpiryDate = new DateTime(2099, 1, 1),
                
            };
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}