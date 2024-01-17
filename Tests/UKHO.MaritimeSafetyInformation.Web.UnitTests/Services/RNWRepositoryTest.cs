using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
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
        private RNWRepository rnwRepository;
        private RadioNavigationalWarningsContext context;
        private RadioNavigationalWarning radioNavigationalWarning;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                    .UseInMemoryDatabase("msi-ut-db");
            context = new RadioNavigationalWarningsContext(builder.Options);

            context.RadioNavigationalWarnings.AddRange(GetFakeRadioNavigationalWarningList());
            context.WarningType.AddRange(GetFakeWarningTypeList());
            context.SaveChanges();

        }

        [SetUp]
        public void SetUp()
        {
            radioNavigationalWarning = new()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "test",
                DateTimeGroup = new DateTime(2019, 1, 1),
                Summary = "Test1",
                Content = "test",
                IsDeleted = true,
            };
            rnwRepository = new RNWRepository(context);
        }

        [Test]
        public void WhenCallAddRadioNavigationWarningsMethod_ThenCreatedNewRNWRecord()
        {
            DateTime dateTime = new DateTime(2022, 12, 1);
            radioNavigationalWarning.DateTimeGroup = dateTime;

            Task result = rnwRepository.AddRadioNavigationWarning(radioNavigationalWarning);

            Task<RadioNavigationalWarning> data = context.RadioNavigationalWarnings.SingleOrDefaultAsync(b => b.Summary == "Test1" && b.DateTimeGroup == dateTime);

            Assert.That(result.IsCompleted);
            Assert.That(data.Result.Summary, Is.Not.Null);
            Assert.That(data.Result.LastModified < DateTime.UtcNow);
        }

        [Test]
        public void WhenCallGetWarningTypeMethod_ThenReturnWarningType()
        {
            Task<List<WarningType>> warningTypeList = rnwRepository.GetWarningTypes();

            Assert.That(warningTypeList, Is.Not.Null);
            Assert.That(warningTypeList, Is.InstanceOf(typeof(Task<List<WarningType>>)));
            Assert.That("NAVAREA 1", Is.EqualTo(warningTypeList.Result.First(x => x.Id == 1).Name));
            Assert.That("UK Coastal", Is.EqualTo(warningTypeList.Result.First(x => x.Id == 2).Name));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarnings_ThenReturnListAsync()
        {
            List<RadioNavigationalWarningsAdmin> result = await rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.That(4, Is.EqualTo(result.Count));
            Assert.That("010000 UTC Jan 20", Is.EqualTo(result.First(x => x.Id == 2).DateTimeGroupRnwFormat));
            Assert.That("NAVAREA 1", Is.EqualTo(result.First(x => x.Id == 3).WarningTypeName));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationWarningWithContentLengthGreaterThan300Char_ThenWrapTheContent()
        {
            List<RadioNavigationalWarningsAdmin> result = await rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.That(result.First(x => x.Id == 1).Content.Length <= 303);
            Assert.That(result.First(x => x.Id == 1).Content.Contains("..."));
        }

        [Test]
        public async Task WhenGetRadioNavigationWarningsIsCalled_ThenCheckIsStatusIsDisplayedCorrectly()
        {
            List<RadioNavigationalWarningsAdmin> result = await rnwRepository.GetRadioNavigationWarningsAdminList();
            Assert.That("Active", Is.EqualTo(result.First(x => x.Id == 1).Status));
            Assert.That("Expired", Is.EqualTo(result.First(x => x.Id == 2).Status));
            Assert.That("Expired", Is.EqualTo(result.First(x => x.Id == 3).Status));
            Assert.That("Active", Is.EqualTo(result.First(x => x.Id == 4).Status));
        }

        [Test]
        public async Task WhenCallGetYears_ThenReturnListAsync()
        {
            List<string> result = await rnwRepository.GetYears();
            Assert.That(3, Is.EqualTo(result.Count));
            Assert.That("2020", Is.EqualTo(result[0]));
            Assert.That("2021", Is.EqualTo(result[1]));
            Assert.That("2022", Is.EqualTo(result[2]));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            List<RadioNavigationalWarningsData> result = await rnwRepository.GetRadioNavigationalWarningsDataList();
            Assert.That(2, Is.EqualTo(result.Count));
        }

        [Test]
        public async Task WhenCallGetSelectedRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            int[] data = { 4 };
            List<RadioNavigationalWarningsData> result = await rnwRepository.GetSelectedRadioNavigationalWarningsDataList(data);
            Assert.That(1, Is.EqualTo(result.Count));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsLastModifiedDateTime_ThenReturnLastModifiedDateTime()
        {
            DateTime result = await rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();
            Assert.That(new DateTime(2099, 02, 03), Is.EqualTo(result));
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsLastModifiedDateTimeWhenNoWarnings_ThenReturnDateTimeMin()
        {
            RadioNavigationalWarningsContext emptyContext = new(new DbContextOptionsBuilder<RadioNavigationalWarningsContext>().UseInMemoryDatabase("msi-ut-empty-db").Options);
            await emptyContext.SaveChangesAsync();
            RNWRepository rnwRepository = new(emptyContext);

            DateTime result = await rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();

            Assert.That(new DateTime(1, 1, 1), Is.EqualTo(result));
        }

        [Test]
        public async Task WhenGetRadioNavigationalWarningsLastModifiedDateTimeIsCalled_ThenShouldExecuteCatch()
        {
            RadioNavigationalWarningsContext emptyContext = null;
            RNWRepository rnwRepository = new(emptyContext);

            DateTime result = await rnwRepository.GetRadioNavigationalWarningsLastModifiedDateTime();
            Assert.That(new DateTime(1, 1, 1), Is.EqualTo(result));
        }


        [Test]
        public void WhenCallUpdateRadioNavigationalWarningsRecord_ThenUpdateRNWRecord()
        {
            Task result = rnwRepository.AddRadioNavigationWarning(radioNavigationalWarning);
            Task isUpdate = rnwRepository.UpdateRadioNavigationalWarning(radioNavigationalWarning);
            Assert.That(result.IsCompleted);
            Assert.That(isUpdate.IsCompleted);
        }

        [Test]
        public void WhenCallEditRadioNavigationalWarningsRecord_ThenReturnRecordForGivenId()
        {
            const int id = 1;
            RadioNavigationalWarning result = rnwRepository.GetRadioNavigationalWarningById(id);
            Assert.That(result, Is.Not.Null);
            Assert.That(1, Is.EqualTo(result.Id));
        }

        [Test]
        public async Task WhenCallCheckReferenceNumberExistOrNot_ThenReturnFalseValue()
        {
            const string referenceNumber = "test_Reference";
            bool result = await rnwRepository.CheckReferenceNumberExistOrNot(WarningTypes.NAVAREA_1, referenceNumber);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task WhenCallCheckReferenceNumberExistOrNot_ThenReturnTrueValue()
        {
            const string referenceNumber = "RnwAdminListReference";
            bool result = await rnwRepository.CheckReferenceNumberExistOrNot(WarningTypes.UK_Coastal, referenceNumber);

            Assert.That(result);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            context.RadioNavigationalWarnings.RemoveRange(context.RadioNavigationalWarnings);
            context.WarningType.RemoveRange(context.WarningType);
            context.SaveChanges();
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
