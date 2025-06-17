using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests
{
    internal class MSIFunctionalPipelineTests : PageTest
    {
        private IConfiguration _configuration;
        private string _httpEndpoint = string.Empty;
        private string _httpRnwEndpoint = string.Empty;
        private string _b2cAutoTest_UserName = string.Empty;
        private string _b2cAutoTest_Password = string.Empty;
        private string _distributorTest_UserName = string.Empty;
        private string _distributorTest_Password = string.Empty;
        private string _rnwAdminAutoTest_User = string.Empty;
        private string _rnwAdminAutoTest_Pass = string.Empty;
        private string _rnwAdminAutoTestNoAccess_User = string.Empty;
        private string _rnwAdminAutoTestNoAccess_Pass = string.Empty;


        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            var builder = new ConfigurationBuilder()
                        .AddUserSecrets<MSIFunctionalPipelineTests>()
                        .AddEnvironmentVariables();
            _configuration = builder.Build();

            _httpEndpoint = _configuration["url"] ?? "";
            _httpRnwEndpoint = _configuration["rnwAdminUrl"] ?? "";
            _b2cAutoTest_UserName = _configuration["B2CAutoTest_User"] ?? "";
            _b2cAutoTest_Password = _configuration["B2CAutoTest_Pass"] ?? "";
            _distributorTest_UserName = _configuration["DistributorTest_UserName"] ?? "";
            _distributorTest_Password = _configuration["DistributorTest_Password"] ?? "";
            _rnwAdminAutoTest_User = _configuration["RNWAdminAutoTest_User"] ?? "";
            _rnwAdminAutoTest_Pass = _configuration["RNWAdminAutoTest_Pass"] ?? "";
            _rnwAdminAutoTestNoAccess_User = _configuration["RNWAdminAutoTestNoAccess_User"] ?? "";
            _rnwAdminAutoTestNoAccess_Pass = _configuration["RNWAdminAutoTestNoAccess_Pass"] ?? "";


        }

        [Test]
        public async Task ShouldGotoNoticesToMarinerPageForDailyDownloadFileWithDistributorLogin()
        {
            // Rhz : THis test is not working because the daily tab is not working in Dev & QA.

            await Page.GotoAsync(_httpEndpoint);

            var login = new LoginPage(Page);
            var noticeFileDownload = new NoticeToMarinersWeekDownloadPageObject(Page);
            await login.GoToSignInAsync();
            await login.LoginWithDistributorDetailsAsync(_distributorTest_UserName, _distributorTest_Password);
            await noticeFileDownload.GoToNoticeToMarinerAsync();
            await noticeFileDownload.GoToDailyFileAsync();
            await noticeFileDownload.CheckForErrorPage();
            await noticeFileDownload.CheckDailyFileNameAsync();
            await noticeFileDownload.CheckDailyFileSizeAsync();
            await noticeFileDownload.CheckDailyFileDownloadAsync();
            await noticeFileDownload.CheckDailyWeekFileNameAsync();
        }


        [Test]
        public async Task Login_WithValidDetails_ShouldSignInAndSignOut()
        {
            await Page.GotoAsync(_httpEndpoint);
            var loginPage = new LoginPage(Page);

            await loginPage.GoToSignInAsync();

            await loginPage.LoginWithValidDetailsAsync(_b2cAutoTest_UserName, _b2cAutoTest_Password);
            await loginPage.SignOutAsync();
        }

        [Test]
        public async Task WithValidDetails()
        {
            await Page.GotoAsync(_httpRnwEndpoint);
            var loginPage = new LoginPage(Page);
            await loginPage.AdLoginAsync(_rnwAdminAutoTest_User, _rnwAdminAutoTest_Pass);
            await loginPage.AdSignOutAsync();
        }

        [Test]
        public async Task WithUnauthorisedDetails()
        {
            await Page.GotoAsync(_httpRnwEndpoint);
            var loginPage = new LoginPage(Page);
            await loginPage.AdLoginAsync(_rnwAdminAutoTestNoAccess_User, _rnwAdminAutoTestNoAccess_Pass);
            await loginPage.AdUnathorisedDetailsAsync();
        }

        [Test]
        public async Task WithInvalidDetails()
        {
            await Page.GotoAsync(_httpRnwEndpoint);
            var loginPage = new LoginPage(Page);
            await loginPage.AdLoginAsync(_rnwAdminAutoTest_User, "1111111");
            await loginPage.AdPasswordErrorCheckAsync();
        }

       
    }
}
