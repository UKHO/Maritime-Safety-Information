using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal class RadioNavigationalWarningsListObject
    {
        private readonly IPage _page;
        public ILocator WarningType { get; }
        public ILocator Year { get; }
        public ILocator Filter { get; }
        public ILocator CreateNewRecordText { get; }
        public ILocator BtnFirst { get; }
        public ILocator BtnPrevious { get; }
        public ILocator BtnNext { get; }
        public ILocator BtnLast { get; }
        public ILocator HeaderLocator { get; }
        public ILocator TableHeader { get; }
        public readonly List<string> TableHeaderText = new()
        {
            "Sr No.","Warning Type","Reference","Date/Time","Description","Text","Expiry Date","Status","Action"
        };

        public RadioNavigationalWarningsListObject(IPage page)
        {
            _page = page;
            WarningType = _page.Locator("#WarningType");
            Year = _page.Locator("#Year");
            Filter = _page.Locator("#BtnFilter");
            CreateNewRecordText = _page.Locator("#BtnCreate");
            BtnFirst = _page.Locator("#BtnFirst");
            BtnPrevious = _page.Locator("#BtnPrevious");
            BtnNext = _page.Locator("#BtnNext");
            BtnLast = _page.Locator("#BtnLast");
            HeaderLocator = _page.Locator(".container-fluid >h1");
            TableHeader = _page.Locator(".table>thead>tr>th");  
            
        }

        public async Task<bool> CheckEnabledWarningTypeDropDownAsync()
        {
            return await WarningType.IsEnabledAsync();
        }

        public async Task<bool> CheckEnabledYearDropDownAsync()
        {
            return await Year.IsEnabledAsync();
        }

        public async Task<string> CheckCreateNewRecordTextAsync()
        {
            return await CreateNewRecordText.InnerTextAsync();
        }

        public async Task<string> CheckPageHeaderTextAsync()
        {
            return await HeaderLocator.TextContentAsync() ?? string.Empty;  //Rhz added is Empty
        }

        public async Task<bool> CheckEnabledFilterButtonAsync()
        {
            return await Filter.IsEnabledAsync();
        }

        public async Task CheckPaginationLinkAsync(ILocator locator)
        {
            Assert.That(locator, Is.Not.Null);
        }

        public async Task SearchWithFilterAsync(string selectWarnings, string selectYear)
        {
            
            await WarningType.SelectOptionAsync(new SelectOptionValue { Label = selectWarnings });
            await Year.SelectOptionAsync(new SelectOptionValue { Label = selectYear });
            await Filter.ClickAsync();
            Assert.That(TableHeader, Is.Not.Null);
        }

        public async Task VerifyTableHeaderAsync()
        {
            var tableColsHeader = await _page.EvalOnSelectorAllAsync<string[]>(
                ".table>thead>tr>th",
                "nodes => nodes.map(n => n.textContent.trim())"
            );
            var filteredHeader = tableColsHeader.Where(h => !string.IsNullOrWhiteSpace(h)).ToList();
            bool match = TableHeaderText.Count == filteredHeader.Count &&
                         TableHeaderText.SequenceEqual(filteredHeader);
            Assert.That(match, Is.True);
        }

        public async Task VerifyTableColumnWarningTypeDataAsync(string expectedText)
        {
            var result = await _page.EvalOnSelectorAllAsync<string[]>(
                "[id^='WarningTypeName']",
                "nodes => nodes.map(n => n.textContent)"
            );
            Assert.That(result.Length > 0);
            foreach (var item in result)
            {
                Assert.That(item.Trim(), Is.EqualTo(expectedText));
            }
        }

        public async Task VerifyTableDateColumnDataAsync(string yearString)
        {
            var resultYear = await _page.EvalOnSelectorAllAsync<string[]>(
                "[id^='DateTimeGroupRnwFormat']",
                "nodes => nodes.map(n => n.textContent.trim().slice(-2))"
            );
            Assert.That(resultYear.Length > 0);
            foreach (var y in resultYear)
            {
                Assert.That(y, Is.EqualTo(yearString[^2..]));
            }

            var resultDate = await _page.EvalOnSelectorAllAsync<string[]>(
                "[id^='DateTimeGroupRnwFormat']",
                "nodes => nodes.map(n => n.textContent.trim().slice(6))"
            );
            var sortedDesc = resultDate.OrderByDescending(d => d).ToArray();
            Assert.That(sortedDesc.SequenceEqual(resultDate));
        }

        public async Task VerifyTableContainsEditLinkAsync()
        {
            var resultLinks = await _page.EvalOnSelectorAllAsync<string[]>(
                "[id^='Edit']",
                "nodes => nodes.map(n => n.textContent)"
            );
            foreach (var link in resultLinks)
            {
                Assert.That(link.Trim(), Is.EqualTo("Edit"));
            }
        }
    }
}
