import { expect, test } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';
import loginPage from '../../pageObject/Login.page';

test.describe("Create duplicate radio navigational warnings record", () => {
    let radioNavigationalWarnings: RadioNavigationalWarnings;
    let login: loginPage;

    test.beforeEach(async ({ page }) => {
        await page.goto(app.rnwAdminUrl);
        login = new loginPage(page);
        await login.adLogin(app.RNWAdminAutoTest_User, app.RNWAdminAutoTest_Pass);
        radioNavigationalWarnings = new RadioNavigationalWarnings(page);
    });

    test('With valid input check for duplicate and reject', async ({ page }) => {

        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.fillFormWithValidDetails("1", "testdata");
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.confirmationBox(radioNavigationalWarnings.alertMessage, radioNavigationalWarnings.message, "no")
        await radioNavigationalWarnings.checkConfirmationBoxVisible(false)
    })

});
