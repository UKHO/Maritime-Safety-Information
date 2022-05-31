using System;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning.Admin
{
    [TestFixture]
    public class RadioNavigationalWarningEditAdminTest : BaseRNWTest
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
            await SeedRadioNavigationalWarnings(GetFakeRadioNavigationalWarnings());
            await SeedWarningType(GetFakeWarningTypes());
        }

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _rnwRepository = new RNWRepository(_fakeContext);
            _rnwService = new RNWService(_rnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLoggerRnwService);
            _tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _rnwService);
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
            IActionResult result = await _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin.Id, _fakeEditRadioNavigationalWarningsAdmin);

            Assert.IsInstanceOf<IActionResult>(result);
            Assert.AreEqual("Record updated successfully!", _controller.TempData["message"].ToString());
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            Assert.IsTrue(_fakeContext.RadioNavigationalWarnings.ToListAsync().Result[0].LastModified < DateTime.Now);
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidValue_ThenRecordIsNotUpdated()
        {
            _controller.TempData = _tempData;
            _fakeEditRadioNavigationalWarningsAdmin = GetFakeEditRadioNavigationalWarningsAdmin();
            _fakeEditRadioNavigationalWarningsAdmin.Reference = string.Empty;

            Task<IActionResult> result = _controller.Edit(_fakeEditRadioNavigationalWarningsAdmin.Id, _fakeEditRadioNavigationalWarningsAdmin);

            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.IsNull(_controller.TempData["message"]);
        }

        [Test]
        public void WhenEditRadioNavigationWarningsWithInValidModel_ThenRecordIsNotUpdated()
        {
            _controller.TempData = _tempData;

            _controller.ModelState.AddModelError("WarningType", "In Valid WarningType Selected");
            Task<IActionResult> result = _controller.Edit(5, new EditRadioNavigationalWarningsAdmin());

            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.IsNull(_controller.TempData["message"]);
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
        public EditRadioNavigationalWarningsAdmin GetFakeEditRadioNavigationalWarningsAdmin()
        {
            return new EditRadioNavigationalWarningsAdmin()
            {
                Id = 5,
                WarningType = 1,
                Reference = "ReferenceTest",
                DateTimeGroup = new DateTime(2022, 12, 30),
                Summary = "SummaryTest",
                Content = "ContentTest",
                ExpiryDate = new DateTime(2099, 1, 1),
                IsDeleted = "true"
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
