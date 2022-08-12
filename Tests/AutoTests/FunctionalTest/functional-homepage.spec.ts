import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import homePage from '../../pageObject/Home.page';

test.describe("Home Page For The Maritime Safety Information", () => {
  let home: homePage;
  test.beforeEach(async ({ page }) => {
    await page.goto(app.url);
    home=new homePage(page);
  });

  test('Should go to Home Page', async ({ page, context }) => {
    
    await home.verifyAdmiraltyHomePage();
    await home.verifyadmiralty();
    await home.verifyHomePageTitle();
    await home.verifypage();
    await home.verifyukHydrographic();
    await home.verifyprivacypolicy();
    await home.verifyaccessibility();
  })  
});
