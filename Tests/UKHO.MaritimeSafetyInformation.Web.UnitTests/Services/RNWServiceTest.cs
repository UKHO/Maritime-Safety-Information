using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RNWServiceTest
    {
        private RNWService _rnwService;
        private IRNWRepository _fakeRnwRepository;
        private ILogger<RNWService> _fakeLogger;
        private IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;
        private RadioNavigationalWarning _fakeRadioNavigationalWarning;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeRadioNavigationalWarning = new(){ WarningType = 1,
                                                    Reference = "test",
                                                    DateTimeGroup = DateTime.UtcNow,
                                                    Summary = "Test1",
                                                    Content = "test"};

            _fakeLogger = A.Fake<ILogger<RNWService>>();
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeRnwRepository = A.Fake<IRNWRepository>();
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;

            _rnwService = new RNWService(_fakeRnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLogger);
        }

        [Test]
        public async Task WhenRequestToGetWarningTypes_ThenReturnListOfWarningType()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            A.CallTo(() => _fakeRnwRepository.GetWarningTypes()).Returns(new List<WarningType>() { new WarningType { Id = 1, Name = "test" } });

            List<WarningType> result = await _rnwService.GetWarningTypes();

            Assert.AreEqual("test", result[0].Name);
        }

        [Test]
        public async Task WhenPostValidRequest_ThenReturnTrue()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId);

            Assert.IsTrue(result);
        }

        [Test]
        public void WhenPostInvalidWarningTypeInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidReferenceInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.Reference = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidSummaryInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidContentInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.Content = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter content"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostValidRequestWithException_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            A.CallTo(() => _fakeRnwRepository.AddRadioNavigationWarning(A<RadioNavigationalWarning>.Ignored)).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
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
            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdminList()
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
            return radioNavigationalWarningList;
        }
        #endregion
    }
}
