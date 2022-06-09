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
        public async Task WhenFssConnectionIsHealthy_ThenReturnsHealthy()
        {
            A.CallTo(() => _fakeFileShareServiceHealthClient.CheckHealthAsync()).Returns(new HealthCheckResult(HealthStatus.Healthy));

            HealthCheckResult response = await _fileShareServiceHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenFssConnectionIsUnHealthy_ThenReturnsUnHealthy()
        {
            A.CallTo(() => _fakeFileShareServiceHealthClient.CheckHealthAsync()).Returns(new HealthCheckResult(HealthStatus.Unhealthy, "File share service is unhealthy", new Exception("File share service is unhealthy")));

            HealthCheckResult response = await _fileShareServiceHealthCheck.CheckHealthAsync(new HealthCheckContext());

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
