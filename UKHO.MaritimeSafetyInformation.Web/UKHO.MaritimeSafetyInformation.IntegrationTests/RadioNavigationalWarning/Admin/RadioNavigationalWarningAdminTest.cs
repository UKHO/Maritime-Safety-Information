﻿using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning.Admin
{
    [TestFixture]
    public class RadioNavigationalWarningAdminTest : RNWTestHelper
    {
        public ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private RNWService _rnwService;

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

            Assert.IsTrue(adminListFilter.RadioNavigationalWarningsAdminList.Count == 8);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await _controller.Index(1, 1, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(4, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
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
            IActionResult result = await _controller.Index(1, 2, 2024);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.AreEqual(1, adminListFilter.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, adminListFilter.PageCount);
            Assert.AreEqual(0, adminListFilter.SrNo);
            Assert.AreEqual(1, adminListFilter.CurrentPageIndex);
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