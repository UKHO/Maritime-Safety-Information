using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.HealthCheck
{
    [TestFixture]
    public class RNWDatabaseHealthCheckTest
    {
        private ILogger<RNWDatabaseHealthCheck> fakeLogger;
        private IRNWDatabaseHealthClient fakeRnwDatabaseHealthClient;
        private RNWDatabaseHealthCheck rnwDatabaseHealthCheck;

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<RNWDatabaseHealthCheck>>();
            fakeRnwDatabaseHealthClient = A.Fake<IRNWDatabaseHealthClient>();

            rnwDatabaseHealthCheck = new RNWDatabaseHealthCheck(fakeRnwDatabaseHealthClient, fakeLogger);
        }

        [Test]
        public async Task WhenRNWDatabaseIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => fakeRnwDatabaseHealthClient.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await rnwDatabaseHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(HealthStatus.Healthy, Is.EqualTo(response.Status));
        }

        [Test]
        public async Task WhenRNWDatabaseIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => fakeRnwDatabaseHealthClient.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Radio Navigational Warnings database is unhealthy", new Exception("Radio Navigational Warnings database is unhealthy")));

            HealthCheckResult response = await rnwDatabaseHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(HealthStatus.Unhealthy, Is.EqualTo(response.Status));
        }
    }
}
