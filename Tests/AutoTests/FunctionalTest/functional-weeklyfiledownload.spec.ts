import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import noticeToMarineWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';

test.describe("Goto maritime-safety-information Notice To Marine Page", ()=> {
  let noticeFileDownload:noticeToMarineWeekDownload;
  test.beforeEach(async ({page}) => {
  await page.goto(app.url);  
  noticeFileDownload = new noticeToMarineWeekDownload(page);
  });

  test('Should Goto Homepage Page',async({page,context})=>{   
    await noticeFileDownload.goToNoticeToMarine();
    const name=await noticeFileDownload.checkFileDownload();
    
    expect((await name).length).toBeGreaterThan(0);
    var fileName = name.slice(1,2);
    
    const [newPage] = await Promise.all([context.waitForEvent('page'), await page.click('a[target="_blank"]'), ])
   
    await page.waitForLoadState();
    expect(newPage.url()).toContain(app.url.concat("NoticesToMariners/DownloadWeeklyFile?fileName=").concat(fileName.toString()));
    newPage.close();

     } )   
});
