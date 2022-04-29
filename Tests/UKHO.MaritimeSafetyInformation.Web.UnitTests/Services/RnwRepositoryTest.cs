using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RnwRepositoryTest
    {
        private RnwRepository _rnwRepository;
        private RadioNavigationalWarningsContext _fakeContext;

        [SetUp]
        public void SetUp()
        {
            var builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>().UseInMemoryDatabase("msi-ut-db");

            _fakeContext = new RadioNavigationalWarningsContext(builder.Options);

            _rnwRepository = new RnwRepository(_fakeContext);
        }

        [Test]
        public void WhenCallAddRadioNavigationWarningsMethod_ThenCreatedNewRNWRecord()
        {
            RadioNavigationalWarnings radioNavigationalWarnings = new() { WarningType = 1,
                                                                          Reference = "test",
                                                                          DateTimeGroup = DateTime.UtcNow ,
                                                                          Summary = "Test1",
                                                                          Content="test"};

            Task result = _rnwRepository.AddRadioNavigationWarnings(radioNavigationalWarnings);

            Task<RadioNavigationalWarnings> data = _fakeContext.RadioNavigationalWarnings.SingleOrDefaultAsync(b => b.Summary == "Test1");

            Assert.IsTrue(result.IsCompleted);
            Assert.IsNotNull(data.Result.Summary);
        }

    ////    [Test]
    ////    public void WhenCallGetWarningTypeMethod_ThenReturnWarningType()
    ////    {
    ////        WarningType warningType = new() { Id =1 ,Name = "test"};

    ////        _fakeContext.WarningType.Add(warningType);

    ////        List<WarningType> warningTypeList = _rnwRepository.GetWarningType();

    ////        Assert.AreEqual(warningType.Name, warningTypeList[0].Name);
    ////    }
    }
}
