import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import loginPage from '../../pageObject/Login.page';

test.describe("AD Authentication Sign in For The maritime-safety-information admin app", () => {
    let login: loginPage;
    test.beforeEach(async ({ page }) => {
        await page.goto(app.rnwAdminUrl);
        login = new loginPage(page);
    });

    test('With the Valid details', async ({ page, context }) => {
        await login.adLogin(app.RNWAdminAutoTest_User, app.RNWAdminAutoTest_Pass);
        await login.adSignOut();
    })

    test('With the Unauthorised details', async ({ page, context }) => {
        await login.adLogin(app.RNWAdminAutoTestNoAccess_User, app.RNWAdminAutoTestNoAccess_Pass);
        await login.adUnathorisedDetails();
    })

    test('With the Invalid details', async ({ page, context }) => {
        await login.adLogin(app.RNWAdminAutoTest_User, '1111111');
        await login.adPasswordErrorCheck();
    })
});
