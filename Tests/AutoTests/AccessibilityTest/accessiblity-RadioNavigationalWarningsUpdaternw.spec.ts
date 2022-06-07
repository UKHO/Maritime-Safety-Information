import { test} from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import { checkA11y, injectAxe, Options } from 'axe-playwright';
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';



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
    const  radioNavigationalWarnings =new RadioNavigationalWarnings(page);
    await radioNavigationalWarnings.getEditUrl();
  });

  test('Radio Navigational Warnings Update page should be accessible', async ({page}) => {
    await injectAxe(page);
    
    await checkA11y(page, undefined, defaultCheckA11yOptions);
    
  })
 
});
