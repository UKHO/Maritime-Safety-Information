﻿using FakeItEasy;
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
        private ILogger<RnwRepository> _fakeLogger;

        [SetUp]
        public void SetUp()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                                .UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);
            _fakeLogger = A.Fake<ILogger<RnwRepository>>();

            _rnwRepository = new RnwRepository(_fakeContext, _fakeLogger);

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
            List<RadioNavigationalWarningsAdminList> result = await _rnwRepository.GetRadioNavigationWarningsAdminList(string.Empty);            
            Assert.IsTrue(result.Count == 4);
            Assert.AreEqual(4, result[0].Id);
            Assert.AreEqual("011200 UTC Jan 22", result[0].DateTimeGroupRnwFormat);
            Assert.AreEqual("UK Coastal", result[0].WarningTypeName);
        }

        [Test]
        public async Task WhenCallGetWarningTypes_ThenReturnListAsync()
        {
            List<WarningType> result = await _rnwRepository.GetWarningTypes();
            Assert.IsTrue(result.Count == 2);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("NAVAREA 1", result[0].Name);
            Assert.AreEqual("UK Coastal", result[1].Name);
        }

        [Test]
        public async Task WhenCallGetYears_ThenReturnListAsync()
        {
            List<string> result = await _rnwRepository.GetYears();
            Assert.IsTrue(result.Count == 3);
            Assert.AreEqual("2022", result[0]);
            Assert.AreEqual("2021", result[1]);
            Assert.AreEqual("2020", result[2]);
        }
    }
}