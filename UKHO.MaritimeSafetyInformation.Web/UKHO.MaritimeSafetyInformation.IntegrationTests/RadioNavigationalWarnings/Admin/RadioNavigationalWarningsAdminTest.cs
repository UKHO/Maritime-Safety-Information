extern alias MSIAdminProjectAlias;
using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningsAdminTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> fakeLogger;
        private ILogger<RNWService> fakeLoggerRnwService;
        private IRNWRepository rnwRepository;
        private IRNWService rnwService;
        private const int Year2024 = 2024;
        private const int Year2020 = 2020;

        private RadioNavigationalWarningsAdminController controller;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await SeedRadioNavigationalWarnings(GetFakeRadioNavigationalWarnings());
            await SeedWarningType(GetFakeWarningTypes());
        }

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            fakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            rnwRepository = new RNWRepository(FakeContext);
            rnwService = new RNWService(rnwRepository, FakeRadioNavigationalWarningConfiguration, fakeLoggerRnwService);

            controller = new RadioNavigationalWarningsAdminController(FakeHttpContextAccessor, fakeLogger, rnwService);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await controller.Index(1, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;

            Assert.That(8, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.Count));
            Assert.That(1, Is.EqualTo(adminListFilter.PageCount));
            Assert.That(0, Is.EqualTo(adminListFilter.SrNo));
            Assert.That(1, Is.EqualTo(adminListFilter.CurrentPageIndex));
            Assert.That(WarningTypes.NAVAREA_1, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 4).WarningType));
            Assert.That("NAVAREA 1", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 4).WarningTypeName));
            Assert.That("RnwAdminListReference", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 4).Reference));
            Assert.That(new DateTime(2022, 1, 1), Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 4).DateTimeGroup));
            Assert.That("RnwAdminListSummary", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 4).Summary));
            Assert.That("RnwAdminListContent", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 4).Content));
            Assert.That(new DateTime(2099, 1, 1), Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 4).ExpiryDate));
            Assert.That(((ViewResult)result).ViewData["WarningTypes"], Is.Not.Null);
            Assert.That(((ViewResult)result).ViewData["Years"], Is.Not.Null);
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await controller.Index(1, WarningTypes.NAVAREA_1, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.That(4, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.Count));
            Assert.That(1, Is.EqualTo(adminListFilter.PageCount));
            Assert.That(0, Is.EqualTo(adminListFilter.SrNo));
            Assert.That(1, Is.EqualTo(adminListFilter.CurrentPageIndex));
            Assert.That(WarningTypes.NAVAREA_1, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 7).WarningType));
            Assert.That("NAVAREA 1", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 7).WarningTypeName));
        }

        [Test]
        public async Task WhenCallIndexWithYearFilter_ThenReturnFilteredListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await controller.Index(1, null, Year2020);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.That(2, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.Count));
            Assert.That(1, Is.EqualTo(adminListFilter.PageCount));
            Assert.That(0, Is.EqualTo(adminListFilter.SrNo));
            Assert.That(1, Is.EqualTo(adminListFilter.CurrentPageIndex));
        }

        [Test]
        public async Task WhenCallIndexWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await controller.Index(1, WarningTypes.UK_Coastal, Year2024);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.That(1, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.Count));
            Assert.That(1, Is.EqualTo(adminListFilter.PageCount));
            Assert.That(0, Is.EqualTo(adminListFilter.SrNo));
            Assert.That(1, Is.EqualTo(adminListFilter.CurrentPageIndex));
            Assert.That(WarningTypes.UK_Coastal, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList[0].WarningType));
            Assert.That("UK Coastal", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 8).WarningTypeName));
            Assert.That(Year2024, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 8).DateTimeGroup.Year));
        }

        [Test]
        public async Task WhenCallIndexWithValidPageNo_ThenReturnAsyncListBasedOnPageNo()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            IActionResult result = await controller.Index(2, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.That(3, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.Count));
            Assert.That(3, Is.EqualTo(adminListFilter.PageCount));
            Assert.That(3, Is.EqualTo(adminListFilter.SrNo));
            Assert.That(2, Is.EqualTo(adminListFilter.CurrentPageIndex));
        }

        [Test]
        public async Task WhenCallIndexWithValidAdminListRecordPerPage_ThenReturnValidRecordPerPageAsync()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
            IActionResult result = await controller.Index(1, null, null);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.That(5, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.Count));
            Assert.That(2, Is.EqualTo(adminListFilter.PageCount));
            Assert.That(0, Is.EqualTo(adminListFilter.SrNo));
            Assert.That(1, Is.EqualTo(adminListFilter.CurrentPageIndex));
        }

        [Test]
        public void WhenCallIndexWithInValidAdminListRecordPerPage_ThenThrowDivideByZeroException()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            Assert.ThrowsAsync(Is.TypeOf<DivideByZeroException>(),
                         async delegate { await controller.Index(1, null, null); });
        }

        [Test]
        public async Task WhenIndexIsCalled_ThenShouldDisplayStatusAsActiveOrExpired()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await controller.Index(1, null, Year2020);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.That(2, Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.Count));
            Assert.That("Active", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 1).Status));
            Assert.That("Expired", Is.EqualTo(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 2).Status));
        }

        [Test]
        public async Task WhenCallIndexWithContentLengthGreaterThan300Char_ThenWrapTheContent()
        {
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
            IActionResult result = await controller.Index(1, null, Year2024);
            RadioNavigationalWarningsAdminFilter adminListFilter = (RadioNavigationalWarningsAdminFilter)((ViewResult)result).Model;
            Assert.That(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 8).Content.Length <= 303);
            Assert.That(adminListFilter.RadioNavigationalWarningsAdminList.First(x => x.Id == 8).Content.Contains("..."));
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}
