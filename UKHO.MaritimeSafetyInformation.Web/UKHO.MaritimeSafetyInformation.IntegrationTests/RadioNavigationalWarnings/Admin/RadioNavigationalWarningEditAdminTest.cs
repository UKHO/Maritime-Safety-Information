using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    public class RadioNavigationalWarningEditAdminTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;
        private TempDataDictionary _tempData;
        private EditRadioNavigationalWarningsAdmin _fakeEditRadioNavigationalWarningsAdmin;
        private RadioNavigationalWarningsAdminController _controller;
        protected readonly RadioNavigationalWarningsContext FakeContext;
        protected readonly IOptions<RadioNavigationalWarningConfiguration> FakeRadioNavigationalWarningConfiguration;
        protected readonly IHttpContextAccessor FakeHttpContextAccessor;
        protected readonly ILogger<RNWService> FakeLoggerRnwService;

        public RadioNavigationalWarningEditAdminTest()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                    .UseInMemoryDatabase("msi-in-db");
            FakeContext = new RadioNavigationalWarningsContext(builder.Options);
            FakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            FakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            FakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
        }

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
            _rnwRepository = new RNWRepository(FakeContext);
            _rnwService = new RNWService(_rnwRepository, FakeRadioNavigationalWarningConfiguration, FakeLoggerRnwService);
            _tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
            _controller = new RadioNavigationalWarningsAdminController(FakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        protected async Task SeedRadioNavigationalWarnings(RadioNavigationalWarning radioNavigationalWarning)
        {
            FakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning);
            await FakeContext.SaveChangesAsync();
        }

        protected async Task SeedWarningType(List<WarningType> warningTypes)
        {
            FakeContext.WarningType.AddRange(warningTypes);
            await FakeContext.SaveChangesAsync();
        }

        protected async Task DeSeedRadioNavigationalWarnings()
        {
            DbSet<RadioNavigationalWarning> warnings = FakeContext.RadioNavigationalWarnings;
            FakeContext.RadioNavigationalWarnings.RemoveRange(warnings);
            await FakeContext.SaveChangesAsync();
        }

        protected async Task DeSeedWarningType()
        {
            DbSet<WarningType> warningType = FakeContext.WarningType;
            FakeContext.WarningType.RemoveRange(warningType);
            await FakeContext.SaveChangesAsync();
        }

        protected static RadioNavigationalWarning GetFakeRadioNavigationalWarnings()
        {
            RadioNavigationalWarning radioNavigationalWarningList = new();
            radioNavigationalWarningList.Id = 1;
            radioNavigationalWarningList.WarningType = WarningTypes.NAVAREA_1;
            radioNavigationalWarningList.Reference = "RnwAdminListReference";
            radioNavigationalWarningList.DateTimeGroup = new DateTime(2020, 1, 1);
            radioNavigationalWarningList.Summary = "RnwAdminListSummary";
            radioNavigationalWarningList.Content = "RnwAdminListContent";
            radioNavigationalWarningList.IsDeleted = true;
            radioNavigationalWarningList.ExpiryDate = new DateTime(2099, 1, 1);
            return radioNavigationalWarningList;
        }

        protected static List<WarningType> GetFakeWarningTypes()
        {
            List<WarningType> warningTypes = new();
            warningTypes.Add(new WarningType()
            {
                Id = 1,
                Name = "NAVAREA 1"
            });

            warningTypes.Add(new WarningType()
            {
                Id = 2,
                Name = "UK Coastal"
            });
            return warningTypes;
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
                Id = 1,
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
