import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import noticeToMarine from '../../pageObject/noticetomarine.page';


test.describe("Maritime Safety Information Notice to Marine Page Functional Test Scenarios", ()=> {
 
       let notice:any;

       test.beforeEach(async ({page}) => {
        test.slow();
         await page.goto(app.url);  
         notice = new noticeToMarine(page);
         await notice.clickToNoticemarine();
      });  
     
       test('Does the Yearly and Weekly Drop Down is enabled, Menu and Tabs text is Displayed',async ({page}) => {          
         expect(notice.checkEnabledYearDropDown()).toBeTruthy();
         expect(notice.checkEnabledWeekDropDown()).toBeTruthy();
         expect(await notice.checkText(notice.menuNoticeToMarine)).toEqual("Notices to Mariners");
         expect(await notice.checkText(notice.menuLeisureFolios)).toEqual("Leisure Folios");
         expect(await notice.checkText(notice.menuValueAddedResellers)).toEqual("Value Added Resellers");
         expect(await notice.checkText(notice.menuAbout)).toEqual("About");
         expect(await notice.checkText(notice.tabweekly)).toEqual("Weekly");
         expect(await notice.checkText(notice.tabdaily)).toEqual("Daily");
         expect(await notice.checkText(notice.tabcumulative)).toEqual("Cumulative");  
         expect(await notice.checkText(notice.tabannual)).toEqual("Annual");    
       })
       
       test('Does the Table Data is Displayed With The Record Count',async ({page}) => {
         expect(await notice.checkTableRecordCount()).toBeGreaterThan(0);
         await notice.verifyTableContainsDownloadLink();
      })
       test('Does the Table Data For Yearly and Weekly Drop Down Include Table Data,File Name and File Size',async ({page}) => {
         await notice.checkFileSizeData();
         await notice.checkFileNameSort();
      })  
     
     
    })