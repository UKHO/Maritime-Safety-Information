import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";
import noticetomarine from '../../pageObject/noticetomarine.page';
import { injectAxe, getViolations, checkA11y, Options} from 'axe-playwright'

test.describe("MSI Notices to Mariners About Page Accessibility Test", () => {
    let notice:any;
    const defaultCheckA11yOptions: Options = {
        axeOptions: {
            runOnly: {
                type: 'tag',
                values: ['wcag2aa'],
            },
            reporter: 'v2'
        },
        detailedReport: true,
        detailedReportOptions: { html: true }
    };

    test.beforeEach(async ({ page }) => {
        test.slow();
        await page.goto(app.url);
        notice = new noticetomarine(page);
        await notice.clickToNoticemarine();
        await notice.clickToNoticemarineAbout();
    });

    test('Notices to Mariners About page should be accessible', async ({ page }) => {
        await injectAxe(page);
        await checkA11y(page, undefined, defaultCheckA11yOptions);
    })
});
