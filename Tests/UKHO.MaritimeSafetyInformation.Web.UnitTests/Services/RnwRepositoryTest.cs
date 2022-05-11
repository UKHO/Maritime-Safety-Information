using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common;
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
        private RadioNavigationalWarnings _fakeRadioNavigationalWarnings;
        private ILogger<RnwRepository> _fakeLogger;

        [SetUp]
        public void SetUp()
        {
            _fakeRadioNavigationalWarnings = new(){ WarningType = 1,
                                                    Reference = "test",
                                                    DateTimeGroup = DateTime.UtcNow,
                                                    Summary = "Test1",
                                                    Content = "test" };
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                                .UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);
            _fakeLogger = A.Fake<ILogger<RnwRepository>>();

            _rnwRepository = new RnwRepository(_fakeContext, _fakeLogger);
            _rnwRepository = new RnwRepository(_fakeContext);

            _fakeContext.RadioNavigationalWarnings.RemoveRange(_fakeContext.RadioNavigationalWarnings);
            _fakeContext.WarningType.RemoveRange(_fakeContext.WarningType);
        }

            RadioNavigationalWarnings radioNavigationalWarning1 = new()
        [Test]
        public void WhenCallAddRadioNavigationWarningsMethod_ThenCreatedNewRNWRecord()
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
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;

            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning1);
            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning2);
            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning3);
            _fakeContext.RadioNavigationalWarnings.Add(radioNavigationalWarning4);
            Task result = _rnwRepository.AddRadioNavigationWarnings(_fakeRadioNavigationalWarnings);

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
            Task<RadioNavigationalWarnings> data = _fakeContext.RadioNavigationalWarnings.SingleOrDefaultAsync(b => b.Summary == "Test1" && b.DateTimeGroup == _fakeDateTime);

            _fakeContext.WarningType.Add(WarningType1);
            _fakeContext.WarningType.Add(WarningType2);
            _fakeContext.SaveChanges();
            Assert.IsTrue(result.IsCompleted);
            Assert.IsNotNull(data.Result.Summary);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        public async Task WhenCallGetWarningTypeMethod_ThenReturnWarningType()
        {
            List<RadioNavigationalWarningsAdminList> result = await _rnwRepository.GetRadioNavigationWarningsAdminList(string.Empty);            
            Assert.IsTrue(result.Count == 4);
            Assert.AreEqual(4, result[0].Id);
            Assert.AreEqual("011200 UTC Jan 22", result[0].DateTimeGroupRnwFormat);
            Assert.AreEqual("UK Coastal", result[0].WarningTypeName);
        }
            WarningType warningType = new() { Name = "test" };

            _fakeContext.WarningType.Add(warningType);
            await _fakeContext.SaveChangesAsync();

        [Test]
        public async Task WhenCallGetWarningTypes_ThenReturnListAsync()
        {
            List<WarningType> result = await _rnwRepository.GetWarningTypes();
            Assert.IsTrue(result.Count == 2);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("NAVAREA 1", result[0].Name);
            Assert.AreEqual("UK Coastal", result[1].Name);
        }
            Task<List<WarningType>> warningTypeList = _rnwRepository.GetWarningTypes();

        [Test]
        public async Task WhenCallGetYears_ThenReturnListAsync()
        {
            List<string> result = await _rnwRepository.GetYears();
            Assert.IsTrue(result.Count == 3);
            Assert.AreEqual("2022", result[0]);
            Assert.AreEqual("2021", result[1]);
            Assert.AreEqual("2020", result[2]);
            Assert.IsNotNull(warningTypeList);
            Assert.IsInstanceOf(typeof(Task<List<WarningType>>), warningTypeList);
        }
    }
}
