using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning.Admin
{
    [TestFixture]
    public class RadioNavigationalWarningAdminTest : BaseRNWTest
    {
        public ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;

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

            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;

            Assert.AreEqual(8, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
            Assert.AreEqual(WarningTypes.NAVAREA_1, adminListFilter.RadioNavigationalWarningsAdminList[2].WarningType);
            Assert.AreEqual("NAVAREA 1", adminListFilter.RadioNavigationalWarningsAdminList[2].WarningTypeName);
            Assert.AreEqual("RnwAdminListReferance", adminListFilter.RadioNavigationalWarningsAdminList[2].Reference);
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
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
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
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, 2020);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(2, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, WarningTypes.UK_Coastal, 2024);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(1, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
            Assert.AreEqual(WarningTypes.UK_Coastal, adminListFilter.RadioNavigationalWarningsAdminList[0].WarningType);
            Assert.AreEqual("UK Coastal", adminListFilter.RadioNavigationalWarningsAdminList[0].WarningTypeName);
            Assert.AreEqual(2024, adminListFilter.RadioNavigationalWarningsAdminList[0].DateTimeGroup.Year);
        }

        [Test]
        public async Task WhenCallIndexWithValidPageNo_ThenReturnAsyncListBasedOnPageNo()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            IActionResult result = await _controller.Index(2, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(3, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(3, adminListFilter.PageCount);
            Assert.AreEqual(3, adminListFilter.SrNo);
            Assert.AreEqual(2, adminListFilter.CurrentPageIndex);
        }

        [Test]
        public async Task WhenCallIndexWithInValidPageNo_ThenReturnEmptyListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            IActionResult result = await _controller.Index(4, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(0, adminListFilter.RadioNavigationalWarningsAdminList.Count);
        }

        [Test]
        public async Task WhenCallIndexWithValidAdminListRecordPerPage_ThenReturnValidRecordPerPageAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
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
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            Assert.ThrowsAsync(Is.TypeOf<DivideByZeroException>(),
                         async delegate { await _controller.Index(1, null, null); });
        }

        [Test]
        public async Task WhenCallIndex_ThenIsDeletedShouldDisplayYesAndNoRespectively()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, 2020);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(2, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual("Yes", adminListFilter.RadioNavigationalWarningsAdminList[0].IsDeleted);
            Assert.AreEqual("No", adminListFilter.RadioNavigationalWarningsAdminList[1].IsDeleted);
        }

        [Test]
        public async Task WhenCallIndexWithContentLengthGreaterThan300Char_ThenWrapTheContent()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, 2024);
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
