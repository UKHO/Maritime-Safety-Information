import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";

test.describe("Goto maritime-safety-information Home Page", ()=> {
  
  test.beforeEach(async ({page}) => {
    await page.goto(app.url);  
  });

  test('Should Goto Homepage Page',async({page})=>{   
     await expect(page.url().toString()).toEqual(app.url);
   } )   
});
