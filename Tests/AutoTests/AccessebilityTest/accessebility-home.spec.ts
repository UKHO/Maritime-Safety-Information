import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";
import headerPage from '../../pageObject/home.page';
import { injectAxe, getViolations, checkA11y, Options} from 'axe-playwright'

test.describe("MSI Home Page Accessibility Test Scenarios", ()=> {
 
  test.beforeEach(async ({page}) => {
    await page.goto(app.url);
    await injectAxe(page);
  });
   
  test.describe("A11y tests", ()=> {
    const defaultCheckA11yOptions: Options = {
      axeOptions: {
        runOnly: {
          type: 'tag',
          values: ['wcag2aa'],
        },
        reporter: 'v2'
      },
      detailedReport: true,
      detailedReportOptions: { html: true }
    };
   
    test.beforeEach(async ({page}) => {
      await page.goto(app.url);    
      console.log(page.url());
    });
  
    test('Home page should be accessible', async ({page}) => {
      await injectAxe(page);
      await checkA11y(page, undefined, defaultCheckA11yOptions);
      
    })
  });
 
});
