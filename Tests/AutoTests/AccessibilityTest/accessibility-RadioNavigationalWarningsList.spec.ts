import { test} from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import { checkA11y, injectAxe, Options } from 'axe-playwright';

import RadioNavigationalWarningsList from '../../pageObject/RadioNavigationalWarningsAdminList.page';

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
    await page.goto(app.rnwAdminUrl);    
  });


  

  test('Radio Navigational Warnings page should be accessible', async ({page}) => {
    await injectAxe(page);
    await checkA11y(page, undefined, defaultCheckA11yOptions);
    
  })
 
});