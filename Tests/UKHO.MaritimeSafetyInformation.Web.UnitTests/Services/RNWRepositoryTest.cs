using Microsoft.EntityFrameworkCore;
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
    public class RNWRepositoryTest
    {
        private RNWRepository _rnwRepository;
        private RadioNavigationalWarningsContext _fakeContext;
        private RadioNavigationalWarning _fakeRadioNavigationalWarning;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                    .UseInMemoryDatabase("msi-ut-db");
            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);

            _fakeContext.RadioNavigationalWarning.AddRange(GetFakeRadioNavigationalWarningList());
            _fakeContext.WarningType.AddRange(GetFakeWarningTypeList());
            _fakeContext.SaveChanges();
        }

        [SetUp]
        public void SetUp()
        {
            _fakeRadioNavigationalWarning = new()
            {
                WarningType = 1,
                Reference = "test",
                DateTimeGroup = new DateTime(2019, 1, 1),
                Summary = "Test1",
                Content = "test"
            };

            _rnwRepository = new RNWRepository(_fakeContext);
        }

        [Test]
        public void WhenCallAddRadioNavigationWarningsMethod_ThenCreatedNewRNWRecord()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            Task result = _rnwRepository.AddRadioNavigationWarning(_fakeRadioNavigationalWarning);

            Task<RadioNavigationalWarning> data = _fakeContext.RadioNavigationalWarnings.SingleOrDefaultAsync(b => b.Summary == "Test1" && b.DateTimeGroup == dateTime);

            Assert.IsTrue(result.IsCompleted);
            Assert.IsNotNull(data.Result.Summary);
        }

        [Test]
        public async Task WhenCallGetWarningTypeMethod_ThenReturnWarningType()
        {
            WarningType warningType = new() { Name = "test" };

            _fakeContext.WarningType.Add(warningType);
            await _fakeContext.SaveChangesAsync();

            Task<List<WarningType>> warningTypeList = _rnwRepository.GetWarningTypes();

            Assert.IsNotNull(warningTypeList);
            Assert.IsInstanceOf(typeof(Task<List<WarningType>>), warningTypeList);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            List<RadioNavigationalWarningsAdminList> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.IsTrue(result.Count == 5);
            Assert.AreEqual(5, result[0].Id);
            Assert.AreEqual("190219 UTC May 22", result[0].DateTimeGroupRnwFormat);
            Assert.AreEqual("NAVAREA 1", result[0].WarningTypeName);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenIsDeletedShouldDisplayYesAndNoRespectively()
        {
            List<RadioNavigationalWarningsAdminList> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.IsTrue(result[0].IsDeleted == "No");
            Assert.IsTrue(result[3].IsDeleted == "Yes");
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningWithContentLenthGreaterThan300Char_ThenWrapTheContent()
        {
            List<RadioNavigationalWarningsAdminList> result = await _rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.IsTrue(result[3].Content.Length <= 303);
            Assert.IsTrue(result[3].Content.Contains("..."));
        }

        [Test]
        public async Task WhenCallGetYears_ThenReturnListAsync()
        {
            List<string> result = await _rnwRepository.GetYears();
            Assert.IsTrue(result.Count == 3);
            Assert.AreEqual("2020", result[0]);
            Assert.AreEqual("2021", result[1]);
            Assert.AreEqual("2022", result[2]);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            _fakeContext.RadioNavigationalWarning.RemoveRange(_fakeContext.RadioNavigationalWarning);
            _fakeContext.WarningType.RemoveRange(_fakeContext.WarningType);
            _fakeContext.SaveChanges();
        }

        #region PrivateMethod
        private static List<RadioNavigationalWarning> GetFakeRadioNavigationalWarningList()
        {
            List<RadioNavigationalWarning> radioNavigationalWarningList = new();
            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
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
                WarningType = 2,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = 1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent"
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
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
