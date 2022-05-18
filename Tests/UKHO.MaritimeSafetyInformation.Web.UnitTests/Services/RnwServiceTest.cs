using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RnwServiceTest
    {
        private ILogger<RnwService> _fakeLogger;
        private IRnwRepository _fakeRnwRepository;
        private RnwService _rnwService;
        private RadioNavigationalWarnings _fakeRadioNavigationalWarnings;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";
        private RadioNavigationalWarningsContext _fakeContext;
        private IRnwRepository _fakeRnwRepository;
        private ILogger<RnwService> _fakeLogger;
        private ILogger<RnwRepository> _fakeRnwRepositoryLogger;
        private IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;

        [SetUp]
        public void Setup()
        public void SetUp()
        {
            _fakeRadioNavigationalWarnings = new(){ WarningType = 1,
                                                    Reference = "test",
                                                    DateTimeGroup = DateTime.UtcNow,
                                                    Summary = "Test1",
                                                    Content = "test"};
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                                .UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);
            _fakeLogger = A.Fake<ILogger<RnwService>>();
            _fakeRnwRepository = A.Fake<IRnwRepository>();
            _fakeRnwRepositoryLogger = A.Fake<ILogger<RnwRepository>>();
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeRnwRepository = new RnwRepository(_fakeContext, _fakeRnwRepositoryLogger);
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;

            _rnwService = new RnwService(_fakeRnwRepository, _fakeLogger);
            _rnwService = new RnwService(_fakeRnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLogger);

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
                Name = "NAVAREA 1"
            };
            WarningType WarningType2 = new()
            {
                Id = 2,
                Name = "UK Coastal"
            };

            _fakeContext.WarningType.Add(WarningType1);
            _fakeContext.WarningType.Add(WarningType2);
            _fakeContext.SaveChanges();
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count > 0);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 0);
            Assert.IsTrue(result.CurrentPageIndex == 1);
        }

        [Test]
        public async Task WhenRequestToGetWarningTypes_ThenReturnListOfWarningType()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;

            A.CallTo(() => _fakeRnwRepository.GetWarningTypes()).Returns(new List<WarningType>() { new WarningType {Id=1, Name="test"}});

            List<WarningType> result = await _rnwService.GetWarningTypes();

            Assert.AreEqual("test", result[0].Name);
        }

        [Test]
        public async Task WhenPostValidRequest_ThenReturnTrue()
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeFilter_ThenReturnFilteredList()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 1, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 2);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId);
        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithYearFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 0, "2022", true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

            Assert.IsTrue(result);
        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithWarningTypeAndYearFilter_ThenReturnFilteredList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 1, "2020", true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 1);
            Assert.IsTrue(result.SrNo == 0);
        }

        [Test]
        public void WhenPostInvalidWarningTypeInRequest_ThenReturnFalse()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value recieved for parameter warningType"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidReferenceInRequest_ThenReturnFalse()
        public async Task WhenPostInvalidRequest_ThenReturnFalse()
        public async Task WhenCallGetRadioNavigationWarningsWithValidPageNo_ThenReturnFilteredList()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.Reference = "";
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(2, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 1);
            Assert.IsTrue(result.PageCount == 2);
            Assert.IsTrue(result.SrNo == 3);
        }

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value recieved for parameter reference"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidSummaryInRequest_ThenReturnFalse()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value recieved for parameter summary"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidContentInRequest_ThenReturnFalse()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.Content = "";
            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId);
        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithInValidPageNo_ThenReturnEmptyList()
        {
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(4, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 0);
        }

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value recieved for parameter content"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
            Assert.IsFalse(result);
        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithInValidAdminListRecordPerPage_ThenThrowExceptionWithNullObject()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 0;
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList == null);
        }

        [Test]
        public void WhenPostValidRequestWithException_ThenReturnException()
        public async Task WhenPostValidRequestWithException_ThenReturnFalse()
        public async Task WhenCallGetRadioNavigationWarningsWithNotReloadData_ThenReturnExisitngList()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
            await _rnwService.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, false, string.Empty);
            AddRadioNavigationWarningRecord();

            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, false, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 4);
        }

            A.CallTo(() => _fakeRnwRepository.AddRadioNavigationWarnings(A<RadioNavigationalWarnings>.Ignored)).Throws(new Exception());
        [Test]
        public async Task WhenCallGetRadioNavigationWarningsWithReloadData_ThenReturnUpdatedList()
        {
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 5;
            await _rnwService.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, false, string.Empty);
            AddRadioNavigationWarningRecord();

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId);
            RadioNavigationalWarningsAdminListFilter result = await _rnwService.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, true, string.Empty);
            Assert.IsTrue(result.RadioNavigationalWarningsAdminList.Count == 5);
        }

            Assert.IsFalse(result);
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
