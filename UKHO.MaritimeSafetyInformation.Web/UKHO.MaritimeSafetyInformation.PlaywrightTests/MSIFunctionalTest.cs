using Aspire.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests
{
    public class MSIFunctionalTest : PageTest
    {
        private DistributedApplication _app;
        private const string _frontend = "ukho-msi-web";

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


        [Test]
        public async Task LandingPageNavigationInPlace()
        {
            // Arrange
            var httpEndpoint = _app.GetEndpoint(_frontend).ToString();

            // Act
            await Page.GotoAsync(httpEndpoint);

            // Assert
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Admiralty Maritime Data" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Maritime Safety Information" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Notices to Mariners", Exact = true })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Radio Navigation Warnings", Exact = true })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" })).ToBeVisibleAsync();
        }

        [Test]
        public async Task HomePageBodyIsValid()
        {
            // Arrange
            var httpEndpoint = _app.GetEndpoint(_frontend).ToString();
            // Act
            await Page.GotoAsync(httpEndpoint);
            // Assert
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Maritime Safety Information" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Go to Radio Navigation" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Privacy and cookies opens in" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Accessibility opens in new tab" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "UK Hydrographic Office Logo" })).ToBeVisibleAsync();
        }

        [Test]
        public async Task HomePageTest()
        {
            // Arrange
            var httpEndpoint = _app.GetEndpoint(_frontend).ToString();
            // Act
            await Page.GotoAsync(httpEndpoint);

            var home = new HomePageObject(Page);
            await home.VerifyAdmiraltyHomePageAsync();
            await home.VerifyAdmiraltyAsync();
            await home.VerifyHomePageTitleAsync();
            await home.VerifyPageAsync();
            await home.VerifyUkHydrographicAsync();
            await home.VerifyPrivacyPolicyAsync();
            await home.VerifyAccessibilityAsync();
        }
    }
}
