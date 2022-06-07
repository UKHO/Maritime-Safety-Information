using FakeItEasy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.HealthCheck
{
    [TestFixture]
    public class RNWDatabaseHealthCheckTest
    {     
        private ILogger<RNWDatabaseHealthCheck> _fakeLogger;
        private IRNWDatabaseHealthClient _fakeRnwDatabaseHealthClient;
        private RNWDatabaseHealthCheck _rnwDatabaseHealthCheck;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RNWDatabaseHealthCheck>>();
            _fakeRnwDatabaseHealthClient = A.Fake<IRNWDatabaseHealthClient>();

            _rnwDatabaseHealthCheck = new RNWDatabaseHealthCheck(_fakeRnwDatabaseHealthClient, _fakeLogger);
        }

        [Test]
        public async Task WhenRNWDatabaseIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeRnwDatabaseHealthClient.CheckHealthAsync()).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await _rnwDatabaseHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenRNWDatabaseIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => _fakeRnwDatabaseHealthClient.CheckHealthAsync()).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Radio navigational Warning database is unhealthy", new Exception("Radio navigational Warning database is unhealthy")));

            HealthCheckResult response = await _rnwDatabaseHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
