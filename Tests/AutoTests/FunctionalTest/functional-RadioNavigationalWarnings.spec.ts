import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';



test.describe("create the rnw record", ()=> {
  let rnw:any;

  test.beforeEach(async ({page}) => {
    await page.goto(app.url); 
    const rnw = new RadioNavigationalWarnings(page);
    rnw.creaternwrecord();
  });

   test('Withvaliddetails',async({page})=>{   
    await rnw.warningtypedropdown('NavArea');
    await rnw.refrencetext('refrence');
    await rnw.datetimegroup('05042022');
    await rnw.descriptiontext('testdata');
    await rnw.contenttext('testdata');
    await rnw.dialog('Record created successfully')
    await rnw.createbuttonclick();
  } )

  test('withValidDetailexceptContent',async({page})=>{   
    await rnw.warningtypedropdown('NavArea');
    await rnw.refrencetext('refrence');
    await rnw.datetimegroup('05042022');
    await rnw.descriptiontext('testdata');
    await rnw.contenttext('testdata');
    await rnw.dialog('Failed to create record.')
    await rnw.createbuttonclick();
 })  
});
