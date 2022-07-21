import { test} from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import { checkA11y, injectAxe, Options } from 'axe-playwright';
import RadioNavigationalWarningsListEndUser from '../../pageObject/RadioNavigationalWarningsAdminListEndUser.page';

let rnwListEndUser:RadioNavigationalWarningsListEndUser;

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
    test.slow()
    await page.goto(app.url);    
    rnwListEndUser = new RadioNavigationalWarningsListEndUser(page);
    await rnwListEndUser.goToRadioWarning();
    await rnwListEndUser.ukCostalEnduser.click();
  
  });
  test('Radio Navigational Warnings For Uk Coastal page should be accessible', async ({page}) => {
     await injectAxe(page);  
     await checkA11y(page, undefined, defaultCheckA11yOptions);
 })
 
});