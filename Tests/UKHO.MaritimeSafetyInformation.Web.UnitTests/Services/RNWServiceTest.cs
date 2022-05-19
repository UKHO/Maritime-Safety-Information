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
    public class RNWServiceTest
    {
        private ILogger<RNWService> _fakeLogger;
        private IRNWRepository _fakeRnwRepository;
        private RNWService _rnwService;
        private RadioNavigationalWarning _fakeRadioNavigationalWarning;
        public const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            _fakeRadioNavigationalWarning = new(){ WarningType = 1,
                                                    Reference = "test",
                                                    DateTimeGroup = DateTime.UtcNow,
                                                    Summary = "Test1",
                                                    Content = "test"};
            _fakeLogger = A.Fake<ILogger<RNWService>>();
            _fakeRnwRepository = A.Fake<IRNWRepository>();

            _rnwService = new RNWService(_fakeRnwRepository, _fakeLogger);
        }

        [Test]
        public async Task WhenRequestToGetWarningTypes_ThenReturnListOfWarningType()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            A.CallTo(() => _fakeRnwRepository.GetWarningTypes()).Returns(new List<WarningType>() { new WarningType {Id=1, Name="test"}});

            List<WarningType> result = await _rnwService.GetWarningTypes();

            Assert.AreEqual("test", result[0].Name);
        }

        [Test]
        public async Task WhenPostValidRequest_ThenReturnTrue()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            bool result = await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId);

            Assert.IsTrue(result);
        }

        [Test]
        public void WhenPostInvalidWarningTypeInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.WarningType = 3;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidReferenceInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.Reference = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidSummaryInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.Summary = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter summary"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostInvalidContentInRequest_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;
            _fakeRadioNavigationalWarning.Content = "";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter content"),
                             async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }

        [Test]
        public void WhenPostValidRequestWithException_ThenReturnException()
        {
            DateTime dateTime = DateTime.UtcNow;
            _fakeRadioNavigationalWarning.DateTimeGroup = dateTime;

            A.CallTo(() => _fakeRnwRepository.AddRadioNavigationWarning(A<RadioNavigationalWarning>.Ignored)).Throws(new Exception());

            Assert.ThrowsAsync(Is.TypeOf<Exception>(),
                               async delegate { await _rnwService.CreateNewRadioNavigationWarningsRecord(_fakeRadioNavigationalWarning, CorrelationId); });
        }
    }
}
