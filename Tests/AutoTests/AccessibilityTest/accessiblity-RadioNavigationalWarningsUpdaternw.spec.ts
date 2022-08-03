import { test} from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import { checkA11y, injectAxe, Options } from 'axe-playwright';
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';
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
    await page.goto(app.rnwAdminUrl);
    let login = new loginPage(page);
    await login.adLogin(app.RNWAdminAutoTest_User,app.RNWAdminAutoTest_Pass); 
    const  radioNavigationalWarnings =new RadioNavigationalWarnings(page);
    await radioNavigationalWarnings.getEditUrl();
  });

  test('Radio Navigational Warnings Update page should be accessible', async ({page}) => {
    await injectAxe(page);
    
    await checkA11y(page, undefined, defaultCheckA11yOptions);
    
  })
 
});
