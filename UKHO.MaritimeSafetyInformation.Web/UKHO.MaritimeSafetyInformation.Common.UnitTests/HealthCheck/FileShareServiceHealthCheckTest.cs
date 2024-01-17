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
    public class FileShareServiceHealthCheckTest
    {
        private ILogger<FileShareServiceHealthCheck> fakeLogger;
        private IFileShareServiceHealthClient fakeFileShareServiceHealthClient;
        private FileShareServiceHealthCheck fileShareServiceHealthCheck;

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<FileShareServiceHealthCheck>>();
            fakeFileShareServiceHealthClient = A.Fake<IFileShareServiceHealthClient>();

            fileShareServiceHealthCheck = new FileShareServiceHealthCheck(fakeFileShareServiceHealthClient, fakeLogger);
        }

        [Test]
        public async Task WhenFSSConnectionIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => fakeFileShareServiceHealthClient.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await fileShareServiceHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(HealthStatus.Healthy, Is.EqualTo(response.Status));
        }

        [Test]
        public async Task WhenFSSConnectionIsUnHealthy_ThenReturnsUnHealthy()
        {
            A.CallTo(() => fakeFileShareServiceHealthClient.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "File Share Service is unhealthy", new Exception("File Share Service is unhealthy")));

            HealthCheckResult response = await fileShareServiceHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.That(HealthStatus.Unhealthy, Is.EqualTo(response.Status));
        }
    }
}
