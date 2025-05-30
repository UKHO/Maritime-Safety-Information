using Aspire.Hosting;
using Microsoft.Playwright.NUnit;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests
{
    public class MSIAdminFunctionalTests : PageTest
    {
        private DistributedApplication _app;
        private const string _frontend = "ukho-msi-admin-web";


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

            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
            await _app.StartAsync();
            await resourceNotificationService.WaitForResourceAsync(_frontend, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            // This method is called once after all tests have run.
            // You can use it to clean up any shared resources or configurations.
            await _app.DisposeAsync();
        }

        [Test]
        public async Task CanGetToLandingPage()
        {
            // Arrange
            var httpClient = _app.CreateHttpClient(_frontend);

            // Act
            var response = await httpClient.GetAsync("/");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
