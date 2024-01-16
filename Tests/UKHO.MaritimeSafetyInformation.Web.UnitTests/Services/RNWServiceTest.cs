using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
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
        private ILogger<RNWService> fakeLogger;
        private IRNWRepository fakeRnwRepository;
        private IOptions<RadioNavigationalWarningConfiguration> fakeRadioNavigationalWarningConfiguration;
        private RNWService rnwService;
        private RadioNavigationalWarning fakeRadioNavigationalWarning;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            fakeRadioNavigationalWarning = new()
            {
                WarningType = 1,
                Reference = "test",
                DateTimeGroup = DateTime.UtcNow,
                Summary = "Test1",
                Content = "test",
                IsDeleted = true
            };

            fakeLogger = A.Fake<ILogger<RNWService>>();
            fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            fakeRnwRepository = A.Fake<IRNWRepository>();
            fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            rnwService = new RNWService(fakeRnwRepository, fakeRadioNavigationalWarningConfiguration, fakeLogger);
        }

        [Test]
        public async Task WhenRequestToGetWarningTypes_ThenReturnListOfWarningType()
        {
            A.CallTo(() => fakeRnwRepository.GetWarningTypes()).Returns(new List<WarningType>() { new WarningType { Id = 1, Name = "test" } });

            List<WarningType> result = await rnwService.GetWarningTypes();

            Assert.That("test", Is.EqualTo(result[0].Name));
        }

        [Test]
        public async Task WhenPostValidRequestWithFlagSkipCheckDuplicateReferenceAsTrue_ThenReturnTrue()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            bool skipDuplicateReferenceCheck = true;

            A.CallTo(() => fakeRnwRepository.CheckReferenceNumberExistOrNot(A<int>.Ignored, A<string>.Ignored)).Returns(false);

            bool result = await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, skipDuplicateReferenceCheck, "testUser");

            Assert.That(result);
        }

        [Test]
        public async Task WhenPostValidRequestWithNewReferenceNumber_ThenReturnTrue()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            bool skipDuplicateReferenceCheck = false;

            A.CallTo(() => fakeRnwRepository.CheckReferenceNumberExistOrNot(A<int>.Ignored, A<string>.Ignored)).Returns(false);

            bool result = await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, skipDuplicateReferenceCheck, "testUser");

            Assert.That(result);
        }

        [Test]
        public async Task WhenPostValidRequestWithExistReferenceNumber_ThenReturnFalse()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            bool skipDuplicateReferenceCheck = false;

            A.CallTo(() => fakeRnwRepository.CheckReferenceNumberExistOrNot(A<int>.Ignored, A<string>.Ignored)).Returns(true);

            bool result = await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, skipDuplicateReferenceCheck, "testUser");

            Assert.That(result, Is.False);
        }

        [Test]
        public void WhenPostInvalidWarningTypeInRequest_ThenReturnInvalidDataException()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            fakeRadioNavigationalWarning.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                             async delegate { await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, false, "testUser"); });
        }

        [Test]
        public void WhenPostInvalidReferenceInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            fakeRadioNavigationalWarning.Reference = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                             async delegate { await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, false, "testUser"); });
        }

        [Test]
        public void WhenPostInvalidSummaryInRequestForEdit_ThenReturnArgumentNullException()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            fakeRadioNavigationalWarning.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                             async delegate { await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, false, "testUser"); });
        }

        [Test]
        public void WhenPostInvalidContentInRequestForEdit_ThenReturnArgumentNullException()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            fakeRadioNavigationalWarning.Content = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter content"),
                             async delegate { await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, false, "testUser"); });
        }

        [Test]
        public void WhenPostValidRequestWithException_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            A.CallTo(() => fakeRnwRepository.CheckReferenceNumberExistOrNot(A<int>.Ignored, A<string>.Ignored)).Returns(false);

            A.CallTo(() => fakeRnwRepository.AddRadioNavigationWarning(A<RadioNavigationalWarning>.Ignored)).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await rnwService.CreateNewRadioNavigationWarningsRecord(fakeRadioNavigationalWarning, CorrelationId, false, "testUser"); });
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await rnwService.GetRadioNavigationWarningsForAdmin(1, null, null, string.Empty);
            Assert.That(result.RadioNavigationalWarningsAdminList.Count > 0);
            Assert.That(2, Is.EqualTo(result.PageCount));
            Assert.That(0, Is.EqualTo(result.SrNo));
            Assert.That(1, Is.EqualTo(result.CurrentPageIndex));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeFilter_ThenReturnFilteredListAsync()
        {
            fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await rnwService.GetRadioNavigationWarningsForAdmin(1, 1, null, string.Empty);
            Assert.That(2, Is.EqualTo(result.RadioNavigationalWarningsAdminList.Count));
            Assert.That(1, Is.EqualTo(result.PageCount));
            Assert.That(0, Is.EqualTo(result.SrNo));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithYearFilter_ThenReturnFilteredListAsync()
        {
            fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await rnwService.GetRadioNavigationWarningsForAdmin(1, null, 2022, string.Empty);
            Assert.That(1, Is.EqualTo(result.RadioNavigationalWarningsAdminList.Count));
            Assert.That(1, Is.EqualTo(result.PageCount));
            Assert.That(0, Is.EqualTo(result.SrNo));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeAndYearFilter_ThenReturnFilteredListAsync()
        {
            fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await rnwService.GetRadioNavigationWarningsForAdmin(1, 2, 2020, string.Empty);
            Assert.That(1, Is.EqualTo(result.RadioNavigationalWarningsAdminList.Count));
            Assert.That(1, Is.EqualTo(result.PageCount));
            Assert.That(0, Is.EqualTo(result.SrNo));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithValidPageNo_ThenReturnFilteredListAsync()
        {
            fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            RadioNavigationalWarningsAdminFilter result = await rnwService.GetRadioNavigationWarningsForAdmin(2, null, null, string.Empty);
            Assert.That(1, Is.EqualTo(result.RadioNavigationalWarningsAdminList.Count));
            Assert.That(2, Is.EqualTo(result.PageCount));
            Assert.That(3, Is.EqualTo(result.SrNo));
        }

        [Test]
        public void WhenCallGetRadioNavigationWarningsWithInValidAdminListRecordPerPage_ThenThrowDivideByZeroException()
        {
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationWarningsAdminList()).Returns(GetFakeRadioNavigationalWarningList());
            fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            Assert.ThrowsAsync(Is.TypeOf<DivideByZeroException>(),
                           async delegate { await rnwService.GetRadioNavigationWarningsForAdmin(1, null, null, string.Empty); });
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnWarnings()
        {
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationalWarningsDataList()).Returns(GetFakeRadioNavigationalWarningsDataList());
            List<RadioNavigationalWarningsData> result = await rnwService.GetRadioNavigationalWarningsData(string.Empty);
            Assert.That(1, Is.EqualTo(result.Count));
        }

        [Test]
        public void WhenCallGetRadioNavigationalWarningsDataListWithException_ThenReturnException()
        {
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationalWarningsDataList()).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await rnwService.GetRadioNavigationalWarningsData(string.Empty); });
        }

        [Test]
        public void WhenCallGetRadioNavigationalWarningsWithNoData_ThenShouldThrowInvalidDataException()
        {
            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = new List<RadioNavigationalWarningsData>();
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationalWarningsDataList()).Returns(radioNavigationalWarningsData);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("No data received for RNW database"),
                               async delegate { await rnwService.GetRadioNavigationalWarningsData(string.Empty); });
        }

        [Test]
        public async Task WhenCallGetSelectedRadioNavigationalWarningsDataList_ThenReturnWarnings()
        {
            A.CallTo(() => fakeRnwRepository.GetSelectedRadioNavigationalWarningsDataList(Array.Empty<int>())).Returns(GetFakeRadioNavigationalWarningsDataList());
            List<RadioNavigationalWarningsData> result = await rnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty);
            Assert.That(1, Is.EqualTo(result.Count));
        }

        [Test]
        public void WhenCallGetSelectedRadioNavigationalWarningsDataListWithException_ThenReturnException()
        {
            A.CallTo(() => fakeRnwRepository.GetSelectedRadioNavigationalWarningsDataList(Array.Empty<int>())).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await rnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty); });
        }

        [Test]
        public void WhenCallGetSelectedRadioNavigationalWarningsWithNoData_ThenShouldThrowInvalidDataException()
        {
            List<RadioNavigationalWarningsData> radioNavigationalWarningsData = new List<RadioNavigationalWarningsData>();

            A.CallTo(() => fakeRnwRepository.GetSelectedRadioNavigationalWarningsDataList(Array.Empty<int>())).Returns(radioNavigationalWarningsData);

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("No data received from RNW database for selected warnings"),
                               async delegate { await rnwService.GetSelectedRadioNavigationalWarningsData(Array.Empty<int>(), string.Empty); });
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsLastModifiedDateTime_ThenReturnLastModifiedDateTime()
        {
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime()).Returns(new DateTime(2020, 01, 01, 13, 14, 15));
            string result = await rnwService.GetRadioNavigationalWarningsLastModifiedDateTime(string.Empty);
            Assert.That("011314 UTC Jan 20", Is.EqualTo(result));
        }

        [Test]
        public void WhenEditRadioNavigationalRecordWithInvalidRecordId_ThenReturnException()
        {
            fakeRadioNavigationalWarning.Id = 0;
            A.CallTo(() => fakeRnwRepository.GetRadioNavigationalWarningById(A<int>.Ignored)).Throws(new Exception());
            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
            delegate { rnwService.GetRadioNavigationalWarningById(fakeRadioNavigationalWarning.Id, CorrelationId); return Task.CompletedTask; });
        }

        [Test]
        public void WhenPostInvalidWarningTypeInRequestForEdit_ThenReturnInvalidDataException()
        {
            fakeRadioNavigationalWarning.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                             async delegate { await rnwService.EditRadioNavigationalWarningsRecord(fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidReferenceInRequestForEdit_ThenReturnArgumentNullException()
        {
            fakeRadioNavigationalWarning.Reference = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                             async delegate { await rnwService.EditRadioNavigationalWarningsRecord(fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidContentInRequestForEdit_ThenReturnException()
        {
            fakeRadioNavigationalWarning.Content = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter content"),
                             async delegate { await rnwService.EditRadioNavigationalWarningsRecord(fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidSummaryInRequestForEdit_ThenReturnException()
        {
            fakeRadioNavigationalWarning.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                             async delegate { await rnwService.EditRadioNavigationalWarningsRecord(fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenCallUpdateRadioNavigationalWarningsRecord_ThenUpdateRNWRecord()
        {
            Task result = rnwService.EditRadioNavigationalWarningsRecord(fakeRadioNavigationalWarning, CorrelationId);
            Assert.That(result.IsCompleted);
        }

        [Test]
        public void WhenEditValidRequestWithException_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            A.CallTo(() => fakeRnwRepository.UpdateRadioNavigationalWarning(A<RadioNavigationalWarning>.Ignored)).Throws(new Exception());
            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await rnwService.EditRadioNavigationalWarningsRecord(fakeRadioNavigationalWarning, CorrelationId); });
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
