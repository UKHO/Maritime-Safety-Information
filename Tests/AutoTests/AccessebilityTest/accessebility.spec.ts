import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as data from "../../configuration/pageObject.json";
import * as app from "../../Configuration/appConfig.json";
import headerPage from '../../pageObject/home.page';
import { injectAxe, getViolations, checkA11y} from 'axe-playwright'

test.describe("Goto maritime-safety-information Home Page ", ()=> {
  let browser: Browser;
  let context: BrowserContext;
  let page: Page;
  let homepage:headerPage;
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
    await injectAxe(page);
  });
  test('Sample Test Accessibility Check', async () => {
       
})
});
