extern alias MSIAdminProjectAlias;

using FakeItEasy;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningEditAdminTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;
        private TempDataDictionary _tempData;
        private EditRadioNavigationalWarningAdmin _fakeEditRadioNavigationalWarningsAdmin;
        private RadioNavigationalWarningsAdminController _controller;
        private ILogger<RNWService> _fakeLoggerRnwService;

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
            _fakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            _rnwRepository = new RNWRepository(FakeContext);
            _rnwService = new RNWService(_rnwRepository, FakeRadioNavigationalWarningConfiguration, _fakeLoggerRnwService);
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
            IActionResult result = await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin);
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.AreEqual("Record updated successfully!", _controller.TempData["message"].ToString());
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            
        }


        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidWarningType_ThenReturnInValidDataException()
        {
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin); });
        }


        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidReference_ThenReturnArgumentNullException()
        {
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.Reference = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin); });
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidSummary_ThenReturnArgumentNullException()
        {
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.Summary = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin); });
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidContent_ThenReturnArgumentNullException()
        {
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.Content = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter content"),
                                async delegate { await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin); });
        }

        private static EditRadioNavigationalWarningAdmin GetFakeEditRadioNavigationalWarningsAdmin()
        {
            return new EditRadioNavigationalWarningAdmin()
            {
                Id = 9,
                WarningType = 1,
                Reference = "EditReferenceTest",
                DateTimeGroup = new DateTime(2022, 12, 30),
                Summary = "EditSummaryTest",
                Content = "EditContentTest",
                ExpiryDate = new DateTime(2099, 1, 1),
                IsDeleted = true
            };
        }

        private static RadioNavigationalWarning GetFakeCreateRadioNavigationalWarning()
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
