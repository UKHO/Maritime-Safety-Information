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

    test('Should Goto Notices to Mariner Page for Weekly Download with Distributor Role', async ({ page, context }) => {
        await login.goToSignIn();
        await login.loginWithDistributorDetails(app.DistributorTest_UserName, app.DistributorTest_Password);
        await noticeFileDownload.goToNoticeToMariner();
        await noticeFileDownload.checkWeeklyFileSectionName();
        await noticeFileDownload.checkWeeklyFileSortingWithDistributorRole();
        const name = await noticeFileDownload.checkFileDownload();
        expect((await name).length).toBeGreaterThan(0);
        var fileName = name[0];
        const newPageUrl = await (await page.$(noticeFileDownload.weelkydowanload)).getAttribute('href');
        expect(newPageUrl).toContain(`NoticesToMariners/DownloadFile?fileName=${fileName}`);
    })

    test('Should Goto Notices to Mariner Page for Weekly NM files with Distributor Role', async ({ page, context }) => {
        await login.goToSignIn();
        await login.loginWithDistributorDetails(app.DistributorTest_UserName, app.DistributorTest_Password);
        await noticeFileDownload.goToNoticeToMariner();
        await noticeFileDownload.verifyDistributorFileCount();
        await noticeFileDownload.verifyIntegrationTestValueForDistributor();
        await noticeFileDownload.verifyIntegrationDownloadAll();
        await noticeFileDownload.verifyIntegrationDownloadPartnerAll();
    })
});
