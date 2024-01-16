using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;

namespace UKHO.MaritimeSafetyInformation.Common.UnitTests.HealthCheck
{
    [TestFixture]
    public class RNWDatabaseHealthClientTest
    {
        private RadioNavigationalWarningsContext context;
        private RNWDatabaseHealthClient rnwDatabaseHealthClient;

        [Test]
        public async Task WhenRNWDatabaseIsSetUp_ThenReturnsHealthy()
        {
            DbContextOptionsBuilder<RadioNavigationalWarningsContext> builder = new DbContextOptionsBuilder<RadioNavigationalWarningsContext>()
                                                        .UseInMemoryDatabase("msi-hc-ut-db");
            context = new RadioNavigationalWarningsContext(builder.Options);

            rnwDatabaseHealthClient = new RNWDatabaseHealthClient(context);

            HealthCheckResult response = await rnwDatabaseHealthClient.CheckHealthAsync(CancellationToken.None);

            Assert.That(HealthStatus.Healthy, Is.EqualTo(response.Status));
        }

        [Test]
        public async Task WhenRNWDatabaseIsNotSetUp_ThenReturnsUnHealthy()
        {
            rnwDatabaseHealthClient = new RNWDatabaseHealthClient(context);

            HealthCheckResult response = await rnwDatabaseHealthClient.CheckHealthAsync(CancellationToken.None);

            Assert.That(HealthStatus.Unhealthy, Is.EqualTo(response.Status));
        }
    }
}
