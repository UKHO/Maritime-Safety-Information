import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsList from '../../pageObject/RadioNavigationalWarningsAdminList.page';

test.describe("Goto maritime-safety-information Admin List Page", ()=> {
   let radioNavigationalWarningsList:RadioNavigationalWarningsList;
   
   test.beforeEach(async ({page}) => {
       await page.goto(app.RnwAdminUrl);
   radioNavigationalWarningsList = new RadioNavigationalWarningsList(page);
   });

  test('Does the Warning Type,Year Drop Down is enabled and Page Header Text,Create Record Text is Displayed',async ({page}) => {
   let warningTypeEnable = await radioNavigationalWarningsList.checkEnabledWarningTypeDropDown();
   expect(warningTypeEnable).toBeTruthy();
   const yearEnable = await radioNavigationalWarningsList.checkEnabledYearDropDown();
   expect(yearEnable).toBeTruthy();
   let createRecordList= await radioNavigationalWarningsList.checkCreateNewrecordText();
   expect(createRecordList).toEqual("Create New Warning");
   let pageHeader = await radioNavigationalWarningsList.checkPageHeaderText();
   expect(pageHeader).toEqual("Radio Navigational Warnings List");
   })

  test('Does the Table data is displayed with Pagination',async ({page}) => {
   await radioNavigationalWarningsList.getTableList();
   await radioNavigationalWarningsList.pagination(radioNavigationalWarningsList.btnFirst);
   await radioNavigationalWarningsList.pagination(radioNavigationalWarningsList.btnLast);
   await radioNavigationalWarningsList.pagination(radioNavigationalWarningsList.btnNext);
   await radioNavigationalWarningsList.pagination(radioNavigationalWarningsList.btnPrevious);
})
});
