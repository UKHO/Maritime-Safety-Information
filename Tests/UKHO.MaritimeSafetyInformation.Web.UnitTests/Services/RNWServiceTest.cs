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
        private ILogger<RNWService> _fakeLogger;
        private IRNWRepository _fakeRnwRepository;
        private IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;
        private RNWService _rnwService;
        private RadioNavigationalWarning _fakeRadioNavigationalWarning;
        private EditRadioNavigationalWarningAdmin _fakeRadioNavigationalWarningAdmin;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeRadioNavigationalWarning = new()
            {
                WarningType = 1,
                Reference = "test",
                DateTimeGroup = DateTime.UtcNow,
                Summary = "Test1",
                Content = "test"
            };

            _fakeRadioNavigationalWarningAdmin = new()
            {
                WarningType = 1,
                Reference = "test",
                DateTimeGroup = DateTime.UtcNow,
                Summary = "Test1",
                Content = "test",
                WarningTypeName = "NavArea",
                IsDeleted = true
            };

            _fakeLogger = A.Fake<ILogger<RNWService>>();
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeRnwRepository = A.Fake<IRNWRepository>();
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;

            _rnwService = new RNWService(_fakeRnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLogger);
        }

        [Test]
        public async Task WhenRequestToGetWarningTypes_ThenReturnListOfWarningType()
        {
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
        public void WhenPostInvalidSummaryInRequestForEdit_ThenReturnArgumentNullException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidContentInRequestForEdit_ThenReturnArgumentNullException()
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
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, null, string.Empty);
            Assert.Greater(result.RadioNavigationalWarningsAdminList.Count, 0);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(0, result.SrNo);
            Assert.AreEqual(1, result.CurrentPageIndex);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 1, null, string.Empty);
            Assert.AreEqual(2, result.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, result.PageCount);
            Assert.AreEqual(0, result.SrNo);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithYearFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, null, 2022, string.Empty);
            Assert.AreEqual(1, result.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, result.PageCount);
            Assert.AreEqual(0, result.SrNo);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 2, 2020, string.Empty);
            Assert.AreEqual(1, result.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(1, result.PageCount);
            Assert.AreEqual(0, result.SrNo);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithValidPageNo_ThenReturnFilteredListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(2, null, null, string.Empty);
            Assert.AreEqual(1, result.RadioNavigationalWarningsAdminList.Count);
            Assert.AreEqual(2, result.PageCount);
            Assert.AreEqual(3, result.SrNo);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithInValidPageNo_ThenReturnEmptyListAsync()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(4, null, null, string.Empty);
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

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnWarnings()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationalWarningsDataList()).Returns(GetFakeRadioNavigationalWarningsDataList());
            List<RadioNavigationalWarningsData> result = await _rnwService.GetRadioNavigationalWarningsData(string.Empty);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void WhenCallGetRadioNavigationalWarningsDataListWithException_ThenReturnException()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationalWarningsDataList()).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.GetRadioNavigationalWarningsData(string.Empty); });
        }

        [Test]
        public async Task WhenCallGetSelectedRadioNavigationalWarningsDataList_ThenReturnWarnings()
        {
            A.CallTo(() => _fakeRnwRepository.GetSelectedRadioNavigationalWarningsDataList(Array.Empty<int>())).Returns(GetFakeRadioNavigationalWarningsDataList());
            List<RadioNavigationalWarningsData> result = await _rnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void WhenCallGetSelectedRadioNavigationalWarningsDataListWithException_ThenReturnException()
        {
            A.CallTo(() => _fakeRnwRepository.GetSelectedRadioNavigationalWarningsDataList(Array.Empty<int>())).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty); });
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsLastModifiedDateTime_ThenReturnLastModifiedDateTime()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime()).Returns(new DateTime(2020, 01, 01, 13, 14, 15));
            string result = await _rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(string.Empty);
            Assert.AreEqual("010114 UTC Jan 20", result);
        }

        [Test]
        public void WhenEditRadioNavigationalRecordWithInvalidRecordId_ThenReturnException()
        {
            _fakeRadioNavigationalWarningAdmin.Id = 0;
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationalWarningById(A<int>.Ignored)).Throws(new Exception()); 
            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
            delegate { _rnwService.GetRadioNavigationalWarningById(_fakeRadioNavigationalWarningAdmin.Id, CorrelationId); return Task.CompletedTask; });
        }

        [Test]
        public void WhenPostInvalidWarningTypeInRequestForEdit_ThenReturnInvalidDataException()
        {
            _fakeRadioNavigationalWarningAdmin.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                             async delegate { await _rnwService.EditRadioNavigationalWarningsRecord(_fakeRadioNavigationalWarningAdmin, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidReferenceInRequestForEdit_ThenReturnArgumentNullException()
        {
            _fakeRadioNavigationalWarningAdmin.Reference = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                             async delegate { await _rnwService.EditRadioNavigationalWarningsRecord(_fakeRadioNavigationalWarningAdmin, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidContentInRequestForEdit_ThenReturnException()
        {
            _fakeRadioNavigationalWarningAdmin.Content = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter content"),
                             async delegate { await _rnwService.EditRadioNavigationalWarningsRecord(_fakeRadioNavigationalWarningAdmin, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidSummaryInRequestForEdit_ThenReturnException()
        {
            _fakeRadioNavigationalWarningAdmin.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                             async delegate { await _rnwService.EditRadioNavigationalWarningsRecord(_fakeRadioNavigationalWarningAdmin, CorrelationId); });
        }

        [Test]
        public void WhenCallUpdateRadioNavigationalWarningsRecord_ThenUpdateRNWRecord()
        {
            Task result = _rnwService.EditRadioNavigationalWarningsRecord(_fakeRadioNavigationalWarningAdmin, CorrelationId);
            Assert.IsTrue(result.IsCompleted);
        }

        [Test]
        public void WhenEditValidRequestWithException_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarningAdmin.DateTimeGroup = dateTime;
            A.CallTo(() => _fakeRnwRepository.UpdateRadioNavigationalWarning(A<EditRadioNavigationalWarningAdmin>.Ignored)).Throws(new Exception());
            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.EditRadioNavigationalWarningsRecord(_fakeRadioNavigationalWarningAdmin, CorrelationId); });
        }

        private static List<RadioNavigationalWarningsAdmin> GetFakeRadioNavigationalWarningList()
        {
            List<RadioNavigationalWarningsAdmin> radioNavigationalWarningList = new();
            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdmin()
            {
                WarningType = 1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdmin()
            {
                WarningType = 2,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdmin()
            {
                WarningType = 1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarningsAdmin()
            {
                WarningType = 2,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });
            return radioNavigationalWarningList;
        }

        private static List<RadioNavigationalWarningsData> GetFakeRadioNavigationalWarningsDataList()
        {
            return new List<RadioNavigationalWarningsData>() { new RadioNavigationalWarningsData() };
        }
    }
}
