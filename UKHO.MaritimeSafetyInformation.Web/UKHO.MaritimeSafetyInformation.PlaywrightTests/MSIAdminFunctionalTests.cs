using Aspire.Hosting;
using Microsoft.Playwright.NUnit;
using UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class MSIAdminFunctionalTests : PageTest
    {
        private DistributedApplication _app;
        private const string _frontend = "ukho-msi-admin-web";
        private string _httpEndpoint = string.Empty;


        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.UKHO_MaritimeSafetyInformation_Web_AppHost>();
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            _app = await appHost.BuildAsync();

            var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
            await _app.StartAsync();
            await resourceNotificationService.WaitForResourceAsync(_frontend, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));

            _httpEndpoint = _app.GetEndpoint(_frontend).ToString();
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
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
