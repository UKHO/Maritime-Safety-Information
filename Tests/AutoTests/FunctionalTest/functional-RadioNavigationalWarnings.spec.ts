import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsList from '../../pageObject/RadioNavigationalWarningsAdminList.page';

test.describe("Goto maritime-safety-information Home Page", ()=> {
  
  test.beforeEach(async ({page}) => {
         await page.goto(app.adminurl);  
  });

  test('Does the Yearly and Weekly Drop Down is enabled',async ({page}) => {
   const WarningTypeEnable = await RadioNavigationalWarningsList.();
   expect(WarningTypeEnable).toBeTruthy();
   const weeklyEnable = await RadioNavigationalWarningsList.checkEnabledYearDropDown();
   expect(weeklyEnable).toBeTruthy();     
})

test('Does the Table data is displayed',async ({page}) => {
  

})
});
