using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Legacy;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal class NoticeToMarinersPageObject
    {
        private readonly IPage _page;
        private readonly JObject _appConfig;

        public ILocator NoticeMarine => _page.Locator("a:has-text(\"Notices to Mariners\")");
        public ILocator RadioNavigationalWarnings => _page.Locator("a:has-text(\"Radio Navigation Warnings\")");
        public ILocator DropDownYearly => _page.Locator("#ddlYears");
        public ILocator DropDownWeekly => _page.Locator("#ddlWeeks");
        public ILocator FileName => _page.Locator("#weekly >> text=File Name");
        public ILocator FileSize => _page.Locator("#weekly >> text=File Size");
        public ILocator MenuNoticeToMarine => _page.Locator("#navbarSupportedContent >> text=Notices to Mariners");
        public ILocator MenuValueAddedResellers => _page.Locator("text=Value Added Resellers");
        public ILocator MenuAbout => _page.Locator("text=About");
        public ILocator TabWeekly => _page.Locator("#weekly-tab");
        public ILocator TabDaily => _page.Locator("#daily-tab");
        public ILocator TabCumulative => _page.Locator("#cumulative-tab");
        public ILocator TabAnnual => _page.Locator("#annual-tab");
        public ILocator NavareaTab => _page.Locator("#NAVAREA1-tab");
        public ILocator UkCoastalTab => _page.Locator("#ukcoastal-tab");

        public NoticeToMarinersPageObject(IPage page)
        {
            _page = page;
        }

        public async Task ClickToNoticeMarineAsync()
        {
            await NoticeMarine.First.ClickAsync();
        }

        public async Task ClickToNoticeMarineAboutAsync()
        {
            await MenuAbout.ClickAsync();
        }

        public async Task ClickToNoticeMarineAnnualAsync()
        {
            await TabAnnual.ClickAsync();
        }

        public async Task<bool> CheckEnabledYearDropDownAsync()
        {
            return await DropDownYearly.IsEnabledAsync();
        }

        public async Task<bool> CheckEnabledWeekDropDownAsync()
        {
            return await DropDownWeekly.IsEnabledAsync();
        }

        //public async Task CheckPageUrlAsync(string url, string title)
        //{
        //    var baseUrl = _appConfig["url"]?.ToString();
        //    Assert.That(_page.Url, Is.EqualTo(baseUrl));
        //    Assert.That(await _page.TitleAsync(), Is.EqualTo(title));
        //}

        //public async Task CheckUrlAsync(ILocator locator, string url, string title)
        //{
        //    var baseUrl = _appConfig["url"]?.ToString();
        //    await locator.ClickAsync();
        //    Assert.That(_page.Url, Is.EqualTo($"{baseUrl}/{url}"));
        //    Assert.That(await _page.TitleAsync(), Is.EqualTo(title));
        //}

        //public async Task CheckNavareaUrlAsync(ILocator locator, string url, string title)
        //{
        //    var baseUrl = _appConfig["url"]?.ToString();
        //    await locator.ClickAsync();
        //    Assert.That(_page.Url, Does.Contain($"{baseUrl}/{url}#navarea1"));
        //    Assert.That(await _page.TitleAsync(), Is.EqualTo(title));
        //}

        //public async Task CheckUkCoastalUrlAsync(ILocator locator, string url, string title)
        //{
        //    var baseUrl = _appConfig["url"]?.ToString();
        //    await locator.ClickAsync();
        //    Assert.That(_page.Url, Does.Contain($"{baseUrl}/{url}#ukcoastal"));
        //    Assert.That(await _page.TitleAsync(), Is.EqualTo(title));
        //}

        public async Task<string> CheckTextAsync(ILocator locator)
        {
            return (await locator.InnerTextAsync()).ToString();
        }

        public async Task<int> CheckTableRecordCountAsync()
        {
            var yearlyLength = (await _page.QuerySelectorAllAsync("#ddlYears option")).Count;
            await DropDownYearly.SelectOptionAsync(new SelectOptionValue { Index = 1 });
            var weekLength = (await _page.QuerySelectorAllAsync("#ddlWeeks option")).Count;
            await DropDownWeekly.SelectOptionAsync(new SelectOptionValue { Index = weekLength - 1 });

            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle); //Rhz

            var result = await _page.EvalOnSelectorAllAsync<string[]>("td[id^=filename]", "els => els.map(e => e.textContent)");
            return result.Length;
        }

        public async Task<string> CheckFileSizeTextAsync()
        {
            return (await FileSize.TextContentAsync()).ToString();
        }

        public async Task<string> CheckFileNameTextAsync()
        {
            return (await FileName.TextContentAsync()).ToString();
        }

        public async Task VerifyTableContainsDownloadLinkAsync()
        {
            var downloadLinks = await _page.EvalOnSelectorAllAsync<string[]>("td[id^=download] > a", "els => els.map(e => e.textContent.trim())");
            foreach (var link in downloadLinks)
            {
                Assert.That(link, Is.EqualTo("Download"));
            }
        }

        public async Task CheckFileNameSortAsync()
        {
            var yearlyLength = (await _page.QuerySelectorAllAsync("#ddlYears option")).Count;
            await DropDownYearly.SelectOptionAsync(new SelectOptionValue { Index = 1 });
            var weekLength = (await _page.QuerySelectorAllAsync("#ddlWeeks option")).Count;
            await DropDownWeekly.SelectOptionAsync(new SelectOptionValue { Index = weekLength - 1 });

            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle); //Rhz

            var fileNameData = await _page.EvalOnSelectorAllAsync<string[]>("td[id^=filename]", "els => els.map(e => e.textContent)");
            var beforeSortFilename = fileNameData.ToArray();
            var afterSortFileName = fileNameData.OrderBy(x => x).ToArray();
            Assert.That(afterSortFileName, Is.EqualTo(beforeSortFilename).AsCollection);
        }

        public async Task CheckFileSizeDataAsync()
        {
            await _page.WaitForLoadStateAsync();
            await _page.WaitForSelectorAsync("#ddlYears");
            var yearlyCount = (await _page.QuerySelectorAllAsync("#ddlYears option")).Count;

            for (var year = 1; year <= yearlyCount - 1; year++)
            {
                await DropDownYearly.SelectOptionAsync(new SelectOptionValue { Index = year });
                var weekCount = (await _page.QuerySelectorAllAsync("#ddlWeeks option")).Count;

                for (var week = 1; week <= 1; week++)
                {
                    await DropDownWeekly.SelectOptionAsync(new SelectOptionValue { Index = weekCount - 1 });

                    await _page.WaitForLoadStateAsync(LoadState.NetworkIdle); //Rhz
                    await _page.WaitForSelectorAsync("td[id^=filesize]"); //Rhz

                    var fileSizeData = await _page.EvalOnSelectorAllAsync<string[]>("td[id^=filesize]", "els => els.map(e => e.textContent)");
                    Assert.That(fileSizeData.Length, Is.GreaterThan(0));
                    Assert.That(await CheckFileNameTextAsync(), Is.EqualTo("File Name"));
                    Assert.That(await CheckFileSizeTextAsync(), Is.EqualTo("File Size"));
                    foreach (var tableCell in fileSizeData)
                    {
                        var fileData = tableCell.Trim().Split(' ');
                        var unit = fileData.Length > 1 ? fileData[1] : "";
                        bool boolFileSize = unit is "MB" or "KB" or "GB" or "Bytes";
                        Assert.That(boolFileSize, Is.True);   
                    }
                }
            }
        }
    }
}
