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
    await login.adLogin(app.RNWAdminAutoTest_User,app.RNWAdminAutoTest_Pass);
  })
  
});
