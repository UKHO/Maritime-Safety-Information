﻿using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Services;
using radioNavigationalWarningDto = UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarning
{
    public class BaseRNWTest
    {
        public readonly RadioNavigationalWarningsContext _fakeContext;
        public IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;
        public IHttpContextAccessor _fakeHttpContextAccessor;
        public ILogger<RNWService> _fakeLoggerRnwService;

        public BaseRNWTest()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                    .UseInMemoryDatabase("msi-in-db");
            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 20;
        }

        public async Task SeedRadioNavigationalWarnings(List<radioNavigationalWarningDto.RadioNavigationalWarning> radioNavigationalWarning)
        {
            _fakeContext.RadioNavigationalWarnings.AddRange(radioNavigationalWarning);
            await _fakeContext.SaveChangesAsync();
        }

        public async Task SeedWarningType(List<radioNavigationalWarningDto.WarningType> warningTypes)
        {
            _fakeContext.WarningType.AddRange(warningTypes);
            await _fakeContext.SaveChangesAsync();
        }

        public async Task DeSeedRadioNavigationalWarnings()
        {
            DbSet<radioNavigationalWarningDto.RadioNavigationalWarning> warnings = _fakeContext.RadioNavigationalWarnings;
            _fakeContext.RadioNavigationalWarnings.RemoveRange(warnings);
            await _fakeContext.SaveChangesAsync();
        }

        public async Task DeSeedWarningType()
        {
            DbSet<radioNavigationalWarningDto.WarningType> warningType = _fakeContext.WarningType;
            _fakeContext.WarningType.RemoveRange(warningType);
            await _fakeContext.SaveChangesAsync();
        }

        public static List<radioNavigationalWarningDto.RadioNavigationalWarning> GetFakeRadioNavigationalWarnings()
        {
            List<radioNavigationalWarningDto.RadioNavigationalWarning> radioNavigationalWarningList = new();

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                IsDeleted = true,
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2020, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2019, 1, 1),

            });

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2021, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2022, 1, 1),
                Summary = "RnwAdminListSummary",
                Content = "RnwAdminListContent",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.NAVAREA_1,
                Reference = "RnwAdminListReferance",
                DateTimeGroup = new DateTime(2023, 1, 1),
                Summary = "NORTHEAST ATLANTIC. Outer Hebrides Westwards. Live weapons firing in progress.",
                Content = "ENGLAND, EAST COAST.   Holy Island Eastwards.   1. Plough Seat light buoy, 55-40.4N 001-45.0W, unlit.  2. Cancel WZ 224.",
                ExpiryDate = new DateTime(2099, 1, 1),
            });

            radioNavigationalWarningList.Add(new radioNavigationalWarningDto.RadioNavigationalWarning()
            {
                WarningType = WarningTypes.UK_Coastal,
                Reference = "RnwAdminListReferance",
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

        public static List<radioNavigationalWarningDto.WarningType> GetFakeWarningTypes()
        {
            List<radioNavigationalWarningDto.WarningType> warningTypes = new();
            warningTypes.Add(new radioNavigationalWarningDto.WarningType()
            {
                Id = 1,
                Name = "NAVAREA 1"
            });

            warningTypes.Add(new radioNavigationalWarningDto.WarningType()
            {
                Id = 2,
                Name = "UK Coastal"
            });
            return warningTypes;
        }

    }
}