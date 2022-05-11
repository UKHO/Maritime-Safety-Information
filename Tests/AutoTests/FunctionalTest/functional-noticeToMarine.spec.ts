import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import noticeToMarineList from '../../pageObject/noticeToMarineAdminList.page';

test.describe("Goto maritime-safety-information Home Page", ()=> {
  
  test.beforeEach(async ({page}) => {
         await page.goto(app.url);  
  });

  test('Does the Yearly and Weekly Drop Down is enabled',async ({page}) => {
   const WarningTypeEnable = await noticeToMarineList.checkEnabledWarningTypeDropDown();
   expect(WarningTypeEnable).toBeTruthy();
   const weeklyEnable = await noticeToMarineList.checkEnabledYearDropDown();
   expect(weeklyEnable).toBeTruthy();     
})

test('Does the Table data is displayed',async ({page}) => {


})
});
