import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import homepage from '../../pageObject/home.page';
import noticemarinepage from '../../pageObject/noticeToMarine.page';

test.describe("Maritime Safety Information Notice to Marine Page Functional Test Scenarios", ()=> {
 
let home:any;
let notice:any;


test.beforeEach(async ({page}) => {
    await page.goto(app.url);  
     home =new homepage(page);
     notice = new noticemarinepage(page);
  });

   test('Does it open Notice to marine page once click on Notice to Marine Header Menu', async ({page}) => {
    await home.gotoNoticemarinePage();
    await expect(page.url()).toContain('NoticesToMariners');
    })

    test('Does the yearly Drop Down is enable',async ({page}) => {
       await home.gotoNoticemarinePage();
       const yearenable = await notice.NoticeMarinePageTheYearDropDownIsEnabled();
        expect(yearenable).toBeTruthy();
     })

    test('Does the weekly Drop Down is enable',async ({page}) => {
       await home.gotoNoticemarinePage();
       page.waitForTimeout(3000);
       const weeklyenable = await notice.NoticeMarinePageTheweeklyDropDownIsEnabled();
      expect(weeklyenable).toBeTruthy();      
     })

    test('Does the Drop Down list count have record',async ({page}) => {
       await home.gotoNoticemarinePage();
       page.waitForTimeout(3000);
       const tablecount = await notice.noticemarinecrecordcount();
       expect(tablecount).toBeGreaterThan(0);      
    })
        
  })