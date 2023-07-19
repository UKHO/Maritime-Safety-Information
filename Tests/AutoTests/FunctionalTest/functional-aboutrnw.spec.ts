import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsListEndUser from '../../pageObject/RadioNavigationalWarningsAdminListEndUser.page';

test.describe("Maritime Safety Information Radio Navigational Warnings About Page Functional Test Scenarios", () => {
    let rnwListEndUser: RadioNavigationalWarningsListEndUser;
    test.beforeEach(async ({ page }) => {
        test.slow();
        await page.goto(app.url);
        rnwListEndUser = new RadioNavigationalWarningsListEndUser(page);
        await rnwListEndUser.goToRadioWarning();
    });

    test('Does the table data for Radio Navigational Warnings About page is displayed', async ({ page }) => {
        await rnwListEndUser.verifyAboutRnw();
        await rnwListEndUser.verifyAboutRNWImportantBlock();
    })
})
