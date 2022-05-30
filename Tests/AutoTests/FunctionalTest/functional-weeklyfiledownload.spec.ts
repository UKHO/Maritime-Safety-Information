import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';

test.describe("Goto maritime-safety-information Notice To Marine Page", () => {
  let noticeFileDownload: noticeToMarinerWeekDownload;
  test.beforeEach(async ({ page }) => {
    await page.goto(app.url);
    noticeFileDownload = new noticeToMarinerWeekDownload(page);
  });

  test('Should Goto Notice to Mariner Page for Weekly Download', async ({ page, context }) => {
    await noticeFileDownload.goToNoticeToMariner();

    const name = await noticeFileDownload.checkFileDownload();

    expect((await name).length).toBeGreaterThan(0);
    var fileName = name[0];

    const newPageUrl = await (await page.$("[id^='download'] > a")).getAttribute('href');
    expect(newPageUrl).toContain(`NoticesToMariners/DownloadWeeklyFile?fileName=${fileName}`);
  })

  test('Should Goto Notice to Mariner Page for Daily download File', async ({ page, context }) => {
    await noticeFileDownload.goToNoticeToMariner();
    await noticeFileDownload.goToDailyFile();
    const firstFileDownloadName = await noticeFileDownload.getFirstDailyFileName();
    const fileDownloadName=await noticeFileDownload.checkDailyFileDownload();
    expect(fileDownloadName).toContain(`fileName=${firstFileDownloadName}`);
  })
});
