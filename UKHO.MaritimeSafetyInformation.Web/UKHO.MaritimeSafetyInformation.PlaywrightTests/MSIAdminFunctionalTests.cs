using Aspire.Hosting;
using Microsoft.Playwright;
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
        private bool _isRunningInPipeline = IsRunningInPipeline();


        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            if (_isRunningInPipeline)
            {
                _httpEndpoint = "https://rnwadmin-dev.ukho.gov.uk/";
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
            if (_isRunningInPipeline)
            {
                return; // No need to dispose in pipeline, as it is managed by the CI/CD environment
            }
            await _app.DisposeAsync();
        }

        [SetUp]
        public async Task SetUpAsync()
        {
            // Navigate to the MSI Admin page before each test
            if (_isRunningInPipeline)
            {
                //await _login.GoToSignIn();
                //await _login.LoginWithDistributorDetails(_appConfig["DistributorTest_UserName"].ToString(), _appConfig["DistributorTest_Password"].ToString());
            }
            await Page.GotoAsync(_httpEndpoint);
        }

        [Test]
        public async Task AppropriateEnvironmentTest()
        {
            if (IsRunningInPipeline())
            {
                Assert.That(_httpEndpoint, Does.Contain("https://rnwadmin-dev.ukho.gov.uk/"), "Running in CI/CD pipeline, expected endpoint to contain 'msi-dev.admiralty.co.uk'.");
            }
            else
            {
                //var expectedUrl = Page.Url;
                Assert.That(Page.Url, Does.Contain("localhost"), "Running locally, expected URL to contain 'localhost'.");
            }
            await Task.CompletedTask;
        }

        [Test]
        public async Task DoesFilterDisplaySearchResultSortedInDescendingOrder()
        {
            var _rnwList = new RadioNavigationalWarningsListObject(Page);

            await _rnwList.SearchWithFilterAsync("UK Coastal", "2022");
            await _rnwList.VerifyTableHeaderAsync();
            await _rnwList.VerifyTableDateColumnDataAsync("2022");
        }

        [Test]
        public async Task DoesTheTableDataIsDisplayedWithPagination()
        {
            var _rnwList = new RadioNavigationalWarningsListObject(Page);

            await _rnwList.SearchWithFilterAsync("UK Coastal", "2022");
            await _rnwList.VerifyTableHeaderAsync();
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnFirst);
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnLast);
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnNext);
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnPrevious);
        }

        [Test]
        public async Task WarningTypeAndYearDropDownsAreEnabledAndHeaderTextsDisplayed()
        {
            var _rnwList = new RadioNavigationalWarningsListObject(Page);

            var warningTypeEnabled = await _rnwList.CheckEnabledWarningTypeDropDownAsync();
            Assert.That(warningTypeEnabled, Is.True);

            var yearEnabled = await _rnwList.CheckEnabledYearDropDownAsync();
            Assert.That(yearEnabled, Is.True);

            var createRecordList = await _rnwList.CheckCreateNewRecordTextAsync();
            Assert.That(string.IsNullOrWhiteSpace(createRecordList), Is.False);

            var pageHeader = await _rnwList.CheckPageHeaderTextAsync();
            Assert.That(pageHeader,Is.EqualTo("Radio Navigational Warnings Admin List"));
        }

        [Test]
        public async Task FilterDisplaysSearchResultsForWarningTypes()
        {
            var _rnwList = new RadioNavigationalWarningsListObject(Page);

            // search UK Coastal
            await _rnwList.SearchWithFilterAsync("UK Coastal", "2022");
            await _rnwList.VerifyTableHeaderAsync();
            await _rnwList.VerifyTableColumnWarningTypeDataAsync("UK Coastal");
            await _rnwList.VerifyTableContainsEditLinkAsync();

            // search NAVAREA 1
            await _rnwList.SearchWithFilterAsync("NAVAREA", "2022");
            await _rnwList.VerifyTableHeaderAsync();
            await _rnwList.VerifyTableColumnWarningTypeDataAsync("NAVAREA");
            await _rnwList.VerifyTableContainsEditLinkAsync();
        }


        [Test]
        public async Task WithValidInputCheckForDuplicateAndAccept()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.PageLoadAsync();
            await radioNavigationalWarnings.FillFormWithValidDetailsAsync("1", "testdata");
            await radioNavigationalWarnings.CreateRNWAsync();
            await radioNavigationalWarnings.ConfirmationBoxAsync(radioNavigationalWarnings.AlertMessage, radioNavigationalWarnings.Message, "yes");
            await radioNavigationalWarnings.GetDialogTextAsync("Record created successfully!");
        }

        [Test]
        public async Task WithValidInputCheckForDuplicateAndCancel()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.PageLoadAsync();
            await radioNavigationalWarnings.FillFormWithValidDetailsAsync("1", "testdata");
            await radioNavigationalWarnings.CreateRNWAsync();
            await radioNavigationalWarnings.ConfirmationBoxAsync(
                radioNavigationalWarnings.AlertMessage,
                radioNavigationalWarnings.Message,
                "cancel"
            );
            await radioNavigationalWarnings.CheckConfirmationBoxVisibleAsync(false);
        }

        [Test]
        public async Task WithValidInputCheckForDuplicateAndReject()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.PageLoadAsync();
            await radioNavigationalWarnings.FillFormWithValidDetailsAsync("1", "testdata");
            await radioNavigationalWarnings.CreateRNWAsync();
            await radioNavigationalWarnings.ConfirmationBoxAsync(
                radioNavigationalWarnings.AlertMessage,
                radioNavigationalWarnings.Message,
                "no"
            );
            await radioNavigationalWarnings.CheckConfirmationBoxVisibleAsync(false);
        }

        [Test]
        public async Task WithoutEnteredInputFields()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.PageLoadAsync();
            await radioNavigationalWarnings.CreateRNWAsync();
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.WarningError, "The Warning Type field is required.");
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.ReferenceError, "The Reference field is required.");
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.ContentError, "The Text field is required.");
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.DatetimeError, "The Date Time Group field is required.");
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.SummaryError, "The Description field is required.");
        }

        [Test]
        public async Task WithContentTextAsBlank()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.PageLoadAsync();
            await radioNavigationalWarnings.FillFormWithValidDetailsAsync("1", "");
            await radioNavigationalWarnings.CreateRNWAsync();
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.ContentError, "The Text field is required.");
        }

        [Test]
        public async Task WithValidInputDetailsWithNavarea()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.PageLoadAsync();
            await radioNavigationalWarnings.FillFormWithValidDetailsAsync("1", "testdata");
            await radioNavigationalWarnings.CreateRNWAsync();
            await radioNavigationalWarnings.GetDialogTextAsync("Record created successfully!");
        }

        [Test]
        public async Task WithValidInputDetailsWithUKCoastal()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.PageLoadAsync();
            await radioNavigationalWarnings.FillFormWithValidDetailsAsync("2", "testdata");
            await radioNavigationalWarnings.CreateRNWAsync();
            await radioNavigationalWarnings.GetDialogTextAsync("Record created successfully!");
        }
        
        [Test]
        public async Task WarningTypeAndYearDropDownsAreEnabledAndHeaderTextsAreDisplayed()
        {
            var _rnwList = new RadioNavigationalWarningsListObject(Page);

            Assert.That(await _rnwList.CheckEnabledWarningTypeDropDownAsync());
            Assert.That(await _rnwList.CheckEnabledYearDropDownAsync());
            var createRecordText = await _rnwList.CheckCreateNewRecordTextAsync();
            Assert.That(string.IsNullOrWhiteSpace(createRecordText), Is.False);
            Assert.That(await _rnwList.CheckPageHeaderTextAsync(), Is.EqualTo("Radio Navigational Warnings Admin List") );
        }

        [Test]
        public async Task FilterDisplaysSearchResultsSortedDescending()
        {
            var _rnwList = new RadioNavigationalWarningsListObject(Page);

            await _rnwList.SearchWithFilterAsync("UK Coastal", "2022");
            await _rnwList.VerifyTableHeaderAsync();
            await _rnwList.VerifyTableDateColumnDataAsync("2022");
        }

        [Test]
        public async Task TableDataIsDisplayedWithPagination()
        {
            var _rnwList = new RadioNavigationalWarningsListObject(Page);

            await _rnwList.SearchWithFilterAsync("UK Coastal", "2022");
            await _rnwList.VerifyTableHeaderAsync();
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnFirst);
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnLast);
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnNext);
            await _rnwList.CheckPaginationLinkAsync(_rnwList.BtnPrevious);
        }
        

        [Test]
        public async Task Update_WithSummaryReferenceAndContentTextAsBlank()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.GetEditUrlAsync();
            await radioNavigationalWarnings.ClearInputAsync(radioNavigationalWarnings.Description);
            await radioNavigationalWarnings.ClearInputAsync(radioNavigationalWarnings.Reference);
            await radioNavigationalWarnings.ClearInputAsync(radioNavigationalWarnings.Content);
            await radioNavigationalWarnings.EditRNWAsync();
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.SummaryError, "The Description field is required.");
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.ReferenceError, "The Reference field is required.");
            await radioNavigationalWarnings.CheckErrorMessageAsync(radioNavigationalWarnings.ContentError, "The Text field is required.");
        }

        [Test]
        public async Task Update_WithValidInputDetailsWithUKCoastal()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.SearchListWithFilterAsync("UK Coastal");
            await radioNavigationalWarnings.GetEditUrlAsync();
            await radioNavigationalWarnings.IsDeleteAsync();
            await radioNavigationalWarnings.FillEditFormWithValidDetailsAsync("testdata");
            await radioNavigationalWarnings.EditRNWAsync();
            await radioNavigationalWarnings.GetDialogTextAsync("Record updated successfully!");
        }

        [Test]
        public async Task Update_WithValidInputDetailsWithNAVAREA()
        {
            var radioNavigationalWarnings = new RadioNavigationalWarningsObject(Page);

            await radioNavigationalWarnings.SearchListWithFilterAsync("NAVAREA");
            await radioNavigationalWarnings.GetEditUrlAsync();
            await radioNavigationalWarnings.IsDeleteAsync();
            await radioNavigationalWarnings.FillEditFormWithValidDetailsAsync("testdata");
            await radioNavigationalWarnings.EditRNWAsync();
            await radioNavigationalWarnings.GetDialogTextAsync("Record updated successfully!");
        }

        //===

        [Test]
        [Ignore("Not working yet")]
        public async Task ShouldGotoNoticesToMarinerPageForWeeklyDownloadWithDistributorRole()
        {



            var notice = new NoticeToMarinersPageObject(Page);

            var noticeFileDownload = new NoticeToMarinersWeekDownloadPageObject(Page);

            


            //await _login.GoToSignIn();
            //await _login.LoginWithDistributorDetails(_appConfig["DistributorTest_UserName"].ToString(), _appConfig["DistributorTest_Password"].ToString());
            await noticeFileDownload.GoToNoticeToMarinerAsync();
            await noticeFileDownload.CheckWeeklyFileSectionNameAsync();
            await noticeFileDownload.CheckWeeklyFileSortingWithDistributorRoleAsync();
            var names = await noticeFileDownload.CheckFileDownloadAsync();
            Assert.That(names.Count > 0);
            var fileName = names[0];
            //var element = await Page.QuerySelectorAsync(noticeFileDownload.WeeklyDownloadSelector);
            //var newPageUrl = await element.GetAttributeAsync("href");
            //Assert.That(newPageUrl.Contains($"NoticesToMariners/DownloadFile?fileName={fileName}"));
        }

        [Test]
        [Ignore("Not working yet")]
        public async Task ShouldGotoNoticesToMarinerPageForWeeklyNMFilesWithDistributorRole()
        {

            var noticeFileDownload = new NoticeToMarinersWeekDownloadPageObject(Page);

            //await _login.GoToSignIn();
            //await _login.LoginWithDistributorDetails(_appConfig["DistributorTest_UserName"].ToString(), _appConfig["DistributorTest_Password"].ToString());
            await noticeFileDownload.GoToNoticeToMarinerAsync();
            await noticeFileDownload.VerifyDistributorFileCountAsync();
            await noticeFileDownload.VerifyIntegrationTestValueForDistributorAsync();
            await noticeFileDownload.VerifyIntegrationDownloadAllAsync();
            await noticeFileDownload.VerifyIntegrationDownloadPartnerAllAsync();
        }

        //===

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
