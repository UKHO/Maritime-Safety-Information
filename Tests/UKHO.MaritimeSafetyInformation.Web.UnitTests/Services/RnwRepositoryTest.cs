using System;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
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

        [SetUp]
        public void SetUp()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                                .UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            _rnwRepository = new RnwRepository(_fakeContext, _fakeRadioNavigationalWarningConfiguration);

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
        public void WhenCallGetRadioNavigationWarningsForAdminMethod_ThenReturnList()
        {
            RadioNavigationalWarningsAdminListFilter result = _rnwRepository.GetRadioNavigationWarningsForAdmin(pageIndex:1,warningTypeId:0,year:string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count>0);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public void WhenCallGetRadioNavigationWarningsWithWarningTypeFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = _rnwRepository.GetRadioNavigationWarningsForAdmin(pageIndex: 1, warningTypeId: 1, year: string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 2);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public void WhenCallGetRadioNavigationWarningsWithYearFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = _rnwRepository.GetRadioNavigationWarningsForAdmin(pageIndex: 1, warningTypeId: 0, year: "2022");
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public void WhenCallGetRadioNavigationWarningsWithWarningTypeAndYearFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = _rnwRepository.GetRadioNavigationWarningsForAdmin(pageIndex: 1, warningTypeId: 1, year: "2020");
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public void WhenCallGetRadioNavigationWarningsWithValidPageNo_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = _rnwRepository.GetRadioNavigationWarningsForAdmin(pageIndex: 2, warningTypeId: 0, year: string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 3);
        }
    }
}
