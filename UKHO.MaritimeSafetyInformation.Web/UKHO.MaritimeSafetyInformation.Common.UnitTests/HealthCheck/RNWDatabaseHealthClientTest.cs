using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.HealthCheck
{
    [TestFixture]
    public class RNWDatabaseHealthClientTest
    {
        private RadioNavigationalWarningsContext _context;
        private RNWDatabaseHealthClient _rnwDatabaseHealthClient;

        [Test]
        public async Task WhenRNWDatabaseIsSetUp_ThenReturnsHealthy()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                        .UseInMemoryDatabase("msi-hc-ut-db");
            _context = new RadioNavigationalWarningsContext(builder.Options);

            _rnwDatabaseHealthClient = new RNWDatabaseHealthClient(_context);

            HealthCheckResult response = await _rnwDatabaseHealthClient.CheckHealthAsync();

            Assert.AreEqual(HealthStatus.Healthy, response.Status);
        }

        [Test]
        public async Task WhenRNWDatabaseIsNotSetUp_ThenReturnsUnHealthy()
        {
            _rnwDatabaseHealthClient = new RNWDatabaseHealthClient(_context);

            HealthCheckResult response = await _rnwDatabaseHealthClient.CheckHealthAsync();

            Assert.AreEqual(HealthStatus.Unhealthy, response.Status);
        }
    }
}
