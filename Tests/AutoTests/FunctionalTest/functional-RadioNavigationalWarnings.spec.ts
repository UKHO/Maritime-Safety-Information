import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsList from '../../pageObject/RadioNavigationalWarningsAdminList.page';

test.describe("Goto maritime-safety-information Home Page", ()=> {
  let RadioNavigationalWarningsList:RadioNavigationalWarningsList;
  test.beforeEach(async ({page}) => {
   await page.goto(app.adminurl);  
  });

  test('Does the Warning Type,Year Drop Down is enabled and Page Header Text,Create Record Text is Displayed',async ({page}) => {
   const WarningTypeEnable = await RadioNavigationalWarningsList.checkEnabledWarningTypeDropDown();
   expect(WarningTypeEnable).toBeTruthy();
   const yearEnable = await RadioNavigationalWarningsList.checkEnabledYearDropDown();
   expect(yearEnable).toBeTruthy();
   const createRecordList= await RadioNavigationalWarningsList.checkCreateNewrecordText();
   expect(createRecordList).toBeTruthy();
   const pageHeader = await RadioNavigationalWarningsList.checkPageHeaderText();
   expect(pageHeader).toContain("Radio Navigational Warnings List");
})

  test('Does the Table data is displayed with Pagination',async ({page}) => {
   RadioNavigationalWarningsList.getTableList();
   RadioNavigationalWarningsList.pagination(RadioNavigationalWarningsList.btnFirst)
   RadioNavigationalWarningsList.pagination(RadioNavigationalWarningsList.btnLast)
   RadioNavigationalWarningsList.pagination(RadioNavigationalWarningsList.btnNext)
   RadioNavigationalWarningsList.pagination(RadioNavigationalWarningsList.btnPrevious)
})
});
