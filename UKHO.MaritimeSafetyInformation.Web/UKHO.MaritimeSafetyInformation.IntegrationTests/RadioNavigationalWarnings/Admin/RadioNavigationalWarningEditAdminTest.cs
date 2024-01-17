extern alias MSIAdminProjectAlias;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningEditAdminTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> fakeLogger;
        private IRNWRepository rnwRepository;
        private RNWService rnwService;
        private TempDataDictionary tempData;
        private RadioNavigationalWarning fakeRadioNavigationalWarning;
        private RadioNavigationalWarningsAdminController controller;
        private ILogger<RNWService> fakeLoggerRnwService;
        private readonly ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Admin User"), }, "mock"));


        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await SeedWarningType(GetFakeWarningTypes());
            await SeedRadioNavigationalWarnings(new List<RadioNavigationalWarning>() { GetFakeRadioNavigationalWarning() });
        }

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            fakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            rnwRepository = new RNWRepository(FakeContext);
            rnwService = new RNWService(rnwRepository, FakeRadioNavigationalWarningConfiguration, fakeLoggerRnwService);
            tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
            controller = new RadioNavigationalWarningsAdminController(FakeHttpContextAccessor, fakeLogger, rnwService);
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        }

        [Test]
        public async Task WhenCallEdit_ThenReturnViewAsync()
        {
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            IActionResult result = await controller.Edit(fakeRadioNavigationalWarning.Id);
            Assert.That(result, Is.InstanceOf<IActionResult>());
            Assert.That(((ViewResult)result).ViewData["WarningType"], Is.Not.Null);
        }

        [Test]
        public async Task WhenCallUpdateRadioNavigationWarnings_ThenRecordIsUpdated()
        {
            controller.TempData = tempData;
            fakeRadioNavigationalWarning = FakeContext.RadioNavigationalWarnings.Find(9);
            fakeRadioNavigationalWarning.IsDeleted = false;
            IActionResult result = await controller.Edit(fakeRadioNavigationalWarning);
            Assert.That(result, Is.InstanceOf<IActionResult>());
            Assert.That("Record updated successfully!", Is.EqualTo(controller.TempData["message"].ToString()));
            Assert.That("Index", Is.EqualTo(((RedirectToActionResult)result).ActionName));
        }


        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidWarningType_ThenReturnInValidDataException()
        {
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                                async delegate { await controller.Edit(fakeRadioNavigationalWarning); });
        }


        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidReference_ThenReturnArgumentNullException()
        {
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.Reference = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                                async delegate { await controller.Edit(fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidSummary_ThenReturnArgumentNullException()
        {
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.Summary = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                                async delegate { await controller.Edit(fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidContent_ThenReturnArgumentNullException()
        {
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.Content = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter content"),
                                async delegate { await controller.Edit(fakeRadioNavigationalWarning); });
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}
