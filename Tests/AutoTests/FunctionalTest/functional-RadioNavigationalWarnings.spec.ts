import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarningsList from '../../pageObject/RadioNavigationalWarningsAdminList.page';

test.describe("Goto maritime-safety-information Home Page", () => {
    let rnwList: RadioNavigationalWarningsList;
    test.beforeEach(async ({ page }) => {
        await page.goto(app.rnwAdminUrl);
        rnwList = new RadioNavigationalWarningsList(page);
    });

    test('Does the Warning Type,Year Drop Down is enabled and Page Header Text,Create Record Text is Displayed', async () => {
        const WarningTypeEnable = await rnwList.checkEnabledWarningTypeDropDown();
        expect(WarningTypeEnable).toBeTruthy();
        const yearEnable = await rnwList.checkEnabledYearDropDown();
        expect(yearEnable).toBeTruthy();
        const createRecordList = await rnwList.checkCreateNewrecordText();
        expect(createRecordList).toBeTruthy();
        const pageHeader = await rnwList.checkPageHeaderText();
        expect(pageHeader).toEqual("Radio Navigational Warnings Admin List");
    })

    test('Dose filter display search result for warning types as "UK Coastal" or  "NAVAREA 1" ', async () => {
        //search UK Coastal
        await rnwList.searchWithfilter('UK Coastal', '2022');
        await rnwList.verifyTableHeader();
        await rnwList.verifyTableColumnWarningTypeData('UK Coastal');
        await rnwList.verifyTableContainsEditLink();

        //search NAVAREA 1
        await rnwList.searchWithfilter('NAVAREA 1', '2022');
        await rnwList.verifyTableHeader();
        await rnwList.verifyTableColumnWarningTypeData('NAVAREA 1');
        await rnwList.verifyTableContainsEditLink();

    })


    test('Dose filter display search result sorted in descending order', async () => {
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
