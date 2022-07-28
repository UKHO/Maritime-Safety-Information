import { test, expect, chromium,firefox,webkit, Page, Browser,BrowserType, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";

import { injectAxe, getViolations, checkA11y, Options} from 'axe-playwright'
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';
let noticeCumulative:noticeToMarinerWeekDownload;
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

 test.beforeEach(async ({page,}) => {
    test.slow();
    await page.goto(app.url);    
    noticeCumulative = new noticeToMarinerWeekDownload(page);
  await noticeCumulative.goToNoticeToMariner();
  await noticeCumulative.goToCumulative();
 });

test('Notices To Mariners page should be accessible for cumulative file', async ({page}) => {
  await injectAxe(page);
  await checkA11y(page, undefined, defaultCheckA11yOptions);
  
})
});
