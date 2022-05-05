import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import { injectAxe, getViolations, checkA11y, Options} from 'axe-playwright'
import homepage from '../../pageObject/home.page';
import noticetomarine from '../../pageObject/noticetomarine.page';

  test.describe("MSI Notice to Marine Page Accessibility Test Scenarios", ()=> {
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
  
    test('Radio Navigational Warnings page should be accessible', async ({page}) => {
  
      await injectAxe(page);
      await checkA11y(page, undefined, defaultCheckA11yOptions);
    })
});
