using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using OtpNet; // ensure reference for Totp and Base32 encoding in v1.4.0

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal class LoginPageObject
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
        // MFA specific locators (Azure AD TOTP prompt)
        public ILocator MfaOtpInput { get; }
        public ILocator MfaVerifyButton { get; }

        public LoginPageObject(IPage page)
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
            // Common Azure AD OTP input & continue button IDs (may vary depending on policy)
            MfaOtpInput = _page.Locator("#idTxtBx_SAOTCC_OTC");
            MfaVerifyButton = _page.Locator("#idSubmit_SAOTCC_Continue");
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
            await _page.WaitForLoadStateAsync(); // Ensure page loaded
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
                _page.WaitForNavigationAsync(),
                AdNext.ClickAsync()
            );
            await AdPassword.FillAsync(password);
            await Task.WhenAll(
                _page.WaitForNavigationAsync(),
                LoginButton.ClickAsync()
            );
        }

        public async Task AdLoginWithMfaAsync(string username, string password, string totpSecret)
        {
            // Perform basic AAD login first
            await AdLoginAsync(username, password);

            if (string.IsNullOrWhiteSpace(totpSecret))
            {
                return; // No MFA secret configured
            }

            // If MFA field appears, submit current TOTP
            try
            {
                if (await MfaOtpInput.IsVisibleAsync())
                {
                    var code = GenerateTotp(totpSecret);
                    await MfaOtpInput.FillAsync(code);
                    // Some tenants auto-verify after fill; attempt click if button present
                    if (await MfaVerifyButton.IsVisibleAsync())
                    {
                        await Task.WhenAll(
                            _page.WaitForNavigationAsync(new PageWaitForNavigationOptions { Timeout = 10000 }),
                            MfaVerifyButton.ClickAsync()
                        );
                    }
                }
            }
            catch (PlaywrightException)
            {
                // Ignore if elements not found; indicates MFA not required for this user/session
            }
        }

        private static string GenerateTotp(string secret)
        {
            // Allow otpauth:// URIs or raw Base32 secrets
            if (secret.StartsWith("otpauth://", StringComparison.OrdinalIgnoreCase))
            {
                var parts = secret.Split('?');
                if (parts.Length > 1)
                {
                    var query = parts[1].Split('&');
                    foreach (var kv in query)
                    {
                        var pair = kv.Split('=');
                        if (pair.Length == 2 && pair[0] == "secret")
                        {
                            secret = pair[1];
                            break;
                        }
                    }
                }
            }
            secret = secret.Replace(" ", "").Trim();
            // Uppercase for Base32 decoding
            var bytes = Base32Encoding.ToBytes(secret.ToUpperInvariant());
            var totp = new Totp(bytes); // v1.4.0 supports this
            return totp.ComputeTotp();
        }

        public async Task AdSignOutAsync()
        {
            await AdUserNameDropdown.ClickAsync();
            var text = await AdSignOutText.InnerTextAsync();
            Assert.That(text.Contains("Sign out"), Is.True, "Login was not successful, username not found on page.");
        }

        public async Task AdUnathorisedDetailsAsync()
        {
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
