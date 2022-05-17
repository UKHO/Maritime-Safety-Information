using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RnwServiceTest
    {
        private RnwService _rnwService;
        private IRnwRepository _fakeRnwRepository;
        private ILogger<RnwService> _fakeLogger;
        private IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;

        [SetUp]
        public void SetUp()
        {
            _fakeLogger = A.Fake<ILogger<RnwService>>();
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeRnwRepository = A.Fake<IRnwRepository>();
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;

            _rnwService = new RnwService(_fakeRnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLogger);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count > 0);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 0);
            Assert.IsTrue(result.CurrentPageIndex == 1);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 1, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 2);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithYearFilter_ThenReturnFilteredListAsync()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, 2022, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 2, 2020, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithValidPageNo_ThenReturnFilteredListAsync()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(2, null, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 3);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithInValidPageNo_ThenReturnEmptyListAsync()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(4, null, null, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 0);
        }

        [Test]
        public void WhenCallGetRadioNavigationWarningsWithInValidAdminListRecordPerPage_ThenThrowDivideByZeroException()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            Assert.ThrowsAsync(Is.TypeOf<DivideByZeroException>(),
                           async delegate { await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, null, string.Empty); });
        }

        #region PrivateMethod
        private static List<RadioNavigationalWarningsAdminList> GetFakeRadioNavigationalWarningList()
        {
            List<RadioNavigationalWarningsAdminList> radioNavigationalWarningList = new();
             radioNavigationalWarningList.Add( new RadioNavigationalWarningsAdminList()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdminList()
            {
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdminList()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdminList()
            {
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });
            return  radioNavigationalWarningList;
        }

        #endregion

    }
}
