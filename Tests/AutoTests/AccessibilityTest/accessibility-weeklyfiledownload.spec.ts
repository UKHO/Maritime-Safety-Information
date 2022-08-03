import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";

import { injectAxe, getViolations, checkA11y, Options} from 'axe-playwright'
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';
let noticeFileDownload:noticeToMarinerWeekDownload;
test.describe("A11y tests", ()=> {
  const defaultCheckA11yOptions: Options = {
   axeOptions: {
     runOnly: {
       type: 'tag',
       values: ['wcag2aa'],
     },
     reporter: 'v2'
   },
   detailedReport: true,
   detailedReportOptions: { html: true }
 };

 test.beforeEach(async ({page}) => {
    test.slow();
   await page.goto(app.url);noticeFileDownload = new noticeToMarinerWeekDownload(page);
   await noticeFileDownload.goToNoticeToMariner();
   await noticeFileDownload.goToDailyFile();    
 });

 

 test('Notice To Mariner page should be accessible for daily file', async ({page}) => {
  
  await injectAxe(page);
  await checkA11y(page, undefined, defaultCheckA11yOptions);
  
})

});
