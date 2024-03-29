import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';
import noticeToMarine from '../../pageObject/noticetomarine.page';


test.describe("Maritime Safety Information Notices to Mariners Annual Page Functional Test Scenarios", ()=> {
 
       let annual:noticeToMarine;
       let annualTab:noticeToMarinerWeekDownload;

    test.beforeEach(async ({ page }) => {
        test.slow(true);
        await page.goto(app.url);
        annual = new noticeToMarine(page);
        annualTab = new noticeToMarinerWeekDownload(page);
        await annual.clickToNoticemarine();
      });  
     
       
       test('Does the Table Data For Annual Include Section,File Name,File Size and download',async ({page}) => {
         await annual.clickToNoticemarineAnnual();
         await annualTab.verifySectionWithDotsCount();
         await annualTab.verifyAnnualFileNameLink();
         await annualTab.verifyAnnualDownloadLink();
         await annualTab.checkAnnualFileSize();
      })  
     
     
    })
