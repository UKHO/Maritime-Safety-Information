using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Json.More;
using Microsoft.Identity.Client;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
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

            _httpEndpoint = _app.GetEndpoint(_frontend,"https").ToString(); //this does not return https.
            
            //_httpEndpoint = "https://msi-dev.admiralty.co.uk/";
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

        //    _httpEndpoint = response.RequestMessage.RequestUri.ToString();
        //}


        [Test]
        public async Task LandingPageNavigationInPlace()
        {
            await Page.GotoAsync(_httpEndpoint);

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Admiralty Maritime Data" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Maritime Safety Information" })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Notices to Mariners", Exact = true })).ToBeVisibleAsync();
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Radio Navigation Warnings", Exact = true })).ToBeVisibleAsync();
            //await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" })).ToBeVisibleAsync();
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
            //await annualTab.VerifySectionWithDotsCountAsync(); // this seems to be checking the existance of the section with "---", why test for data?
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

        [Test]
        public async Task ShouldGotoNoticesToMarinerPageForWeeklyDownload()
        {
            await Page.GotoAsync(_httpEndpoint);


            await Page.ScreenshotAsync(new() { Path = "rhz_PageScreenshot1.png" });

            var noticeFileDownload = new NoticeToMarinersWeekDownloadPageObject(Page);

            await noticeFileDownload.GoToNoticeToMarinerAsync();

            await Page.ScreenshotAsync(new() { Path = "rhz_PageScreenshot2.png" });

            var names = await noticeFileDownload.CheckFileDownloadAsync();
            Assert.That(names.Count > 0,Is.True);
            var fileName = names[0];

            var element = await Page.QuerySelectorAsync(noticeFileDownload.WeeklyDownload);
            var newPageUrl = await element.GetAttributeAsync("href");
            var decodedUrl = WebUtility.UrlDecode(newPageUrl);
            Assert.That(decodedUrl, Does.Contain($"NoticesToMariners/DownloadFile?fileName={fileName}"));
        }


        [Test]
        public async Task DoesTheNoticesToMarinersCumulativePageIsDisplayed()
        {
            await Page.GotoAsync(_httpEndpoint);

            //await Page.GetByRole(AriaRole.Link, new() { Name = "Go to Notices to Mariners" }).ClickAsync();
            // Rhz need to change data for this to work. Filenames should be  NP234(A) 2024 or similar.
            var noticeFileDownload = new NoticeToMarinersWeekDownloadPageObject(Page);

            await noticeFileDownload.GoToNoticeToMarinerAsync();
            await noticeFileDownload.GoToCumulativeAsync();
            await noticeFileDownload.VerifyCumulativeFileNameAsync();
            await noticeFileDownload.VerifyCumulativeFileNameDownloadAsync();
        }

        [Test]
        public async Task DoesTheNoticesToMarinersPageUrlsAreDisplayedWithPageTitle()
        {
            await Page.GotoAsync(_httpEndpoint);

            var notice = new NoticeToMarinersPageObject(Page);
            // Rhz : These tests involve comparing the current url with the expected url in config, why?
            // Rhz : That comparison is currently commented out.
            await notice.CheckPageUrlAsync(_httpEndpoint, "Maritime Safety Information");
            await notice.CheckUrlAsync(notice.NoticeMarine.Nth(0), "NoticesToMariners/Weekly", "Notices to Mariners - Weekly");
            await notice.CheckUrlAsync(notice.TabDaily, "NoticesToMariners/Daily", "Notices to Mariners - Daily");
            await notice.CheckUrlAsync(notice.TabCumulative, "NoticesToMariners/Cumulative", "Notices to Mariners - Cumulative");
            await notice.CheckUrlAsync(notice.TabAnnual, "NoticesToMariners/Annual", "Notices to Mariners - Annual");
            await notice.CheckUrlAsync(notice.MenuValueAddedResellers, "NoticesToMariners/Resellers", "Notices to Mariners - Value Added Resellers");
            await notice.CheckUrlAsync(notice.MenuAbout, "NoticesToMariners/About", "About Notices to Mariners");
            await notice.CheckUrlAsync(notice.RadioNavigationalWarnings, "RadioNavigationalWarnings", "Radio Navigation Warnings");
            await notice.CheckNavareaUrlAsync(notice.NavareaTab, "RadioNavigationalWarnings", "Radio Navigation Warnings - NAVAREA I");
            await notice.CheckUkCoastalUrlAsync(notice.UkCoastalTab, "RadioNavigationalWarnings", "Radio Navigation Warnings - UK Coastal");
        }


        //Rnw
        [Test]
        public async Task TableDataForRadioNavigationalWarningsAboutPageIsDisplayed()
        {
            await Page.GotoAsync(_httpEndpoint);

            var _rnwListEndUser = new RadioNavigationalWarningsListEndUser(Page);

            await _rnwListEndUser.GoToRadioWarningAsync();

            await _rnwListEndUser.VerifyAboutRnwAsync();
            await _rnwListEndUser.VerifyAboutRNWImportantBlockAsync();
        }

        [Test]
        public async Task MenuTabsAndPageTextIsDisplayed()
        {
            await Page.GotoAsync(_httpEndpoint);
            var _rnwListEndUser = new RadioNavigationalWarningsListEndUser(Page);

            Assert.That(await _rnwListEndUser.CheckTextAsync(_rnwListEndUser.RadioNavigationalWarningsEndUser), Is.EqualTo("Radio Navigation Warnings"));
            Assert.That(await _rnwListEndUser.CheckTextAsync(_rnwListEndUser.RadioWarningEndUser), Is.EqualTo("Radio Warnings"));
            Assert.That(await _rnwListEndUser.CheckTextAsync(_rnwListEndUser.AboutEndUser), Is.EqualTo("About"));
            Assert.That(await _rnwListEndUser.CheckTextAsync(_rnwListEndUser.AllWarningEndUser), Is.EqualTo("All warnings"));
            Assert.That(await _rnwListEndUser.CheckTextAsync(_rnwListEndUser.NavAreaEndUser),Is.EqualTo("NAVAREA 1"));
            Assert.That(await _rnwListEndUser.CheckTextAsync(_rnwListEndUser.UkCostalEnduser), Is.EqualTo("UK Coastal"));
        }

        [Test]
        public async Task TableDataHeaderTextAndViewDetailsLinkWithDateSortingIsDisplayed()
        {
            await Page.GotoAsync(_httpEndpoint);
            var _rnwListEndUser = new RadioNavigationalWarningsListEndUser(Page);

            await _rnwListEndUser.VerifyTableHeaderAsync();
            await _rnwListEndUser.VerifyTableContainsViewDetailsLinkAsync();
            await _rnwListEndUser.VerifyTableDateColumnDataAsync();
            await _rnwListEndUser.VerifyTableViewDetailsUrlAsync();
            await _rnwListEndUser.VerifyImportantBlockAsync();
            await _rnwListEndUser.VerifySelectOptionTextAsync();
            await _rnwListEndUser.VerifySelectOptionCheckBoxAsync();
            //await _rnwListEndUser.VerifyPrint();
        }

        //[Test]
        //public async Task TableDataHasNavarea1DataWithDateSortingIsDisplayed()
        //{
        //    await Page.GotoAsync(_httpEndpoint);
        //    var _rnwListEndUser = new RadioNavigationalWarningsListEndUser(Page);

        //    await _rnwListEndUser.VerifyNavareaAndUkCostalFilterAsync(_rnwListEndUser.NavAreaEndUser, "NAVAREA 1");
        //}

        //[Test]
        //public async Task TableDataHasUkCoastalDataWithDateSortingIsDisplayed()
        //{
        //    await Page.GotoAsync(_httpEndpoint);
        //    var _rnwListEndUser = new RadioNavigationalWarningsListEndUser(Page);

        //    await _rnwListEndUser.VerifyNavareaAndUkCostalFilterAsync(_rnwListEndUser.UkCostalEnduser, "UK Coastal");
        //}



    }
}
