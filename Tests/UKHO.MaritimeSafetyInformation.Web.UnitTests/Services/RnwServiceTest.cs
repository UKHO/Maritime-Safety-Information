using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RnwServiceTest
    {
        private RnwService _rnwService;
        private RadioNavigationalWarningsContext _fakeContext;
        private IRnwRepository _fakeRnwRepository;
        private ILogger<RnwService> _fakeLogger;
        private ILogger<RnwRepository> _fakeRnwRepositoryLogger;
        private IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;

        [SetUp]
        public void SetUp()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                                .UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);
            _fakeLogger = A.Fake<ILogger<RnwService>>();
            _fakeRnwRepositoryLogger = A.Fake<ILogger<RnwRepository>>();
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeRnwRepository = new RnwRepository(_fakeContext, _fakeRnwRepositoryLogger);
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;

            _rnwService = new RnwService(_fakeRnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLogger);

            _fakeContext.RadioNavigationalWarnings.RemoveRange(_fakeContext.RadioNavigationalWarnings);
            _fakeContext.WarningType.RemoveRange(_fakeContext.WarningType);

            _fakeContext.RadioNavigationalWarnings.AddRange(GetFakeRadioNavigationalWarningList());
            _fakeContext.WarningType.AddRange(GetFakeWarningTypeList());
            _fakeContext.SaveChanges();
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count > 0);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 0);
            Assert.IsTrue(result.CurrentPageIndex == 1);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 1, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 2);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithYearFilter_ThenReturnFilteredListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, 2022, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 2, 2020, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithValidPageNo_ThenReturnFilteredListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(2, null, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 3);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithInValidPageNo_ThenReturnEmptyListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(4, null, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 0);
        }

        [Test]
        public void WhenCallGetRadioNavigationWarningsWithInValidAdminListRecordPerPage_ThenThrowDivideByZeroException()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            Assert.ThrowsAsync(Is.TypeOf<DivideByZeroException>(),
                           async delegate { await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, null, string.Empty); });
        }

        #region PrivateMethod
        private static List<RadioNavigationalWarnings> GetFakeRadioNavigationalWarningList()
        {
            List<RadioNavigationalWarnings> radioNavigationalWarningList = new();
            radioNavigationalWarningList.Add(new RadioNavigationalWarnings()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarnings()
            {
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarnings()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarnings()
            {
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });
            return radioNavigationalWarningList;
        }

        private static List<WarningType> GetFakeWarningTypeList()
        {
            List<WarningType> warningTypeList = new();
            warningTypeList.Add(new WarningType()
            {
                Id = 1,
                Name = "NAVAREA 1"
            });

            warningTypeList.Add(new WarningType()
            {
                Id = 2,
                Name = "UK Coastal"
            });
            return warningTypeList;
        }
        #endregion

    }
}
