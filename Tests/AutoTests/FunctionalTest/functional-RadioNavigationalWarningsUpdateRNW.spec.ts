import { expect, test } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';
import loginPage from '../../pageObject/Login.page';

test.describe("Update radio navigational warnings record", () => {
    let radioNavigationalWarnings: RadioNavigationalWarnings;
    let login: loginPage;

    test.beforeEach(async ({ page }) => {
        test.setTimeout(120 * 1000);
        await page.goto(app.rnwAdminUrl);
        login = new loginPage(page);
        await login.adLogin(app.RNWAdminAutoTest_User, app.RNWAdminAutoTest_Pass);
        radioNavigationalWarnings = new RadioNavigationalWarnings(page);
    });

    test('Update with summary, reference and content text as blank', async ({ page }) => {
        await radioNavigationalWarnings.getEditUrl();
        await radioNavigationalWarnings.clearInput(radioNavigationalWarnings.description);
        await radioNavigationalWarnings.clearInput(radioNavigationalWarnings.reference);
        await radioNavigationalWarnings.clearInput(radioNavigationalWarnings.content);
        await radioNavigationalWarnings.editRNW();
        await page.waitForTimeout(5000);
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.summaryError, 'The Description field is required.');
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.referenceEror, "The Reference field is required.");
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError, 'The Text field is required.');
    })

    test('Update with valid input details with UK Coastal', async ({ page }) => {
        await radioNavigationalWarnings.searchListWithfilter('UK Coastal');
        await radioNavigationalWarnings.getEditUrl();
        await radioNavigationalWarnings.isDelete();
        await radioNavigationalWarnings.fillEditFormWithValidDetails("testdata");
        await radioNavigationalWarnings.editRNW();
        await radioNavigationalWarnings.getDialogText('Record updated successfully!')
    })

    test('Update with valid input details with NAVAREA', async ({ page }) => {
        await radioNavigationalWarnings.searchListWithfilter('NAVAREA 1');
        await radioNavigationalWarnings.getEditUrl();
        await radioNavigationalWarnings.isDelete();
        await radioNavigationalWarnings.fillEditFormWithValidDetails("testdata");
        await radioNavigationalWarnings.editRNW();
        await radioNavigationalWarnings.getDialogText('Record updated successfully!')
    })
});
