import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as data from "../../Configuration/pageObject.json";
import * as app from "../../Configuration/appConfig.json";
import home from '../../pageObject/home.page';

test.describe("Goto maritime-safety-information Home Page", ()=> {
  let browser: Browser;
  let context: BrowserContext;
  let page: Page;
  let homepage:home;

  test.beforeAll(async () => {
    browser = await chromium.launch();
  });

  test.afterAll(async () => {
     await browser.close();
  });
  
  test.afterEach(async () => {
    await page.close();
    await context.close();
  });

  test.beforeEach(async () => {
    context = await browser.newContext();
    page = await context.newPage();
    await page.goto(app.url);  
    const header = new home(page);
  });

 test('Should Goto Homepage Page',async()=>{   
    await expect(page.url().toString()).toEqual(app.url);
    } )
});
