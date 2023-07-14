import { expect, test } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';
import loginPage from '../../pageObject/Login.page';

test.describe("Create new radio navigational warnings record", () => {
    let radioNavigationalWarnings: RadioNavigationalWarnings;
    let login: loginPage;

    test.beforeEach(async ({ page }) => {
        await page.goto(app.rnwAdminUrl);
        login = new loginPage(page);
        await login.adLogin(app.RNWAdminAutoTest_User, app.RNWAdminAutoTest_Pass);
        radioNavigationalWarnings = new RadioNavigationalWarnings(page);
    });

    test('Without entered input fields', async ({ page }) => {
        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.warningError, "The Warning Type field is required.");
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.referenceEror, "The Reference field is required.");
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError, 'The Text field is required.');
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.datetimeError, "The Date Time Group field is required.");
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.summaryError, "The Description field is required.");
    })

    test('With content text as blank', async ({ page }) => {
        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.fillFormWithValidDetails("1", "");
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError, 'The Text field is required.');
    })

    test('With valid input details with Navarea', async ({ page }) => {
        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.fillFormWithValidDetails("1", "testdata");
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.getDialogText('Record created successfully!')
    })

    test('With valid input details with UK costal', async ({ page }) => {
        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.fillFormWithValidDetails("2", "testdata");
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.getDialogText('Record created successfully!');
    })

    test('With valid input check for duplicate and accept', async ({ page }) => {

        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.fillFormWithValidDetails("1", "testdata");
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.confirmationBox(radioNavigationalWarnings.alertMessage, radioNavigationalWarnings.message, "yes")
        await radioNavigationalWarnings.getDialogText('Record created successfully!');
    })

    test('With valid input check for duplicate and reject', async ({ page }) => {

        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.fillFormWithValidDetails("1", "testdata");
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.confirmationBox(radioNavigationalWarnings.alertMessage, radioNavigationalWarnings.message, "no")
        await radioNavigationalWarnings.checkConfirmationBoxVisible(false)
    })

    test('With valid input check for duplicate and cancel', async ({ page }) => {

        await radioNavigationalWarnings.pageLoad();
        await radioNavigationalWarnings.fillFormWithValidDetails("1", "testdata");
        await radioNavigationalWarnings.createRNW();
        await radioNavigationalWarnings.confirmationBox(radioNavigationalWarnings.alertMessage, radioNavigationalWarnings.message, "cancel")
        await radioNavigationalWarnings.checkConfirmationBoxVisible(false)
    })
});
