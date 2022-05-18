using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RnwServiceTest
    {
        private RnwService _rnwService;
        private IRnwRepository _fakeRnwRepository;
        private ILogger<RnwService> _fakeLogger;
        private IOptions<RadioNavigationalWarningConfiguration> _fakeRadioNavigationalWarningConfiguration;

        [SetUp]
        public void SetUp()
        {
            _fakeLogger = A.Fake<ILogger<RnwService>>();
            _fakeRadioNavigationalWarningConfiguration = A.Fake<IOptions<RadioNavigationalWarningConfiguration>>();
            _fakeRnwRepository = A.Fake<IRnwRepository>();
            _fakeRadioNavigationalWarningConfiguration.Value.AdminListRecordPerPage = 3;

            _rnwService = new RnwService(_fakeRnwRepository, _fakeRadioNavigationalWarningConfiguration, _fakeLogger);

        }

        [Test]
        public void WhenCallGetRadioNavigationalWarningsData_ThenReturnException()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationalWarningsDataList()).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.GetRadioNavigationalWarningsData(string.Empty); });
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnListAsync()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationalWarningsDataList()).Returns(new List<RadioNavigationalWarningsData>() { new RadioNavigationalWarningsData() { Description ="Test"} });

            List<RadioNavigationalWarningsData> result = await _rnwService.GetRadioNavigationalWarningsData(string.Empty);

            Assert.IsTrue(result.Count > 0);
        }
    }
}
