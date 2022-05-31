using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings
{
    internal class BaseRNWTest
    {
        protected readonly RadioNavigationalWarningsContext FakeContext;
        protected readonly IOptions<RadioNavigationalWarningConfiguration> FakeRadioNavigationalWarningConfiguration;
        protected readonly IHttpContextAccessor FakeHttpContextAccessor;
        protected readonly ILogger<RNWService> FakeLoggerRnwService;

        public BaseRNWTest()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                    .UseInMemoryDatabase("msi-in-db");
            FakeContext = new RadioNavigationalWarningsContext(builder.Options);
            FakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            FakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            FakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            FakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
        }

        protected async Task SeedRadioNavigationalWarnings(List<RadioNavigationalWarning> radioNavigationalWarning)
        {
            FakeContext.RadioNavigationalWarnings.AddRange(radioNavigationalWarning);
            await FakeContext.SaveChangesAsync();
        }

        protected async Task SeedWarningType(List<WarningType> warningTypes)
        {
            FakeContext.WarningType.AddRange(warningTypes);
            await FakeContext.SaveChangesAsync();
        }

        protected async Task DeSeedRadioNavigationalWarnings()
        {
            DbSet<RadioNavigationalWarning> warnings = FakeContext.RadioNavigationalWarnings;
            FakeContext.RadioNavigationalWarnings.RemoveRange(warnings);
            await FakeContext.SaveChangesAsync();
        }

        protected async Task DeSeedWarningType()
        {
            DbSet<WarningType> warningType = FakeContext.WarningType;
            FakeContext.WarningType.RemoveRange(warningType);
            await FakeContext.SaveChangesAsync();
        }

        protected static List<RadioNavigationalWarning> GetFakeRadioNavigationalWarnings()
        {
            List<RadioNavigationalWarning> radioNavigationalWarningList = new();

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                IsDeleted = true,
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2019, 1, 1),

            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2023, 1, 1),
                Summary = "NORTHEAST ATLANTIC. Outer Hebrides Westwards. Live weapons firing in progress.",
                Content = "ENGLAND, EAST COAST.   Holy Island Eastwards.   1. Plough Seat light buoy, 55-40.4N 001-45.0W, unlit.  2. Cancel WZ 224.",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReference",
                DateTimeGroup = new DateTime(2024, 1, 1),
                Summary = "DOVER STRAIT, EASTERN PART. Sandettie Bank North-westwards. Inter Bank light-buoy, Racon inoperative.",
                Content = "1. NAVAREA I WARNINGS IN FORCE AT 221000 UTC APR 22:     2021 SERIES: 031.  2022 SERIES: 033, 041, 043, 044."
                 + "NOTES:  A. Texts of NAVAREA I Warnings issued each week are published in weekly editions of the ADMIRALTY Notices to"
                 + "Mariners bulletin (ANMB).  B. NAVAREA I Warnings less than 42 days old (033/22 onward) are promulgated via Enhanced"
                 + "Group Call (EGC) and/or relevant NAVTEX transmitters.  C. The complete texts of all in-force NAVAREA I warnings,"
                 + "including those which are no longer being broadcast, are reprinted in Section III of ANMB in weeks 1, 13, 26 and 39"
                 + "and are also available from the UKHO website at: www.admiralty.co.uk/RNW.  Alternatively, these may be requested by"
                 + "e-mail from NAVAREA I Co-ordinator at: navwarnings@ukho.gov.uk    2. Cancel NAVAREA I 042/22.",
                ExpiryDate = new DateTime(2099, 1, 1),
            });
            return radioNavigationalWarningList;
        }

        protected static List<WarningType> GetFakeWarningTypes()
        {
            List<WarningType> warningTypes = new();
            warningTypes.Add(new WarningType()
            {
                Id = 1,
                Name = "NAVAREA 1"
            });

            warningTypes.Add(new WarningType()
            {
                Id = 2,
                Name = "UK Coastal"
            });
            return warningTypes;
        }

    }
}
