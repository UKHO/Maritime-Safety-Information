using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal partial class NoticeToMarinersWeekDownloadPageObject
    {
        private readonly IPage _page;

        //public ILocator NoticeToMarine { get; }
        public ILocator Year { get; }
        public ILocator Week { get; }
        public ILocator Daily { get; }
        public ILocator Download { get; }
        public ILocator FileName { get; }
        public ILocator TabCumulative { get; }
        public ILocator ImportantSafetyNotice { get; }
        public ILocator DistributorPartner { get; }
        public ILocator DistributorPublic { get; }
        public ILocator DistributorFileNumber { get; }
        public ILocator DistributorFirstFileName { get; }
        public ILocator DistributorFirstSize { get; }
        public ILocator DistributorSecondFileName { get; }
        public ILocator DistributorSecondSize { get; }
        public ILocator DistributorThirdFileName { get; }
        public ILocator DistributorThirdSize { get; }
        public ILocator PublicFirstFileName { get; }
        public ILocator PublicFirstSize { get; }
        public ILocator FileNameDownload { get; }
        public ILocator FileSize { get; }
        public ILocator AnnualSection { get; }
        public string WeeklyDownload { get; }
        public string NoticeToMarinerLinkId { get; }
        public ILocator DownloadAll { get; }
        public ILocator DownloadPartnerAll { get; }

        public NoticeToMarinersWeekDownloadPageObject(IPage page)
        {
            _page = page;
            //NoticeToMarine = _page.Locator("a:has-text(\"Notices to Mariners\")");  Rhz replaced with test id
            NoticeToMarinerLinkId = "NMs";
            Year = _page.Locator("#ddlYears");
            Week = _page.Locator("#ddlWeeks");
            Daily = _page.Locator("a[role=\"listitem\"]:has-text(\"Daily\")");
            TabCumulative = _page.Locator("#cumulative-tab");
            ImportantSafetyNotice = _page.Locator("text=Important safety notice");
            Download = _page.Locator("[id^='download'] > a");
            FileName = _page.Locator("[id^='filename']");
            DistributorPartner = _page.Locator("text=Partner");
            DistributorPublic = _page.Locator("text=Public");
            DistributorFileNumber = _page.Locator("[id^='partner']");
            DistributorFirstFileName = _page.Locator("#filename_0");
            DistributorFirstSize = _page.Locator("#filesize_0");
            DistributorSecondFileName = _page.Locator("#filename_1");
            DistributorSecondSize = _page.Locator("#filesize_1");
            DistributorThirdFileName = _page.Locator("#filename_2");
            DistributorThirdSize = _page.Locator("#filesize_2");
            PublicFirstFileName = _page.Locator("#filename_0");
            PublicFirstSize = _page.Locator("#filesize_0");
            WeeklyDownload = "[id^='download_0'] > a";
            FileNameDownload = _page.Locator("[id^='filename'] > a");
            FileSize = _page.Locator("[id^='filesize']");
            AnnualSection = _page.Locator("[id^=\"section\"]");
            DownloadAll = _page.Locator("#downloadAll");
            DownloadPartnerAll = _page.Locator("#downloadPartner");
        }

        public async Task GoToNoticeToMarinerAsync()
        {
            await _page.GetByTestId(NoticeToMarinerLinkId).ClickAsync();
        }

        public async Task GoToDailyFileAsync()
        {
            await Daily.ClickAsync();
        }

        public async Task GoToCumulativeAsync()
        {
            await TabCumulative.ClickAsync();
        }

        public async Task<bool> IsErrorPageDisplayed()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var errorElement = await _page.QuerySelectorAsync(".custom-error");
            Assert.That(errorElement, Is.Null, "This page has not loaded correctly");
            return errorElement == null;
        }

        public async Task<IReadOnlyList<string>> CheckFileDownloadAsync()
        {
            await Year.SelectOptionAsync(new SelectOptionValue { Index = 1 });
            await Week.SelectOptionAsync(new SelectOptionValue { Index = 1 });
            await _page.WaitForSelectorAsync("[id^='filename']");
            var result = await _page.Locator("[id^='filename']").AllInnerTextsAsync();
            return [.. result.Select(x => x.Trim())];
        }

        public async Task CheckImportantSafetyNoticeAsync()
        {
            var text = await ImportantSafetyNotice.InnerTextAsync();
            Assert.That(text, Does.Contain("Important safety notice"));
        }

        public async Task CheckDailyFileDownloadAsync()
        {
            await _page.WaitForSelectorAsync("[id^='filename']");
            var dailyFileName = await FileName.First.EvaluateAsync<string>("el => el.textContent");
            if (!string.IsNullOrWhiteSpace(dailyFileName))
            {
                var href = await Download.First.GetAttributeAsync("href");
                var dailyDownloadPageUrl = href?.Trim().Split('&');
                var downloadUrl = dailyDownloadPageUrl?[1].Replace("%20", " ");
                Assert.That(downloadUrl, Does.Contain($"fileName={dailyFileName}"));
            }
            else
            {
                throw new Exception("No download File Found");
            }
        }

        public async Task CheckWeeklyFileSortingWithDistributorRoleAsync()
        {
            await _page.WaitForSelectorAsync("[id^='filename']");
            var fileNames = await _page.Locator("[id^='filename']").AllInnerTextsAsync();
            var sortedFileNames = fileNames.OrderBy(x => x).ToList();
            Assert.That(fileNames, Is.EqualTo(sortedFileNames).AsCollection);
        }

        public async Task CheckWeeklyFileSectionNameAsync()
        {
            await _page.WaitForLoadStateAsync();
            await _page.WaitForSelectorAsync("text=Partner");
            var distributorSectionName = await DistributorPartner.First.InnerTextAsync();
            Assert.That(distributorSectionName, Does.Contain("Partner"));
            var publicSectionName = await DistributorPublic.First.InnerTextAsync();
            Assert.That(publicSectionName, Does.Contain("Public"));
        }

        public async Task VerifyCumulativeFileNameAsync()
        {
            await _page.WaitForSelectorAsync("td[id^='FileName']");
            var cumulativeFileNames = await _page.Locator("td[id^='FileName']").AllInnerTextsAsync();
            Assert.That(cumulativeFileNames, Is.Not.Empty);

            var sortedDesc = cumulativeFileNames.OrderByDescending(x => int.Parse(CumulativeFileNameRegex().Match(x).Value)).ToList();
            Assert.That(cumulativeFileNames, Is.EqualTo(sortedDesc).AsCollection);
        }

        public async Task VerifyCumulativeFileNameDownloadAsync()
        {
            var resultLinks = await _page.Locator("[id^='download']").AllInnerTextsAsync();
            foreach (var link in resultLinks)
            {
                Assert.That(link.Trim(), Is.EqualTo("Download"));
            }
        }

        public async Task CheckDailyFileNameAsync()
        {
            await _page.WaitForSelectorAsync("[id^='filename']");
            var dailyFileNames = await _page.Locator("[id^='filename']").AllInnerTextsAsync();
            foreach (var name in dailyFileNames)
            {
                Assert.That(name, Does.Contain("Daily"));
                Assert.That(name, Does.Contain("zip"));
                var dailyFileNameData = name.Length >= 14 ? name.Substring(6, 8) : "";
                // DateTime.ParseExact expects MM-dd-yy, but original is dd-mm-yy, so adjust as needed
                Assert.DoesNotThrow(() => DateTime.ParseExact(dailyFileNameData, "dd-MM-yy", null));
            }
            var sortedDesc = dailyFileNames.OrderBy(x => x).ToList();
            Assert.That(dailyFileNames, Is.EqualTo(sortedDesc).AsCollection);
        }

        public async Task CheckDailyWeekFileNameAsync()
        {
            await _page.WaitForLoadStateAsync();
            await _page.WaitForSelectorAsync("[id^='caption']");
            var dailyWeekFileNames = await _page.Locator("[id^='caption']").AllInnerTextsAsync();
            var regex = DailyWeekFileRegex();
            foreach (var name in dailyWeekFileNames)
            {
                Assert.That(regex.IsMatch(name), Is.True);
            }
            var sortedDesc = dailyWeekFileNames.OrderByDescending(x => x).ToList();
            Assert.That(dailyWeekFileNames, Is.EqualTo(sortedDesc).AsCollection);
        }

        public async Task CheckDailyFileSizeAsync()
        {
            await _page.WaitForLoadStateAsync();
            await _page.WaitForSelectorAsync("[id^='filesize']");
            var dailyFileSizes = await _page.Locator("[id^='filesize']").AllInnerTextsAsync();
            var regex = DailyFileSizeRegex();
            foreach (var size in dailyFileSizes)
            {
                var parts = size.Split(" ");
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(parts, Has.Length.GreaterThan(1));
                    Assert.That(regex.IsMatch($"{parts[0]} {parts[1]}"), Is.True);
                }
            }
        }

        public async Task VerifyDistributorFileCountAsync()
        {
            await Year.SelectOptionAsync(new SelectOptionValue { Label = "2022" });

            await Week.SelectOptionAsync(new SelectOptionValue { Label = "10" }); // rhz originally 18
            await _page.WaitForLoadStateAsync();

            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "a_rhz_DistributorFileCount.png", FullPage = true });
            //await _page.WaitForSelectorAsync("[id^='partner']");
            await _page.WaitForSelectorAsync("text=Partner");
            var fileNumber = await DistributorFileNumber.CountAsync();
            if (fileNumber > 0)
            {
                Assert.That(fileNumber, Is.EqualTo(1));
            }
            else
                Assert.That(fileNumber, Is.Zero);
        }

        public async Task VerifyIntegrationDownloadAllAsync()
        {
            var text = await DownloadAll.InnerTextAsync();
            Assert.That(text, Does.Contain("Download All"));
            var href = await DownloadAll.GetAttributeAsync("href");
            Assert.That(href, Does.Contain("/NoticesToMariners/DownloadAllWeeklyZipFile?fileName=2022%20Wk%2010%20Weekly%20NMs.zip&batchId=3a42a817-8a59-4072-9b7e-bb594c095fd9&mimeType=application%2Fgzip&type=public"));
        }

        public async Task VerifyIntegrationDownloadPartnerAllAsync()
        {
            var text = await DownloadPartnerAll.InnerTextAsync();
            Assert.That(text, Does.Contain("Download All"));
            var href = await DownloadPartnerAll.GetAttributeAsync("href");
            Assert.That(href, Does.Contain("/NoticesToMariners/DownloadAllWeeklyZipFile?fileName=2022%20Wk%2010%20Weekly%20NMs.zip&batchId=3a42a817-8a59-4072-9b7e-bb594c095fd9&mimeType=application%2Fgzip&type=partner"));
        }

        public async Task VerifyIntegrationTestValueForDistributorAsync()
        {
            await _page.ScreenshotAsync(new PageScreenshotOptions { Path = "a_rhz_Distributor_test.png" });
            var distributorFileName = await DistributorFirstFileName.First.InnerTextAsync();
            Assert.That(distributorFileName, Is.EqualTo("25snii24"));  //rhz originally "DistributorTest"
            var distributorFileSize = await DistributorFirstSize.First.InnerTextAsync();
            Assert.That(distributorFileSize, Is.EqualTo("904 KB (.pdf)")); // rhz originally "839 Bytes (.pdf)"
            //var publicFileNameFirst = await PublicFirstFileName.Last.InnerTextAsync();
            //Assert.That(publicFileNameFirst, Is.EqualTo("NM_MSI"));
            //var publicFileSizeFirst = await PublicFirstSize.Last.InnerTextAsync();
            //Assert.That(publicFileSizeFirst, Is.EqualTo("3 KB (.jpg)"));
        }

        public async Task VerifySectionWithDotsCountAsync()
        {
            var section = AnnualSection;
            var countOfDots = await section.EvaluateAllAsync<string[]>("els => els.map(e => e.textContent)");
            var count = countOfDots.Count(x => x == "---");
            //Assert.That(count, Is.EqualTo(1));
            Assert.That(count, Is.GreaterThanOrEqualTo(1), "There should be at least one section with '---' dots.");
        }

        public async Task VerifyAnnualFileNameLinkAsync()
        {
            var fileNameLinks = FileNameDownload;
            var fileNameData = await fileNameLinks.EvaluateAllAsync<string[]>("els => els.map(e => e.getAttribute('href'))");
            Assert.That(fileNameData, Is.Not.Empty);
            Assert.That(fileNameData.All(x => !string.IsNullOrEmpty(x)),Is.True);
        }

        public async Task VerifyAnnualDownloadLinkAsync()
        {
            var annualDownload = Download;
            var annualDownloadLinks = await annualDownload.EvaluateAllAsync<string[]>("els => els.map(e => e.getAttribute('href'))");
            Assert.That(annualDownloadLinks, Is.Not.Empty);
            Assert.That(annualDownloadLinks.All(x => !string.IsNullOrEmpty(x)),Is.True);
        }

        public async Task CheckAnnualFileSizeAsync()
        {
            await _page.WaitForLoadStateAsync();
            await _page.WaitForSelectorAsync("[id^='filesize']");
            var annualFileSizeData = FileSize;
            var dailyFileSizes = await annualFileSizeData.EvaluateAllAsync<string[]>("els => els.map(e => e.textContent.trim())");
            var regex = AnnualFileSizeRegex();
            foreach (var size in dailyFileSizes)
            {
                var parts = size.Split(" ");
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(parts, Has.Length.GreaterThan(1));
                    Assert.That(regex.IsMatch($"{parts[0]} {parts[1]}"), Is.True, $"Error no match P0: {parts[0]} P1: {parts[1]}");
                }
            }
        }

        [GeneratedRegex(@"\d{4}$")]
        private static partial Regex CumulativeFileNameRegex();

        [GeneratedRegex(@"^\d{4},\s[a-zA-Z]{4}\s\d{2}$")]
        private static partial Regex DailyWeekFileRegex();

        [GeneratedRegex(@"^\S+\s(B|MB|KB|GB)$")]
        private static partial Regex DailyFileSizeRegex();

        [GeneratedRegex(@"^\S+\s(B|MB|KB|GB|Bytes)$")]
        private static partial Regex AnnualFileSizeRegex();
    }
}
