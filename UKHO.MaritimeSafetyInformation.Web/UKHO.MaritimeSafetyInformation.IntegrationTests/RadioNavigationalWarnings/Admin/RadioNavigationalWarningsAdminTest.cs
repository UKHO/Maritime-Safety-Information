extern alias MSIAdminProjectAlias;

using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningsAdminTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private ILogger<RNWService> _fakeLoggerRnwService;
        private IRNWRepository _rnwRepository;
        private IRNWService _rnwService;
        private const int Year2024 = 2024;
        private const int Year2020 = 2020;

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
            _fakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            _rnwRepository = new RNWRepository(FakeContext);
            _rnwService = new RNWService(_rnwRepository, FakeRadioNavigationalWarningConfiguration, _fakeLoggerRnwService);

            _controller = new RadioNavigationalWarningsAdminController(FakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;

            Assert.AreEqual(8, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
            Assert.AreEqual(WarningTypes.NAVAREA_1, adminListFilter.RadioNavigationalWarningsAdminList[2].WarningType);
            Assert.AreEqual("NAVAREA 1", adminListFilter.RadioNavigationalWarningsAdminList[2].WarningTypeName);
            Assert.AreEqual("RnwAdminListReference", adminListFilter.RadioNavigationalWarningsAdminList[2].Reference);
            Assert.AreEqual(new DateTime(2022, 1, 1), adminListFilter.RadioNavigationalWarningsAdminList[2].DateTimeGroup);
            Assert.AreEqual("RnwAdminListSummary", adminListFilter.RadioNavigationalWarningsAdminList[2].Summary);
            Assert.AreEqual("RnwAdminListContent", adminListFilter.RadioNavigationalWarningsAdminList[2].Content);
            Assert.AreEqual(new DateTime(2099, 1, 1), adminListFilter.RadioNavigationalWarningsAdminList[2].ExpiryDate);
            Assert.AreEqual("No", adminListFilter.RadioNavigationalWarningsAdminList[2].IsDeleted);
            Assert.IsNotNull(((ViewResult)result).ViewData["WarningTypes"]);
            Assert.IsNotNull(((ViewResult)result).ViewData["Years"]);
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, WarningTypes.NAVAREA_1, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(4, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
            Assert.AreEqual(WarningTypes.NAVAREA_1, adminListFilter.RadioNavigationalWarningsAdminList[0].WarningType);
            Assert.AreEqual("NAVAREA 1", adminListFilter.RadioNavigationalWarningsAdminList[0].WarningTypeName);
        }

        [Test]
        public async Task WhenCallIndexWithYearFilter_ThenReturnFilteredListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, Year2020);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(2, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, WarningTypes.UK_Coastal, Year2024);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(1, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
            Assert.AreEqual(WarningTypes.UK_Coastal, adminListFilter.RadioNavigationalWarningsAdminList[0].WarningType);
            Assert.AreEqual("UK Coastal", adminListFilter.RadioNavigationalWarningsAdminList[0].WarningTypeName);
            Assert.AreEqual(Year2024, adminListFilter.RadioNavigationalWarningsAdminList[0].DateTimeGroup.Year);
        }

        [Test]
        public async Task WhenCallIndexWithValidPageNo_ThenReturnAsyncListBasedOnPageNo()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            IActionResult result = await _controller.Index(2, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(3, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(3, adminListFilter.PageCount);
            Assert.AreEqual(3, adminListFilter.SrNo);
            Assert.AreEqual(2, adminListFilter.CurrentPageIndex);
        }

        [Test]
        public async Task WhenCallIndexWithValidAdminListRecordPerPage_ThenReturnValidRecordPerPageAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
            IActionResult result = await _controller.Index(1, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(5, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(2, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
        }

        [Test]
        public void WhenCallIndexWithInValidAdminListRecordPerPage_ThenThrowDivideByZeroException()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            Assert.ThrowsAsync(Is.TypeOf<DivideByZeroException>(),
                         async delegate { await _controller.Index(1, null, null); });
        }

        [Test]
        public async Task WhenCallIndex_ThenIsDeletedShouldDisplayYesAndNoRespectively()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, Year2020);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(2, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual("Yes", adminListFilter.RadioNavigationalWarningsAdminList[0].IsDeleted);
            Assert.AreEqual("No", adminListFilter.RadioNavigationalWarningsAdminList[1].IsDeleted);
        }

        [Test]
        public async Task WhenCallIndexWithContentLengthGreaterThan300Char_ThenWrapTheContent()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, Year2024);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList[0].Content.Length <= 303);
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList[0].Content.Contains("..."));
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}
