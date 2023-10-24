import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";
import Login from '../../pageObject/Login.page';
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';
import noticeToMariner from '../../pageObject/noticetomarine.page';

test.describe("Goto maritime-safety-information Notice To Mariner Page to Check The Weekly and Daily File Download", () => {
  let noticeFileDownload: noticeToMarinerWeekDownload;
  let notice:noticeToMariner;
  let login:Login;
  test.beforeEach(async ({ page }) => {
      test.setTimeout(test.timeout + 60000);

      await page.goto(app.url);
      noticeFileDownload = new noticeToMarinerWeekDownload(page);
      notice = new noticeToMariner(page);
      login=new Login(page);
  });

  test('Should Goto Notices to Mariner Page for Daily download File', async ({ page, context }) => {
    await noticeFileDownload.goToNoticeToMariner();
    await noticeFileDownload.goToDailyFile();
    await noticeFileDownload.checkDailyFileDownload();
  })

  test('Should Goto Notices to Mariner Page for Daily download File with Distributor Login', async ({ page, context }) => {
  await login.goToSignIn();
  await login.loginWithDistributorDetails(app.DistributorTest_UserName, app.DistributorTest_Password);
  await noticeFileDownload.goToNoticeToMariner();
  await noticeFileDownload.goToDailyFile();
  await noticeFileDownload.checkDailyFileName();
  await noticeFileDownload.checkDailyFileSize();
  await noticeFileDownload.checkDailyFileDownload();
  await noticeFileDownload.checkDailyWeekFileName()
})
});
