using Microsoft.EntityFrameworkCore;
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
    public class RNWRepositoryTest
    {
        private RNWRepository _rnwRepository;
        private RadioNavigationalWarningsContext _context;
        private RadioNavigationalWarning _radioNavigationalWarning;
        private EditRadioNavigationalWarningAdmin _fakeRadioNavigationalWarningAdmin;

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
                Content = "test"
            };
            _fakeRadioNavigationalWarningAdmin = new()
            {
                Id = 1,
                WarningType = 1,
                WarningTypeName = "NAVAREA 1",
                Reference = "edittest",
                DateTimeGroup = new DateTime(2019, 1, 1),
                Summary = "editsummary",
                Content = "editcontent",
                IsDeleted = false,

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
            Assert.AreEqual(WarningTypes.NAVAREA_1, warningTypeList.Result[0].Id);
            Assert.AreEqual("NAVAREA 1", warningTypeList.Result[0].Name);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            List<RadioNavigationalWarningsAdmin> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(5, result[0].Id);
            Assert.AreEqual("011200 UTC Jan 20", result[3].DateTimeGroupRnwFormat);
            Assert.AreEqual("NAVAREA 1", result[0].WarningTypeName);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenIsDeletedShouldDisplayYesAndNoRespectively()
        {
            List<RadioNavigationalWarningsAdmin> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.AreEqual("No", result[0].IsDeleted);
            Assert.AreEqual("Yes", result[3].IsDeleted);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningWithContentLenthGreaterThan300Char_ThenWrapTheContent()
        {
            List<RadioNavigationalWarningsAdmin> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.IsTrue(result[3].Content.Length <= 303);
            Assert.IsTrue(result[3].Content.Contains("..."));
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
        public async Task WhenCallShowRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            int[] data = { 4 };
            List<RadioNavigationalWarningsData> result = await _rnwRepository.ShowRadioNavigationalWarningsDataList(data);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsLastModifiedDateTime_ThenReturnLastModifiedDateTime()
        {
            DateTime result = await _rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();
            Assert.AreEqual(new DateTime(2099, 02, 03), result);
        }

        [Test]
        public void WhenCallUpdateRadioNavigationalWarningsRecord_ThenUpdateRNWRecord()
        {
            Task result = _rnwRepository.UpdateRadioNavigationalWarning(_fakeRadioNavigationalWarningAdmin);
            Assert.IsTrue(result.IsCompleted);
        }

        [Test]
        public void WhenCallEditRadioNavigationalWarningsRecord_ThenReturnRecordForGivenId()
        {
            const int id = 1;
            EditRadioNavigationalWarningAdmin result = _rnwRepository.GetRadioNavigationalWarningById(id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("NAVAREA 1", result.WarningTypeName);
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
                IsDeleted = true
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
