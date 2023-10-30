import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import Login from '../../pageObject/Login.page';
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';
import noticeToMariner from '../../pageObject/noticetomarine.page';

test.describe("Goto maritime-safety-information Notice To Mariner Page to Check The Weekly and Daily File Download", () => {

    let noticeFileDownload: noticeToMarinerWeekDownload;
    let notice: noticeToMariner;
    let login: Login;

    test.beforeEach(async ({ page }) => {
        test.setTimeout(120 * 1000);
        await page.goto(app.url);
        noticeFileDownload = new noticeToMarinerWeekDownload(page);
        notice = new noticeToMariner(page);
        login = new Login(page);
    });

    test('Should Goto Notices to Mariner Page for Weekly Download', async ({ page, context }) => {
        await noticeFileDownload.goToNoticeToMariner();

        const name = await noticeFileDownload.checkFileDownload();

        expect((await name).length).toBeGreaterThan(0);
        var fileName = name[0];

        const newPageUrl = await (await page.$(noticeFileDownload.weelkydowanload)).getAttribute('href');
        expect(newPageUrl).toContain(`NoticesToMariners/DownloadFile?fileName=${fileName}`);
    })

    test('Does the Notices to Mariners Cumulative Page is displayed', async ({ page }) => {
        await noticeFileDownload.goToNoticeToMariner();
        await noticeFileDownload.goToCumulative();
        await noticeFileDownload.verifyCumulativeFileName();
        await noticeFileDownload.verifyCumulativeFileNameDownload();
    })

    test('Does the Notices to Mariners Page urls are displayed with page title', async ({ page }) => {
        await notice.checkpageurl('', 'Maritime Safety Information')
        await notice.checkurl(notice.noticeMarine.first(), 'NoticesToMariners/Weekly', 'Notices to Mariners - Weekly')
        await notice.checkurl(notice.tabdaily, 'NoticesToMariners/Daily', 'Notices to Mariners - Daily')
        await notice.checkurl(notice.tabcumulative, 'NoticesToMariners/Cumulative', 'Notices to Mariners - Cumulative')
        await notice.checkurl(notice.tabannual, 'NoticesToMariners/Annual', 'Notices to Mariners - Annual')
        await notice.checkurl(notice.menuValueAddedResellers, 'NoticesToMariners/Resellers', 'Notices to Mariners - Value Added Resellers')
        await notice.checkurl(notice.menuAbout, 'NoticesToMariners/About', 'About Notices to Mariners')
        await notice.checkurl(notice.radioNavigationalWarnings, 'RadioNavigationalWarnings', 'Radio Navigation Warnings')
        await notice.checkNavareaUrl(notice.navareatab, 'RadioNavigationalWarnings', 'Radio Navigation Warnings - NAVAREA I')
        await notice.checkUkcoastalUrl(notice.ukcoastaltab, 'RadioNavigationalWarnings', 'Radio Navigation Warnings - UK Coastal')

    })
});
