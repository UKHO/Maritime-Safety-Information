import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsListEndUser from '../../pageObject/RadioNavigationalWarningsAdminListEndUser.page';


test.describe("Goto maritime-safety-information Home Page", ()=> {
  let rnwListEndUser:RadioNavigationalWarningsListEndUser;
  

    test.beforeEach(async ({ page }) => {
 
        await page.goto(app.url);
        rnwListEndUser = new RadioNavigationalWarningsListEndUser(page);
        await rnwListEndUser.goToRadioWarning();
  });

  test('Does the Menu , Tabs and Page Text is displayed',async () => { 
    expect(await rnwListEndUser.checkText(rnwListEndUser.radioNavigationalWarningsEndUser)).toEqual("Radio Navigation Warnings");
    expect(await rnwListEndUser.checkText(rnwListEndUser.radioWarningEndUser)).toEqual("Radio Warnings");
    expect(await rnwListEndUser.checkText(rnwListEndUser.aboutEndUser)).toEqual("About");
    expect(await rnwListEndUser.checkText(rnwListEndUser.allWarningEndUser)).toEqual("All warnings");
    expect(await rnwListEndUser.checkText(rnwListEndUser.navAreaEndUser)).toEqual("NAVAREA 1");
    expect(await rnwListEndUser.checkText(rnwListEndUser.ukCostalEnduser)).toEqual("UK Coastal");
   })

   test('Does the Table data ,Table Header Text and View details link with Date Sorting is Displayed',async ({page,context}) => {
    await rnwListEndUser.verifyTableHeader();
    await rnwListEndUser.verifyTableContainsViewDetailsLink();
    await rnwListEndUser.verifyTableDateColumnData();
    await rnwListEndUser.verifyTableViewDetailsUrl();
    await rnwListEndUser.verifyImportantBlock();
    await rnwListEndUser.verifySelectOptionText();
    await rnwListEndUser.verifySelectOptionCheckBox();
    await rnwListEndUser.verifyPrint();
    
  })

  test('Does the Table data has navarea1 data with date sorting is displayed ',async ({page,context}) => {
  
    await rnwListEndUser.verifyNavareaAndUkCostalFilter(rnwListEndUser.navAreaEndUser,"NAVAREA 1")
   
  })
  test('Does the Table data has UK Coastal data with date sorting is displayed ',async ({page,context}) => {
    
   await rnwListEndUser.verifyNavareaAndUkCostalFilter(rnwListEndUser.ukCostalEnduser,"UK Coastal")
  })


});
