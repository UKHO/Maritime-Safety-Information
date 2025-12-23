using Microsoft.Playwright;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal class HomePageObject
    {
        private readonly IPage _page;
        public ILocator SiteTitle { get; }
        public ILocator HeadingLevelOne { get; }
        public ILocator Admiralty { get; }
        public ILocator NavigationalInformation { get; }
        public ILocator AdmiraltyDistributor { get; }
        public ILocator GotoNoticeMariners { get; }
        public ILocator GotoNavigationalWarnings { get; }
        public ILocator UkHydrographic { get; }
        public ILocator PrivacyPolicyLink { get; }
        public ILocator AccessibilityLink { get; }

        public HomePageObject(IPage page)
        {
            _page = page;
            SiteTitle = _page.Locator("#siteTitle");
            HeadingLevelOne = _page.Locator("h1:has-text(\"Maritime Safety Information\")");
            Admiralty = _page.Locator("img[alt=\"Admiralty Maritime Data Solutions Home\"]");
            NavigationalInformation = _page.Locator("text=View the latest safety-critical navigational information");
            AdmiraltyDistributor = _page.Locator("text=If you are an ADMIRALTY Distributor, sign in to see more content");
            GotoNoticeMariners = _page.Locator("text=Go to Notices to Mariners");
            GotoNavigationalWarnings = _page.Locator("text=Go to Radio Navigation Warnings");
            UkHydrographic = _page.Locator("img[alt=\"UK Hydrographic Office Logo\"]");
            PrivacyPolicyLink = _page.Locator("#privacypolicylink");
            AccessibilityLink = _page.Locator("#accessibilitylink");
        }

        public async Task VerifyAdmiraltyHomePageAsync()
        {
            var text = await SiteTitle.InnerTextAsync();
            Assert.That(text, Does.Contain("Maritime Safety Information"));
        }

        public async Task VerifyHomePageTitleAsync()
        {
            var text = await HeadingLevelOne.InnerTextAsync();
            Assert.That(text, Does.Contain("Maritime Safety Information"));
        }

        public async Task VerifyAdmiraltyAsync()
        {
            var src = await Admiralty.GetAttributeAsync("src");
            Assert.That(src, Does.Contain("/images/Admiralty-stacked-logo.svg"));
        }

        public async Task VerifyPageAsync()
        {
            var noticeText = await GotoNoticeMariners.InnerTextAsync();
            Assert.That(noticeText, Does.Contain("Go to Notices to Mariners"));

            var warningText = await GotoNavigationalWarnings.InnerTextAsync();
            Assert.That(warningText, Does.Contain("Go to Radio Navigation Warnings"));
        }

        public async Task VerifyUkHydrographicAsync()
        {
            var src = await UkHydrographic.GetAttributeAsync("src");
            Assert.That(src, Does.Contain("/images/UKHO-stacked-logo.svg"));
        }

        public async Task VerifyPrivacyPolicyAsync()
        {
            var text = await PrivacyPolicyLink.InnerTextAsync();
            Assert.That(text, Does.Contain("Privacy and cookies"));

            var href = await PrivacyPolicyLink.GetAttributeAsync("href");
            Assert.That(href, Does.Contain("https://www.admiralty.co.uk/cookie-policy"));
        }

        public async Task VerifyAccessibilityAsync()
        {
            var text = await AccessibilityLink.InnerTextAsync();
            Assert.That(text, Does.Contain("Accessibility"));

            var href = await AccessibilityLink.GetAttributeAsync("href");
            Assert.That(href, Does.Contain("AccessibilityStatement"));
        }
    }
}
