import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";
import loginPage from '../../pageObject/Login.page';

test.describe("Sign in For The maritime-safety-information", () => {
  let login: loginPage;
  test.beforeEach(async ({ page }) => {
    await page.goto(app.url);
    login = new loginPage(page);
    await login.goToSignIn();
  });

  test('With the Valid details', async ({ page, context }) => {
    await login.loginWithValidDetails(app.username, app.password);
    await login.signout();
  })
});
