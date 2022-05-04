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
     rnw.warningtypedropdown();
     rnw.refrencetext().fill('refrence');
     rnw.datetimegroup().fill('05042022');
     rnw.descriptiontext().fill('testdata');
     rnw.contenttext().fill('testdata');
     page.on('dialog',(dialog)=>{
       expect(dialog.message).toEqual('Record created successfully');
     })
     rnw.createbuttonclick().click();
  } )  
  test('withValidDetailexceptContent',async({page})=>{   
    rnw.warningtypedropdown();
    rnw.refrencetext().fill('testdata');
    rnw.datetimegroup().fill('05042022');
    rnw.descriptiontext().fill('testdata');
    page.on('dialog',(dialog)=>{
      expect(dialog.message).toEqual('Failed to create record.')
    })
    rnw.createbuttonclick().click();
 })  
});
