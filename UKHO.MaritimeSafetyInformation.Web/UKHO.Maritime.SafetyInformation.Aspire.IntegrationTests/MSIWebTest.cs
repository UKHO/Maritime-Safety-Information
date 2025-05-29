using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace UKHO.MaritimeSafetyInformation.Aspire.IntegrationTests
{
    public class MSIWebTest
    {
        private DistributedApplication _app;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            // This method is called once before any tests are run.
            // You can use it to set up any shared resources or configurations needed for the tests.
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UKHO_MaritimeSafetyInformation_Web_AppHost>();
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            _app = await appHost.BuildAsync();


        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            // This method is called once after all tests have run.
            // You can use it to clean up any shared resources or configurations.
            await _app.DisposeAsync();
        }

        [Test]
        public async Task GetWebResourceRootReturnsOkStatusCode()
        {
            // Arrange
            //var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UKHO_MaritimeSafetyInformation_Web_AppHost>();
            //appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            //{
            //    clientBuilder.AddStandardResilienceHandler();
            //});

            //await using var app = await appHost.BuildAsync();

            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
            await _app.StartAsync();

            // Act
            var httpClient = _app.CreateHttpClient("ukho-msi-web");
            await resourceNotificationService.WaitForResourceAsync("ukho-msi-web", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
            var response = await httpClient.GetAsync("/");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [Test]
        public async Task TestHealth()
        {
            // Arrange
            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
            await _app.StartAsync();

            var httpClient = _app.CreateHttpClient("ukho-msi-web");
            await resourceNotificationService.WaitForResourceAsync("ukho-msi-web", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
            var response = await httpClient.GetAsync("/health");
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
