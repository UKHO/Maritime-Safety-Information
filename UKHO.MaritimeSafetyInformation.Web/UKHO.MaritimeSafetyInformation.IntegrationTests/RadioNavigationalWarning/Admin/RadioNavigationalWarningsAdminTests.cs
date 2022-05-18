using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning.Admin
{
    [TestFixture()]
    public class RadioNavigationalWarningsAdminTests : RnwTestsHelper
    {
        private IRnwRepository _rnwRepository;
        private IRnwService _rnwService;

        private RadioNavigationalWarningsAdminController _controller;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await SeedRadioNavigationalWarnings(GetRadioNavigationalWarnings());
            await SeedWarningType(GetWarningTypes());
        }

        [SetUp]
        public void Setup()
        {
            _rnwRepository = new RnwRepository(_fakeContext, _fakeLoggerRnwRepository);
            _rnwService = new RnwService(_rnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLoggerRnwService);

            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _rnwService);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, null);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;

            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 8);
            Assert.IsTrue(adminListFilter.PageCount == 1);
            Assert.IsTrue(adminListFilter.SrNo == 0);
            Assert.IsTrue(adminListFilter.CurrentPageIndex == 1);
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, 1, null);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 4);
            Assert.IsTrue(adminListFilter.PageCount == 1);
            Assert.IsTrue(adminListFilter.SrNo == 0);
            Assert.IsTrue(adminListFilter.CurrentPageIndex == 1);
        }

        [Test]
        public async Task WhenCallIndexWithYearFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, 2020);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 2);
            Assert.IsTrue(adminListFilter.PageCount == 1);
            Assert.IsTrue(adminListFilter.SrNo == 0);
            Assert.IsTrue(adminListFilter.CurrentPageIndex == 1);
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, 2, 2024);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(adminListFilter.PageCount == 1);
            Assert.IsTrue(adminListFilter.SrNo == 0);
            Assert.IsTrue(adminListFilter.CurrentPageIndex == 1);
        }

        [Test]
        public async Task WhenCallIndexWithValidPageNo_ThenReturnAsyncListBasedOnPageNo()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            IActionResult result = await _controller.Index(2, null, null);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 3);
            Assert.IsTrue(adminListFilter.PageCount == 3);
            Assert.IsTrue(adminListFilter.SrNo == 3);
            Assert.IsTrue(adminListFilter.CurrentPageIndex == 2);
        }

        [Test]
        public async Task WhenCallIndexWithInValidPageNo_ThenReturnEmptyListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            IActionResult result = await _controller.Index(4, null, null);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 0);
        }

        [Test]
        public async Task WhenCallIndexWithWithValidAdminListRecordPerPage_ThenReturnValidRecordPerPageAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
            IActionResult result = await _controller.Index(1, null, null);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 5);
            Assert.IsTrue(adminListFilter.PageCount == 2);
            Assert.IsTrue(adminListFilter.SrNo == 0);
            Assert.IsTrue(adminListFilter.CurrentPageIndex == 1);
        }

        [Test]
        public void WhenCallIndexWithWithInValidAdminListRecordPerPage_ThenThrowDivideByZeroExceptiont()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            Assert.ThrowsAsync(Is.TypeOf<DivideByZeroException>(),
                         async delegate { await  _controller.Index(1, null, null); });
        }

        [Test]
        public async Task WhenCallIndex_ThenIsDeletedShouldDisplayYesAndNoRespectively()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, 2020);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 2);
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList[0].IsDeleted == "Yes");
            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList[1].IsDeleted == "No");
        }

        [Test]
        public async Task WhenCallIndexWithContentLenthGreaterThan300Char_ThenWrapTheContent()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, null, 2024);
            RadioNavigationalWarningsAdminListFilter adminListFilter = (RadioNavigationalWarningsAdminListFilter)((ViewResult)result).Model;
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
