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
      test('Does the Yearly and Weekly Drop Down is enabled',async ({page}) => {
         const yearEnable = await notice.checkEnabledYearDropDown();
         expect(yearEnable).toBeTruthy();
         const weeklyEnable = await notice.checkEnabledWeekDropDown();
         expect(weeklyEnable).toBeTruthy();     
       })
   
      test('Does the Table Data For Yearly and Weekly Drop Down Include Table Data,File Name and File Size',async ({page}) => {
         await notice.getTableData();
      })  
   
    })
