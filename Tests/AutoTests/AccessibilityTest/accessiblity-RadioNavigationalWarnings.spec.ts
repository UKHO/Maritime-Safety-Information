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
    await page.goto(app.url);
    const login = new loginPage(page);
    await login.goToSignIn();
    await login.loginWithValidDetails(app.B2CAutoTest_User,app.B2CAutoTest_Pass);   
  });

  test('Radio Navigational Warnings page should be accessible', async ({page}) => {
    await injectAxe(page);
    await checkA11y(page, undefined, defaultCheckA11yOptions);
  })
 
});
