import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";
import Login from '../../pageObject/Login.page';
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';
import noticeToMariner from '../../pageObject/noticetomarine.page';

test.describe("Goto maritime-safety-information Notice To Mariner Page to Check The Weekly and Daily File Download", () => {
  let noticeFileDownload: noticeToMarinerWeekDownload;
  let notice:noticeToMariner;
  let login: Login;
  test.beforeEach(async ({ page }) => {
    test.slow();
    await page.goto(app.url);
    noticeFileDownload = new noticeToMarinerWeekDownload(page);
    notice = new noticeToMariner(page);
    login=new Login(page);
  });

  test('Should Goto Notices to Mariner Page for Weekly Download', async ({ page, context }) => {
    await noticeFileDownload.goToNoticeToMariner();

    const name = await noticeFileDownload.checkFileDownload();

    expect((await name).length).toBeGreaterThan(0);
    var fileName = name[0];

    const newPageUrl = await (await page.$(noticeFileDownload.weelkydowanload)).getAttribute('href');
    expect(newPageUrl).toContain(`NoticesToMariners/DownloadFile?fileName=${fileName}`);
  })

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

  

  test('Does the Notices to Mariners Cumulative Page is displayed',async ({page}) => {
    await noticeFileDownload.goToNoticeToMariner();
    await noticeFileDownload.goToCumulative();
    await noticeFileDownload.verifyCumulativeFileName();
    await noticeFileDownload.verifyCumulativeFileNameDownload();
  }) 
   

  test('Does the Notices to Mariners Page urls are displayed with page title',async ({page}) => {
    await notice.checkpageurl('','Maritime Safety Information')
    await notice.checkurl(notice.noticeMarine.first(),'NoticesToMariners/Weekly','Notices to Mariners - Weekly')
    await notice.checkurl(notice.tabdaily,'NoticesToMariners/Daily','Notices to Mariners - Daily')
    await notice.checkurl(notice.tabcumulative,'NoticesToMariners/Cumulative','Notices to Mariners - Cumulative')
    await notice.checkurl(notice.tabannual,'NoticesToMariners/Annual','Notices to Mariners - Annual')
    await notice.checkurl(notice.menuLeisureFolios,'NoticesToMariners/Leisure','Notices to Mariners - Leisure')
    await notice.checkurl(notice.menuValueAddedResellers,'NoticesToMariners/Resellers','Notices to Mariners - Value Added Resellers')
    await notice.checkurl(notice.menuAbout,'NoticesToMariners/About','About Notices to Mariners')
    await notice.checkurl(notice.radioNavigationalWarnings,'RadioNavigationalWarnings','Radio Navigational Warnings')
    await notice.checkNavareaUrl(notice.navareatab,'RadioNavigationalWarnings','Radio Navigational Warnings - NAVAREA I')
    await notice.checkUkcoastalUrl(notice.ukcoastaltab,'RadioNavigationalWarnings','Radio Navigational Warnings - UK Coastal')
    
  })  

 test('Should Goto Notices to Mariner Page for Leisure Folios is displayed', async ({ page, context }) => {
  await noticeFileDownload.goToNoticeToMariner();
  await noticeFileDownload.goToLeisureFolios();
  await noticeFileDownload.checkFurtherInformation();
  await noticeFileDownload.checkImportantSafetyNotice();
  await noticeFileDownload.verifyleisureFoliosFileName();
  await noticeFileDownload.verifyleisureFoliosFileNameDownload();
  
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
