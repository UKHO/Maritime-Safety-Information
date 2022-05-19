import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsListEndUser from '../../pageObject/RadioNavigationalWarningsAdminListEndUser.page';


test.describe("Goto maritime-safety-information Home Page", ()=> {
  let rnwListEndUser:RadioNavigationalWarningsListEndUser;

  test.beforeEach(async ({page}) => {
    await page.goto(app.url);
    rnwListEndUser = new RadioNavigationalWarningsListEndUser(page);
    await rnwListEndUser.goToRadioWarning();
    
  });

  test('Does the Menu , Tabs and Page Header Text is displayed',async () => { 
    expect(await rnwListEndUser.checkText(rnwListEndUser.radioNavigationalWarningsEndUser)).toEqual("Radio Navigation Warnings");
    expect(await rnwListEndUser.checkText(rnwListEndUser.radioWarningEndUser)).toEqual("Radio Warnings");
    expect(await rnwListEndUser.checkText(rnwListEndUser.aboutEndUser)).toEqual("About");
    expect(await rnwListEndUser.checkText(rnwListEndUser.allWarningEndUser)).toEqual("All warnings");
    expect(await rnwListEndUser.checkText(rnwListEndUser.navAreaEndUser)).toEqual("NAVAREA 1");
    expect(await rnwListEndUser.checkText(rnwListEndUser.ukCostalEnduser)).toEqual("UK Costal");
   })

   test('Does the Warning Type,Year Drop Down is enabled and Page Header Text,Create Record Text is Displayed',async () => {
    await rnwListEndUser.verifyTableContainsViewDetailsLink();
    await rnwListEndUser.verifyTableDateColumnData();
  })
 
});
