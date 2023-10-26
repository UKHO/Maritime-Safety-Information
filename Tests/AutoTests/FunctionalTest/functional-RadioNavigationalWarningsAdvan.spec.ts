import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsList from '../../pageObject/RadioNavigationalWarningsAdminList.page';
import loginPage from '../../pageObject/Login.page';

test.describe("Goto maritime-safety-information Home Page", () => {

    let rnwList: RadioNavigationalWarningsList;
    let login: loginPage;

    test.beforeEach(async ({ page }) => {
        await page.goto(app.rnwAdminUrl);
        login = new loginPage(page);
        await login.adLogin(app.RNWAdminAutoTest_User, app.RNWAdminAutoTest_Pass);
        rnwList = new RadioNavigationalWarningsList(page);
    });

    test('Does filter display search result sorted in descending order', async () => {
        await rnwList.searchWithfilter('UK Coastal', '2022');
        await rnwList.verifyTableHeader();
        await rnwList.verifyTableDateColumnData('2022');
    })

    test('Does the Table data is displayed with Pagination', async () => {
        await rnwList.searchWithfilter('UK Coastal', '2022');
        await rnwList.verifyTableHeader();
        await rnwList.checkPaginationLink(rnwList.btnFirst);
        await rnwList.checkPaginationLink(rnwList.btnLast);
        await rnwList.checkPaginationLink(rnwList.btnNext);
        await rnwList.checkPaginationLink(rnwList.btnPrevious);
    })
});
