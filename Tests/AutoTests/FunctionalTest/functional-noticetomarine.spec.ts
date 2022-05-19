import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import noticeToMarine from '../../pageObject/noticetomarine.page';

test.describe("Maritime Safety Information Notice to Marine Page Functional Test Scenarios", ()=> {
 
      let notice:any;

       test.beforeEach(async ({page}) => {
         await page.goto(app.url);  
         notice = new noticeToMarine(page);
         await notice.clickToNoticemarine();
      });  
     
       test('Does the Yearly and Weekly Drop Down is enabled, Menu and Tabs text is Displayed',async ({page}) => {          
         expect(notice.checkEnabledYearDropDown()).toBeTruthy();
         expect(notice.checkEnabledWeekDropDown()).toBeTruthy();
         expect(notice.checkText(notice.menuNoticeToMarine)).toEqual("Notice to Mariners");
         expect(notice.checkText(notice.menuLeisureFolios)).toEqual("Leisure folios");
         expect(notice.checkText(notice.menuValueAddedResellers)).toEqual("Value added resellers");
         expect(notice.checkText(notice.menuAbout)).toEqual("About");
         expect(notice.checkText(notice.tabweekly)).toEqual("Weekly");
         expect(notice.checkText(notice.tabdaily)).toEqual("Daily");
         expect(notice.checkText(notice.tabcumulative)).toEqual("Cumulative");  
         expect(notice.checkText(notice.tabannual)).toEqual("Annual");    
       })
       test('Does the table record count',async ({page}) => {
         const tablerecordCount = notice.checkTableRecordCount();
         expect(tablerecordCount).toBeGreaterThan(0);
         notice.verifyTableContainsDownloadLink();
         
      })
       test('Does the Table Data For Yearly and Weekly Drop Down Include Table Data,File Name and File Size',async ({page}) => {
         notice.checkFileSizeData();
         notice.checkFileNameSort();
      })     
    })
