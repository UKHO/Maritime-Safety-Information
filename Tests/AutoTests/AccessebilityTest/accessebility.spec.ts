import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';

import * as app from "../../Configuration/appConfig.json";
import headerPage from '../../pageObject/home.page';
import { injectAxe, getViolations, checkA11y} from 'axe-playwright'

test.describe("A11y tests", ()=> {
 
  test.beforeEach(async ({page}) => {
    await page.goto(app.url);
    await injectAxe(page);
  });
 
  test('Sample Test Accessibility Check', async () => {  
    expect('test').toEqual('test');
  })
});
