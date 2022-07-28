import { test} from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import { checkA11y, injectAxe, Options } from 'axe-playwright';

import loginPage from '../../pageObject/Login.page';


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
    test.slow();
    await page.goto(app.url);    
    const login = new loginPage(page);
    await login.goToSignIn();
  });

  test('Radio Navigational Warnings page should be accessible', async ({page}) => {
    
    test.setTimeout(12000);
    await injectAxe(page);
    
    await checkA11y(page, undefined, defaultCheckA11yOptions);
    
  })
 
});