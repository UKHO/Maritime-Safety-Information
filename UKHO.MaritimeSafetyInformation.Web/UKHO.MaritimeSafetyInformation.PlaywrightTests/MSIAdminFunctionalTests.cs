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
            if (IsRunningInPipeline())
            {
                _httpEndpoint = "https://rnwadmin.ukho.gov.uk/";
            }
            else
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
            
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            if (IsRunningInPipeline())
            {
                return; // No need to dispose in pipeline, as it is managed by the CI/CD environment
            }
            await _app.DisposeAsync();
        }

        [Test]
        public async Task RunningAppropriateTestEnvironment()
        {
            await Page.GotoAsync(_httpEndpoint);
            if (IsRunningInPipeline())
            {
                Assert.That(_httpEndpoint, Does.Contain("https://rnwadmin.ukho.gov.uk/"), "Running in CI/CD pipeline, expected endpoint to contain 'msi-dev.admiralty.co.uk'.");
            }
            else
            {
                //var expectedUrl = Page.Url;
                Assert.That(Page.Url, Does.Contain("localhost"), "Running locally, expected URL to contain 'localhost'.");
            }
        }


        private static bool IsRunningInPipeline()
        {
            // Common environment variables for CI/CD pipelines
            var ci = Environment.GetEnvironmentVariable("CI");
            var tfBuild = Environment.GetEnvironmentVariable("TF_BUILD");
            var githubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");
            var azurePipeline = Environment.GetEnvironmentVariable("AGENT_NAME");

            return !string.IsNullOrEmpty(ci)
                || !string.IsNullOrEmpty(tfBuild)
                || !string.IsNullOrEmpty(githubActions)
                || !string.IsNullOrEmpty(azurePipeline);
        }

    }
}
