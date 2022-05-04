using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RnwRepositoryTest
    {
        private RnwRepository _rnwRepository;
        private RadioNavigationalWarningsContext _fakeContext;
        private IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;
        private ILogger<RnwRepository> _fakeLogger;

        [SetUp]
        public void SetUp()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                                .UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeLogger = A.Fake<ILogger<RnwRepository>>();
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;

            _rnwRepository = new RnwRepository(_fakeContext, _fakeRadioNavigationalWarningConfiguration, _fakeLogger);

            _fakeContext.RadioNavigationalWarnings.RemoveRange(_fakeContext.RadioNavigationalWarnings);
            _fakeContext.WarningType.RemoveRange(_fakeContext.WarningType);

            RadioNavigationalWarnings radioNavigationalWarning1 = new()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            };
            RadioNavigationalWarnings radioNavigationalWarning2 = new()
            {
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            };
            RadioNavigationalWarnings radioNavigationalWarning3 = new()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            };
            RadioNavigationalWarnings radioNavigationalWarning4 = new()
            {
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            };

            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning1);
            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning2);
            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning3);
            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning4);

            WarningType WarningType1 = new()
            {
                Id = 1,
                Name = "UK Coastal"
            };
            WarningType WarningType2 = new()
            {
                Id = 2,
                Name = "NAVAREA 1"
            };

            _fakeContext.WarningType.Add(WarningType1);
            _fakeContext.WarningType.Add(WarningType2);

            _fakeContext.SaveChanges();
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count > 0);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 1, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 2);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithYearFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 0, "2022", true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeAndYearFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 1, "2020", true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithValidPageNo_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(2, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 3);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithInValidPageNo_ThenReturnEmptyList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(4, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithInValidAdminListRecordPerPage_ThenThrowExceptionWithNullObject()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList == null);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithNotReloadData_ReturnExisitngList()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
            await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 0, string.Empty, false, string.Empty);
            AddRadioNavigationWarningRecord();

            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 0, string.Empty, false, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 4);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithReloadDataReturnUpdatedList()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
            await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 0, string.Empty, false, string.Empty);
            AddRadioNavigationWarningRecord();

            RadioNavigationalWarningsAdminListFilter result = await _rnwRepository.GetRadioNavigationWarningsForAdminAsync(1, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 5);
        }

        private void AddRadioNavigationWarningRecord()
        {
            RadioNavigationalWarnings radioNavigationalWarning5 = new()
            {
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            };
            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning5);
            _fakeContext.SaveChanges();
        }
    }
}
