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
    public class EventHubLoggingHealthCheckTest
    {
        private IEventHubLoggingHealthClient fakeEventHubLoggingHealthClient;
        private EventHubLoggingHealthCheck eventHubLoggingHealthCheck;
        private ILogger<EventHubLoggingHealthCheck> fakeLogger;

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<EventHubLoggingHealthCheck>>();
            fakeEventHubLoggingHealthClient = A.Fake<IEventHubLoggingHealthClient>();

            eventHubLoggingHealthCheck = new EventHubLoggingHealthCheck(fakeEventHubLoggingHealthClient, fakeLogger);
        }

        [Test]
        public async Task WhenEventHubLoggingIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => fakeEventHubLoggingHealthClient.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(HealthStatus.Healthy, Is.EqualTo(response.Status));
        }

        [Test]
        public async Task WhenEventHubLoggingIsUnhealthy_ThenReturnsUnhealthy()
        {
            A.CallTo(() => fakeEventHubLoggingHealthClient.CheckHealthAsync(A<HealthCheckContext>.Ignored, A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "Event hub is unhealthy", new Exception("Event hub is unhealthy")));

            HealthCheckResult response = await eventHubLoggingHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(HealthStatus.Unhealthy, Is.EqualTo(response.Status));
        }
    }
}
