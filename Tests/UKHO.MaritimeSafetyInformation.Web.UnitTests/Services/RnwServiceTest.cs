using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class RnwServiceTest
    {
        private ILogger<RnwService> _fakeLogger;
        private IRnwRepository _fakeRnwRepository;
        private RnwService _rnwService;
        private RadioNavigationalWarnings _fakeRadioNavigationalWarnings;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeRadioNavigationalWarnings = new(){ WarningType = 1,
                                                    Reference = "test",
                                                    DateTimeGroup = DateTime.UtcNow,
                                                    Summary = "Test1",
                                                    Content = "test"};
            _fakeLogger = A.Fake<ILogger<RnwService>>();
            _fakeRnwRepository = A.Fake<IRnwRepository>();

            _rnwService = new RnwService(_fakeRnwRepository, _fakeLogger);
        }

        [Test]
        public async Task WhenPostValidRequest_ThenReturnTrue()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;

            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task WhenPostInvalidRequest_ThenReturnFalse()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.Reference = "";

            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task WhenPostValidRequestWithException_ThenReturnFalse()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;

            A.CallTo(() => _fakeRnwRepository.AddRadioNavigationWarnings(A<RadioNavigationalWarnings>.Ignored)).Throws(new Exception());

            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId);

            Assert.IsFalse(result);
        }
    }
}
