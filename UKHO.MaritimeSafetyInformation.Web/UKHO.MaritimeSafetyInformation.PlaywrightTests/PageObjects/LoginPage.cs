using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal class LoginPage
    {
        private readonly IPage _page;
        public ILocator SignIn { get; }
        public ILocator Username { get; }
        public ILocator BtnContinue { get; }
        public ILocator Password { get; }
        public ILocator BtnLogin { get; }
        public ILocator LoginUsername { get; }
        public ILocator SignOutText { get; }
        public ILocator EmailError { get; }
        public ILocator AdUsername { get; }
        public ILocator AdNext { get; }
        public ILocator AdPassword { get; }
        public ILocator LoginButton { get; }
        public ILocator AdYes { get; }
        public ILocator AdUserNameDropdown { get; }
        public ILocator AdSignOutText { get; }
        public ILocator AdPasswordError { get; }
        public ILocator AdUnathorisedError { get; }

        public LoginPage(IPage page)
        {
            _page = page;
            SignIn = _page.Locator("a:has-text(\"Sign in\")");
            Username = _page.Locator("[placeholder=\"Email\"]");
            BtnContinue = _page.Locator("[aria-label=\"Continue\"]");
            Password = _page.Locator("[placeholder=\"Password\"]");
            BtnLogin = _page.Locator("button:has-text(\"Sign in\")");
            LoginUsername = _page.Locator("text=UKHOTest MSI");
            SignOutText = _page.Locator("text=Sign out");
            EmailError = _page.Locator("div.error.itemLevel.show");
            AdUsername = _page.Locator("[placeholder=\"someone\\@ukho\\.gov\\.uk\"]");
            AdNext = _page.Locator("text=Next");
            AdPassword = _page.Locator("[placeholder=\"Password\"]");
            LoginButton = _page.Locator("text=Sign in");
            AdYes = _page.Locator("#idSIButton9");
            AdUserNameDropdown = _page.Locator("#navbarDropdown");
            AdSignOutText = _page.Locator("#navbarSupportedContent > ul > li > ul > li > a");
            AdPasswordError = _page.Locator("#passwordError");
            AdUnathorisedError = _page.Locator("#headingLevelOne");
        }

        public async Task GoToSignInAsync()
        {
            await SignIn.ClickAsync();
        }

        public async Task LoginWithValidDetailsAsync(string username, string password)
        {
            await Username.ClickAsync();
            await Username.FillAsync(username);
            await BtnContinue.ClickAsync();
            await Password.FillAsync(password);
            await BtnLogin.ClickAsync();
            await LoginUsername.ClickAsync();
            var text = await LoginUsername.InnerTextAsync();
            Assert.That(text.Contains("UKHOTest MSI"), Is.True, "Login was not successful, username not found on page.");
        }

        public async Task LoginWithDistributorDetailsAsync(string username, string password)
        {
            await Username.ClickAsync();
            await Username.FillAsync(username);
            await BtnContinue.ClickAsync();
            await Password.FillAsync(password);
            await BtnLogin.ClickAsync();
        }

        public async Task SignOutAsync()
        {
            await _page.WaitForLoadStateAsync();
            await LoginUsername.ClickAsync();
            await SignOutText.ClickAsync();
            await _page.WaitForLoadStateAsync();
            var text = await SignIn.InnerTextAsync();
            Assert.That(text.Contains("Sign in"), Is.True, "Sign out was not successful, Sign in text not found on page.");
        }

        public async Task AdLoginAsync(string username, string password)
        {
            await AdUsername.FillAsync(username);
            await Task.WhenAll(
                //_page.WaitForURLAsync("**/signin.microsoftonline.com/*"),
                _page.WaitForNavigationAsync(),
                AdNext.ClickAsync()
            );
            await AdPassword.FillAsync(password);
            await Task.WhenAll(
                _page.WaitForNavigationAsync(),
                LoginButton.ClickAsync()
            );
        }

        public async Task AdSignOutAsync()
        {
            await AdUserNameDropdown.ClickAsync();
            var text = await AdSignOutText.InnerTextAsync();
            Assert.That(text.Contains("Sign out"), Is.True, "Login was not successful, username not found on page.");
        }

        public async Task AdUnathorisedDetailsAsync()
        {
            await _page.ScreenshotAsync(new() { Path = "a_rhz_AdUnathorisedError.png" });
            var text = await AdUnathorisedError.InnerTextAsync();
            Assert.That(text.Contains("Sorry, you do not have access to this website."), Is.True, "Login was not successful, username not found on page.");
        }

        public async Task AdPasswordErrorCheckAsync()
        {
            var text = await AdPasswordError.InnerTextAsync();
            Assert.That(text.Contains("Your account or password is incorrect"), Is.True, "Login was not successful, username not found on page.");
        }
    }

}
