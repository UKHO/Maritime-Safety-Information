using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal class RadioNavigationalWarningsListEndUserObject
    {
        private readonly IPage _page;
        public ILocator RadioNavigationalWarningsPage { get; }
        public ILocator RadioNavigationalWarningsEndUser { get; }
        public ILocator RadioWarningEndUser { get; }
        public ILocator AboutEndUser { get; }
        public ILocator AllWarningEndUser { get; }
        public ILocator NavAreaEndUser { get; }
        public ILocator UkCostalEnduser { get; }
        public ILocator TableHeader { get; }
        public ILocator ShowSelection { get; }
        public ILocator SelectCheckBox { get; }
        public ILocator BtnShowSelection { get; }
        public ILocator SelectAll { get; }
        public ILocator BackToAllWarning { get; }
        public ILocator Refrence { get; }
        public ILocator DateTimeGroupRnwFormat { get; }
        public ILocator DetailsReference { get; }
        public ILocator DetailsDateTimeGroupRnwFormat { get; }
        public ILocator Print { get; }
        public ILocator ViewDetails { get; }
        public ILocator DetailWarningType { get; }
        public ILocator About { get; }
        public ILocator AboutRNW { get; }
        public ILocator RadioNavigationalWarnings { get; }

        public readonly string[] TableHeaderText = { "Reference", "Date Time Group", "Description", "Select all", "Select" };

        public RadioNavigationalWarningsListEndUserObject(IPage page)
        {
            _page = page;
            RadioNavigationalWarningsPage = _page.Locator("a:has-text(\"Radio Navigation Warnings\")");
            RadioNavigationalWarnings = _page.Locator("#navbarSupportedContent > ul > li:nth-child(2) > a");
            RadioNavigationalWarningsEndUser = _page.Locator("#headingLevelOne");
            RadioWarningEndUser = _page.Locator("text=Radio Warnings");
            AboutEndUser = _page.Locator("text=About");
            AllWarningEndUser = _page.Locator("#allwarnings-tab");
            NavAreaEndUser = _page.Locator("#NAVAREA1-tab");
            UkCostalEnduser = _page.Locator("#ukcoastal-tab");
            ShowSelection = _page.Locator("#showSelectionId");
            SelectCheckBox = _page.Locator("[id^='checkbox'] > input");
            BtnShowSelection = _page.Locator("#BtnShowSelection");
            SelectAll = _page.Locator("#select_button");
            BackToAllWarning = _page.Locator("text=Back to all warnings");
            Refrence = _page.Locator("[id^=\"Reference\"]");
            DateTimeGroupRnwFormat = _page.Locator("[id^=\"DateTimeGroupRnwFormat\"]");
            DetailsReference = _page.Locator("[id^=\"Details_Reference\"]");
            DetailsDateTimeGroupRnwFormat = _page.Locator("[id^=\"Details_DateTimeGroupRnwFormat\"]");
            Print = _page.Locator("#Print");
            ViewDetails = _page.Locator("[id^=\"Viewdetails\"] > button > span.view_details");
            DetailWarningType = _page.Locator("[id^=\"Details_WarningType\"]");
            About = _page.Locator("a:has-text(\"IHO WWNWS-SC\")");
            AboutRNW = _page.Locator(" div >  p:nth-child(3)");
        }

        public async Task GoToRadioWarningAsync()
        {
            await RadioNavigationalWarnings.HighlightAsync();
            await RadioNavigationalWarnings.ClickAsync();
        }

        public async Task<string> CheckTextAsync(ILocator locator)
        {
            return await locator.InnerTextAsync();
        }

        public async Task VerifyTableDateColumnDataAsync()
        {
            var resultYear = await _page.Locator("[id^=\"DateTimeGroupRnwFormat\"]").AllInnerTextsAsync();
            Assert.That(resultYear.Count, Is.GreaterThan(0));

            var resultdate = (await _page.Locator("[id^=\"DateTimeGroupRnwFormat\"]").AllInnerTextsAsync())
                .Select(x => x.Trim().Substring(6)).ToList();

            //To do a date sort , we need to remove "UTC " and trim the string but also keep the original format for comparison
            var sortedDesc = resultdate
                .Select(x => x.Trim().Replace("UTC ", "")) // Remove "UTC " and trim
                .Select(x => DateTime.TryParseExact(x, "MMM yy", null, DateTimeStyles.None, out var dt)
                    ? new { Original = $" UTC {x}", Date = dt }
                    : null)
                .Where(x => x != null)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Original)
                .ToList();
            Assert.That(resultdate, Is.EqualTo(sortedDesc));
        }

        public async Task VerifyTableContainsViewDetailsLinkAsync()
        {
            var resultLinks = await _page.Locator("[id^=\"Viewdetails\"] > button > span.view_details").AllInnerTextsAsync();
            foreach (var link in resultLinks)
            {
                Assert.That(link.Trim(), Is.EqualTo("View details"));
            }
        }

        public async Task VerifyTableHeaderAsync()
        {
            var tableColsHeader = (await _page.Locator(".table>thead>tr>th").AllInnerTextsAsync()).Select(x => x.Trim()).ToList();
            var selectAllHeader = await SelectAll.InputValueAsync();
            tableColsHeader.Insert(3, selectAllHeader);
            tableColsHeader = tableColsHeader.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var match = TableHeaderText.Length == tableColsHeader.Count && TableHeaderText.SequenceEqual(tableColsHeader);
            Assert.That(match, Is.True);
        }

        public async Task VerifyTableViewDetailsUrlAsync()
        {
            var viewDetails = _page.Locator("[id^=\"Viewdetails\"] > button > span.view_details");
            var beforeReference = await _page.Locator("[id^=\"Reference\"]").First.InnerTextAsync();
            var beforeDatetime = await _page.Locator("[id^=\"DateTimeGroupRnwFormat\"]").First.InnerTextAsync();
            var beforeViewDetails = await _page.Locator("[id^=\"Viewdetails\"] > button").First.GetAttributeAsync("aria-expanded");
            Assert.That(beforeViewDetails, Is.EqualTo("false"));
            await viewDetails.First.ClickAsync(new LocatorClickOptions { Force = true });
            var newDetails = await _page.Locator("[id^=\"Viewdetails\"] > button").First.GetAttributeAsync("aria-expanded");
            Assert.That(newDetails, Is.Not.Null.And.Not.Empty);
            var afterReference = await _page.Locator("[id^=\"Details_Reference\"]").First.InnerTextAsync();
            var afterDateTime = await _page.Locator("[id^=\"Details_DateTimeGroupRnwFormat\"]").First.InnerTextAsync();
            Assert.That(beforeReference, Is.EqualTo(afterReference));
            Assert.That(beforeDatetime, Is.EqualTo(afterDateTime));
        }

        public async Task VerifyImportantBlockAsync()
        {
            var rnwHeader = (await _page.Locator("#rnwInfo > p").InnerTextAsync()).ToString();
            await ImportantBlockAsync(rnwHeader);
        }

        public async Task ImportantBlockAsync(string rnwHeader)
        {
            var rnwHeaderText = rnwHeader.Split(':');
            var rnwMessageText = rnwHeaderText[0];
            Assert.That(rnwMessageText, Does.Contain("NAVAREA 1 and UK Coastal"));

            var currentDateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var rnwDateTime = rnwHeaderText[1].Replace("UTC", "").Trim();

            //var rnwModifiedDateTime = DateTimeLuxon.FromFormat(rnwDateTime, "ddHHmm  MMM yy").ToDateTimeUtc();
            var rnwModifiedDateTime = DateTime.ParseExact(rnwDateTime, "ddHHmm  MMM yy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToUniversalTime();
            var lastModifiedDateTime = new DateTimeOffset(rnwModifiedDateTime).ToUnixTimeMilliseconds();

            Assert.That(lastModifiedDateTime < currentDateTime, Is.True);
        }

        public async Task VerifySelectOptionTextAsync()
        {
            Assert.That(await SelectAll.InputValueAsync(), Is.EqualTo("Select all"));
            var currentValue = await ShowSelection.GetAttributeAsync("value");
            Assert.That(currentValue, Is.EqualTo(string.Empty));

            await SelectAll.ClickAsync(new LocatorClickOptions { Force = true });
            Assert.That(await SelectCheckBox.First.IsCheckedAsync(), Is.True);
            currentValue = await ShowSelection.GetAttributeAsync("value");
            Assert.That(currentValue, Is.Not.Empty);

            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            Assert.That(await SelectAll.InputValueAsync(), Is.EqualTo("Clear all"));
            await SelectAll.ClickAsync(new LocatorClickOptions { Force = true });
            Assert.That(await SelectCheckBox.First.IsCheckedAsync(), Is.False);
            currentValue = await ShowSelection.GetAttributeAsync("value");
            Assert.That(currentValue, Is.EqualTo(string.Empty));

            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            Assert.That(await SelectAll.InputValueAsync(), Is.EqualTo("Select all"));
            await SelectAll.ClickAsync(new LocatorClickOptions { Force = true });
        }

        public async Task VerifySelectOptionCheckBoxAsync()
        {
            await SelectAll.ClickAsync(new LocatorClickOptions { Force = true });
            Assert.That(await SelectCheckBox.First.IsEnabledAsync(), Is.True);
            var detailsReference = await Refrence.First.InnerTextAsync();
            Assert.That(detailsReference.Length, Is.GreaterThan(0));
            var beforeDetailsReference = (await Refrence.First.InnerTextAsync()).Trim();
            var beforeDetailsDateTimeGroupRnwFormat = (await DateTimeGroupRnwFormat.First.InnerTextAsync()).Trim();
            await SelectCheckBox.First.ClickAsync();
            await BtnShowSelection.ClickAsync();
            var afterDetailsReference = (await DetailsReference.First.InnerTextAsync()).Trim();
            var afterDetailsDateTimeGroupRnwFormat = (await DetailsDateTimeGroupRnwFormat.First.InnerTextAsync()).Trim();
            Assert.That(beforeDetailsDateTimeGroupRnwFormat, Is.EqualTo(afterDetailsDateTimeGroupRnwFormat));
            Assert.That(beforeDetailsReference, Is.EqualTo(afterDetailsReference));
            await BackToAllWarning.ClickAsync();
        }

        //public async Task VerifyPrintAsync()
        //{
        //    await SelectCheckBox.First.ClickAsync();
        //    await BtnShowSelection.ClickAsync();
        //    await _page.WaitForLoadStateAsync();
        //    Assert.That(await Print.IsEnabledAsync(), Is.True);
        //    Assert.That((await Print.InnerTextAsync()).ToString(), Does.Contain("Print"));
        //    var msgPromise = _page.WaitForEventAsync(PageEvent.Console);
        //    await Print.ClickAsync();
        //    var msg = await msgPromise;
        //    Assert.That(msg.Text, Does.Contain("Print dialog is opened"));
        //    await _page.WaitForTimeoutAsync(5000);
        //}

        public async Task VerifyNavareaAndUkCostalFilterAsync(ILocator locator, string text, string appUrl)
        {
            await locator.ClickAsync();
            await ViewDetails.First.ClickAsync();
            var detailWarningType = await DetailWarningType.First.InnerTextAsync();
            Assert.That(detailWarningType, Does.Contain(text));
            var resultdate = (await _page.Locator("[id^=\"DateTimeGroupRnwFormat\"]").AllInnerTextsAsync())
                .Select(x => x.Trim().Substring(6)).ToList();
            var sortedDesc = resultdate.OrderByDescending(x => x).ToList();
            Assert.That(resultdate, Is.EqualTo(sortedDesc));
            var anchor = await locator.GetAttributeAsync("href");
            var urlName = new UriBuilder(appUrl) { Path = $"/RadioNavigationalWarnings"}.Uri.ToString();
            urlName += anchor;
            Assert.That(_page.Url, Is.EqualTo(urlName));
            //await _page.WaitForTimeoutAsync(5000);
        }

        public async Task VerifyAboutRnwAsync()
        {
            await AboutEndUser.ClickAsync();
            var href = await About.EvaluateAsync<string>("option => option.getAttribute('href')");
            Assert.That(href, Does.Contain("https://iho.int/navigation-warnings-on-the-web"));
        }

        public async Task VerifyAboutRNWImportantBlockAsync()
        {
            var rnwHeader = (await AboutRNW.Last.InnerTextAsync()).ToString();
            await ImportantBlockAsync(rnwHeader);
        }

    }
}
