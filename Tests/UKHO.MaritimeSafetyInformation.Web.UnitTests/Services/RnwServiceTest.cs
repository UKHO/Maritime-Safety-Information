using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
        public async Task WhenRequestToGetWarningTypes_ThenReturnListOfWarningType()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;

            A.CallTo(() => _fakeRnwRepository.GetWarningTypes()).Returns(new List<WarningType>() { new WarningType {Id=1, Name="test"}});

            List<WarningType> result = await _rnwService.GetWarningTypes();

            Assert.AreEqual("test", result[0].Name);
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
        public void WhenPostInvalidWarningTypeInRequest_ThenReturnException()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value recieved for parameter warningType"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidReferenceInRequest_ThenReturnException()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.Reference = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value recieved for parameter reference"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidSummaryInRequest_ThenReturnException()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value recieved for parameter summary"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidContentInRequest_ThenReturnException()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;
            _fakeRadioNavigationalWarnings.Content = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value recieved for parameter content"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }

        [Test]
        public void WhenPostValidRequestWithException_ThenReturnException()
        {
            DateTime _fakeDateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarnings.DateTimeGroup = _fakeDateTime;

            A.CallTo(() => _fakeRnwRepository.AddRadioNavigationWarnings(A<RadioNavigationalWarnings>.Ignored)).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarnings, CorrelationId); });
        }
    }
}
