using FakeItEasy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.HealthCheck
{
    [TestFixture]
    public class FileShareServiceHealthCheckTest
    {
        private ILogger<FileShareServiceHealthCheck> _fakeLogger;
        private IFileShareServiceHealthClient _fakeFileShareServiceHealthClient;
        private FileShareServiceHealthCheck _fileShareServiceHealthCheck;

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<FileShareServiceHealthCheck>>();
            _fakeFileShareServiceHealthClient = A.Fake<IFileShareServiceHealthClient>();

            _fileShareServiceHealthCheck = new FileShareServiceHealthCheck(_fakeFileShareServiceHealthClient, _fakeLogger);
        }

        [Test]
        public async Task WhenFSSConnectionIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeFileShareServiceHealthClient.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await _fileShareServiceHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenFSSConnectionIsUnHealthy_ThenReturnsUnHealthy()
        {
            A.CallTo(() => _fakeFileShareServiceHealthClient.CheckHealthAsync(A<CancellationToken>.Ignored)).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "File Share Service is unhealthy", new Exception("File Share Service is unhealthy")));

            HealthCheckResult response = await _fileShareServiceHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
