using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common;
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

        [SetUp]
        public void SetUp()
        {
            _fakeRadioNavigationalWarning = new(){ WarningType = 1,
                                                    Reference = "test",
                                                    DateTimeGroup = DateTime.UtcNow,
                                                    Summary = "Test1",
                                                    Content = "test" };
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                                                .UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);

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
    }
}
