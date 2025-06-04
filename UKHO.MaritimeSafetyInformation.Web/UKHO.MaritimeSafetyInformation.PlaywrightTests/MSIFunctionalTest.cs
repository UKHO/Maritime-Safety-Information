using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects;
using static System.Net.WebRequestMethods;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class MSIFunctionalTest : PageTest
    {
        private DistributedApplication _app;
        private const string _frontend = "ukho-msi-web";
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
            //_httpEndpoint = "https://msi.admiralty.co.uk/";
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            await _app.DisposeAsync();
        }

        //[Test]
        //public async Task CanGetToLandingPage()
        //{
        //    // Arrange
        //    var httpClient = _app.CreateHttpClient(_frontend);

        //    // Act
        //    var response = await httpClient.GetAsync("/");

        //    // Assert
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //}


        [Test]
        public async Task LandingPageNavigationInPlace()
        {
            await Page.GotoAsync(_httpEndpoint);

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Admiralty Maritime Data" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Maritime Safety Information" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Notices to Mariners", Exact = true })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Radio Navigation Warnings", Exact = true })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" })).ToBeVisibleAsync();
        }

        [Test]
        public async Task HomePageBodyIsValidAlternative()
        {
            await Page.GotoAsync(_httpEndpoint);
            
            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Maritime Safety Information" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Go to Radio Navigation" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Privacy and cookies opens in" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Accessibility opens in new tab" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "UK Hydrographic Office Logo" })).ToBeVisibleAsync();
        }

        [Test]
        public async Task HomePageBodyIsValid()
        {
            await Page.GotoAsync(_httpEndpoint);

            var home = new HomePageObject(Page);
            await home.VerifyAdmiraltyHomePageAsync();
            await home.VerifyAdmiraltyAsync();
            await home.VerifyHomePageTitleAsync();
            await home.VerifyPageAsync();
            await home.VerifyUkHydrographicAsync();
            await home.VerifyPrivacyPolicyAsync();
            await home.VerifyAccessibilityAsync();
        }

        [Test]
        public async Task YearlyAndWeeklyDropDownsEnabled_MenuAndTabsTextDisplayed()
        {
            await Page.GotoAsync(_httpEndpoint);

            await Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" }).ClickAsync();

            var notice = new NoticeToMarinersPageObject(Page);

            Assert.That(await notice.CheckEnabledYearDropDownAsync(), Is.True);
            Assert.That(await notice.CheckEnabledWeekDropDownAsync(), Is.True);
            Assert.That(await notice.CheckTextAsync(notice.MenuNoticeToMarine), Is.EqualTo("Notices to Mariners"));
            Assert.That(await notice.CheckTextAsync(notice.MenuValueAddedResellers),Is.EqualTo("Value Added Resellers") );
            Assert.That(await notice.CheckTextAsync(notice.MenuAbout), Is.EqualTo("About"));
            Assert.That(await notice.CheckTextAsync(notice.TabWeekly), Is.EqualTo("Weekly"));
            Assert.That(await notice.CheckTextAsync(notice.TabDaily), Is.EqualTo("Daily"));
            Assert.That(await notice.CheckTextAsync(notice.TabCumulative), Is.EqualTo("Cumulative"));
            Assert.That(await notice.CheckTextAsync(notice.TabAnnual), Is.EqualTo("Annual"));
        }

        [Test]
        public async Task TableDataDisplayedWithRecordCount()
        {
            
            await Page.GotoAsync(_httpEndpoint);

            await Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" }).ClickAsync();

            var notice = new NoticeToMarinersPageObject(Page);

            Assert.That(await notice.CheckTableRecordCountAsync(), Is.GreaterThan(0), "Table record count should be greater than 0");
            await notice.VerifyTableContainsDownloadLinkAsync();
        }

        [Test]
        public async Task TableDataForYearlyAndWeeklyDropDown_IncludeTableDataFileNameAndFileSize()
        {
            
            await Page.GotoAsync(_httpEndpoint);

            await Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" }).ClickAsync();

            var notice = new NoticeToMarinersPageObject(Page);

            await notice.CheckFileSizeDataAsync();
            await notice.CheckFileNameSortAsync();
        }

        [Test]
        public async Task TableDataForAnnualIncludesSectionFileNameFileSizeAndDownload()
        {
            await Page.GotoAsync(_httpEndpoint);

            await Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" }).ClickAsync();

            var annual = new NoticeToMarinersPageObject(Page);

            var annualTab = new NoticeToMarinersWeekDownloadPageObject(Page);

            await annual.ClickToNoticeMarineAnnualAsync();
            await annualTab.VerifySectionWithDotsCountAsync();
            await annualTab.VerifyAnnualFileNameLinkAsync();
            await annualTab.VerifyAnnualDownloadLinkAsync();
            await annualTab.CheckAnnualFileSizeAsync();
        }

        [Test]
        public async Task ShouldGotoNoticesToMarinerPageForDailyDownloadFile()
        {
            await Page.GotoAsync(_httpEndpoint);

            await Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" }).ClickAsync();
            var noticeFileDownload = new NoticeToMarinersWeekDownloadPageObject(Page);

            await noticeFileDownload.GoToNoticeToMarinerAsync();
            await noticeFileDownload.GoToDailyFileAsync();
            await noticeFileDownload.CheckDailyFileDownloadAsync();
        }

        //[Test]
        //public async Task ShouldGotoNoticesToMarinerPageForDailyDownloadFileWithDistributorLogin()
        //{
        //    await Page.GotoAsync(_httpEndpoint);

        //    await Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" }).ClickAsync();

        //    await login.GoToSignIn();
        //    await login.LoginWithDistributorDetails(app.DistributorTest_UserName, app.DistributorTest_Password);
        //    await noticeFileDownload.GoToNoticeToMariner();
        //    await noticeFileDownload.GoToDailyFile();
        //    await noticeFileDownload.CheckDailyFileName();
        //    await noticeFileDownload.CheckDailyFileSize();
        //    await noticeFileDownload.CheckDailyFileDownload();
        //    await noticeFileDownload.CheckDailyWeekFileName();
        //}


    }
}
