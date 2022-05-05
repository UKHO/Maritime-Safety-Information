import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import homepage from '../../pageObject/home.page';
import noticemarinepage from '../../pageObject/noticeToMarine.page';

test.describe("Maritime Safety Information Home Page Functional Test Scenarios", ()=> {
 
  let home:any;
  let notice:any;
  test.beforeEach(async ({page}) => {
    await page.goto(app.url);  
    home =new homepage(page);
  });

   test('Does it open Admirality page once click on Admirality logo', async ({page}) => {
    await home.admirilitylogo();
    await expect(page.url()).toContain('admiralty');
  } )  

  test('Does it contains correct header text', async ({page}) => {
        expect(await home.headertext()).toEqual('Maritime Safety Information');
  } )  

  test('Does it contains correct copyright text', async ({page}) => {
    expect(await home.footertext()).toEqual("© Crown copyright " + new Date().getFullYear() + " UK Hydrographic Office");
})
  test('Does it open Notice to marine page', async ({page}) => {
    await home.gotoNoticemarinePage();
    expect(page.url()).toContain('NoticesToMariners');
    })

    test('Does it open Radio Navigational Warnings page once click on Radio Navigational Warnings link', async ({page}) => {
      expect(await home.RadioNavigationalWarnings()).toContain("Radio Navigational Warnings");
})

test('Does it open Privacy policy page once click on Privacy policy link', async ({page,context}) => {
  await page.click('#privacypolicylink');
  context.on('page', async page => {
    await page.waitForLoadState();
    expect(page.url()).toContain("cookie-policy");
})
  })
  test('Does it open Accessibility page once click on Accessibility link', async ({page,context}) => {
           expect(await page.innerText('#accessibilitylink')).toContain("Accessibility");
  })
});
