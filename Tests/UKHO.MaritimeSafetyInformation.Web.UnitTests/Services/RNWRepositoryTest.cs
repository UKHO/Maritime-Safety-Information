using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RNWRepositoryTest
    {
        private RNWRepository _rnwRepository;
        private RadioNavigationalWarningsContext _context;
        private RadioNavigationalWarning _radioNavigationalWarning;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                    .UseInMemoryDatabase("msi-ut-db");
            _context = new RadioNavigationalWarningsContext(builder.Options);

            _context.RadioNavigationalWarnings.AddRange(GetFakeRadioNavigationalWarningList());
            _context.WarningType.AddRange(GetFakeWarningTypeList());
            _context.SaveChanges();

        }

        [SetUp]
        public void SetUp()
        {
            _radioNavigationalWarning = new()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "test",
                DateTimeGroup = new DateTime(2019, 1, 1),
                Summary = "Test1",
                Content = "test",
                IsDeleted = true,
            };
            _rnwRepository = new RNWRepository(_context);
        }

        [Test]
        public void WhenCallAddRadioNavigationWarningsMethod_ThenCreatedNewRNWRecord()
        {
            DateTime dateTime = DateTime.UtcNow;
            _radioNavigationalWarning.DateTimeGroup = dateTime;

            Task result = _rnwRepository.AddRadioNavigationWarning(_radioNavigationalWarning);

            Task<RadioNavigationalWarning> data = _context.RadioNavigationalWarnings.SingleOrDefaultAsync(b => b.Summary == "Test1" && b.DateTimeGroup == dateTime);

            Assert.IsTrue(result.IsCompleted);
            Assert.IsNotNull(data.Result.Summary);
            Assert.IsNotNull(data.Result.LastModified < DateTime.UtcNow);
        }

        [Test]
        public void WhenCallGetWarningTypeMethod_ThenReturnWarningType()
        {
            Task<List<WarningType>> warningTypeList = _rnwRepository.GetWarningTypes();

            Assert.IsNotNull(warningTypeList);
            Assert.IsInstanceOf(typeof(Task<List<WarningType>>), warningTypeList);
            Assert.AreEqual("NAVAREA 1", warningTypeList.Result.First(x=>x.Id==1).Name);
            Assert.AreEqual("UK Coastal", warningTypeList.Result.First(x=>x.Id==2).Name);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            List<RadioNavigationalWarningsAdmin> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("010000 UTC Jan 20", result.First(x=>x.Id == 2).DateTimeGroupRnwFormat);
            Assert.AreEqual("NAVAREA 1", result.First(x => x.Id == 3).WarningTypeName);
        }        

        [Test]
        public async Task WhenCallGetRadioNavigationWarningWithContentLengthGreaterThan300Char_ThenWrapTheContent()
        {
            List<RadioNavigationalWarningsAdmin> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.IsTrue(result.First(x => x.Id == 1).Content.Length <= 303);
            Assert.IsTrue(result.First(x => x.Id == 1).Content.Contains("..."));
        }

        [Test]
        public async Task WhenGetRadioNavigationWarningsIsCalled_ThenCheckIsStatusIsDisplayedCorrectly()
        {
            List<RadioNavigationalWarningsAdmin> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.AreEqual("Active", result.First(x=>x.Id==1).Status);
            Assert.AreEqual("Expired", result.First(x => x.Id == 2).Status);
            Assert.AreEqual("Expired", result.First(x => x.Id == 3).Status);
            Assert.AreEqual("Active", result.First(x => x.Id == 4).Status);
        }

        [Test]
        public async Task WhenCallGetYears_ThenReturnListAsync()
        {
            List<string> result = await _rnwRepository.GetYears();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("2020", result[0]);
            Assert.AreEqual("2021", result[1]);
            Assert.AreEqual("2022", result[2]);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            List<RadioNavigationalWarningsData> result = await _rnwRepository.GetRadioNavigationalWarningsDataList();
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task WhenCallGetSelectedRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            int[] data = { 4 };
            List<RadioNavigationalWarningsData> result = await _rnwRepository.GetSelectedRadioNavigationalWarningsDataList(data);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsLastModifiedDateTime_ThenReturnLastModifiedDateTime()
        {
            DateTime result = await _rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();
            Assert.AreEqual(new DateTime(2099, 02, 03), result);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsLastModifiedDateTimeWhenNoWarnings_ThenReturnDateTimeMin()
        {
            RadioNavigationalWarningsContext emptyContext = new(new DbContextOptionsBuilder<RadioNavigationalWarningsContext>().UseInMemoryDatabase("msi-ut-empty-db").Options);
            await emptyContext.SaveChangesAsync();
            RNWRepository rnwRepository = new(emptyContext);

            DateTime result = await rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();

            Assert.AreEqual(new DateTime(1, 1, 1), result);
        }

        [Test]
        public async Task WhenGetRadioNavigationalWarningsLastModifiedDateTimeIsCalled_ThenShouldExecuteCatch()
        {
            RadioNavigationalWarningsContext emptyContext = null;
            RNWRepository rnwRepository = new(emptyContext);

            DateTime result = await rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();
            Assert.AreEqual(new DateTime(1, 1, 1), result);
        }


        [Test]
        public void WhenCallUpdateRadioNavigationalWarningsRecord_ThenUpdateRNWRecord()
        {
            Task result = _rnwRepository.AddRadioNavigationWarning(_radioNavigationalWarning);
            Task isUpdate = _rnwRepository.UpdateRadioNavigationalWarning(_radioNavigationalWarning);
            Assert.IsTrue(result.IsCompleted);
            Assert.IsTrue(isUpdate.IsCompleted);
        }

        [Test]
        public void WhenCallEditRadioNavigationalWarningsRecord_ThenReturnRecordForGivenId()
        {
            const int id = 1;
            RadioNavigationalWarning result = _rnwRepository.GetRadioNavigationalWarningById(id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public async Task WhenCallCheckReferenceNumberExistOrNot_ThenReturnFalseValue()
        {
            const string referenceNumber = "test_Reference";
            bool result = await _rnwRepository.CheckReferenceNumberExistOrNot(WarningTypes.NAVAREA_1, referenceNumber);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task WhenCallCheckReferenceNumberExistOrNot_ThenReturnTrueValue()
        {
            const string referenceNumber = "RnwAdminListReference";
            bool result = await _rnwRepository.CheckReferenceNumberExistOrNot(WarningTypes.UK_Coastal, referenceNumber);

            Assert.IsTrue(result);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            _context.RadioNavigationalWarnings.RemoveRange(_context.RadioNavigationalWarnings);
            _context.WarningType.RemoveRange(_context.WarningType);
            _context.SaveChanges();
        }

        private static List<RadioNavigationalWarning> GetFakeRadioNavigationalWarningList()
        {
            List<RadioNavigationalWarning> radioNavigationalWarningList = new();
            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "1. NAVAREA I WARNINGS IN FORCE AT 221000 UTC APR 22:     2021 SERIES: 031.  2022 SERIES: 033, 041, 043, 044."
                 + "NOTES:  A. Texts of NAVAREA I Warnings issued each week are published in weekly editions of the ADMIRALTY Notices to"
                 + "Mariners bulletin (ANMB).  B. NAVAREA I Warnings less than 42 days old (033/22 onward) are promulgated via Enhanced"
                 + "Group Call (EGC) and/or relevant NAVTEX transmitters.  C. The complete texts of all in-force NAVAREA I warnings,"
                 + "including those which are no longer being broadcast, are reprinted in Section III of ANMB in weeks 1, 13, 26 and 39"
                 + "and are also available from the UKHO website at: www.admiralty.co.uk/RNW.  Alternatively, these may be requested by"
                 + "e-mail from NAVAREA I Co-ordinator at: navwarnings@ukho.gov.uk    2. Cancel NAVAREA I 042/22.",
                IsDeleted = false
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2020, 1, 1)
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2021, 1, 1),
                LastModified = new DateTime(2099, 02, 03)
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
                LastModified = new DateTime(2099, 01, 02)
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
    }
}
