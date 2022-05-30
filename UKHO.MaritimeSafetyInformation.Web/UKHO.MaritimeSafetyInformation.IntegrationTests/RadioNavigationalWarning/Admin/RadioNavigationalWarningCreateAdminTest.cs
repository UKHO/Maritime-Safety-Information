using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using radioNavigationalWarningDto = UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using System;
using System.IO;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning.Admin
{
    [TestFixture]
    public class RadioNavigationalWarningCreateAdminTest : RNWTestHelper
    {
        public ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;
        private TempDataDictionary _tempData;
        private radioNavigationalWarningDto.RadioNavigationalWarning _fakeRadioNavigationalWarning;


        private RadioNavigationalWarningsAdminController _controller;

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
        public async Task WhenCallCreate_ThenReturnViewAsync()
        {
            IActionResult result = await _controller.Create();
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.IsNotNull(((ViewResult)result).ViewData["WarningType"]);
        }

        [Test]
        public async Task WhenCallAddRadioNavigationWarnings_ThenNewRecordIsCreated()
        {
            _controller.TempData = _tempData;

            IActionResult result = await _controller.Create(GetFakeRadioNavigationalWarning());

            Assert.IsInstanceOf<IActionResult>(result);
            Assert.AreEqual("Record created successfully!", _controller.TempData["message"].ToString());
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual(1, _fakeContext.RadioNavigationalWarnings.ToListAsync().Result.Count);
            Assert.IsTrue(_fakeContext.RadioNavigationalWarnings.ToListAsync().Result[0].LastModified < DateTime.Now);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidValue_ThenNewRecordIsNotCreated()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Reference = string.Empty;

            Task<IActionResult> result = _controller.Create(_fakeRadioNavigationalWarning);

            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.IsNull(_controller.TempData["message"]);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidModel_ThenNewRecordIsNotCreated()
        {
            _controller.TempData = _tempData;

            _controller.ModelState.AddModelError("WarningType", "In Valid WarningType Selected");
            Task<IActionResult> result = _controller.Create(new Common.Models.RadioNavigationalWarning.DTO.RadioNavigationalWarning());

            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.IsNull(_controller.TempData["message"]);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidWarningType_ThenReturnInValidDataException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>(),
                                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidReference_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Reference = string.Empty;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
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
    }
}
