import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import noticetomarine from '../../pageObject/noticetomarine.page';

test.describe("Maritime Safety Information Notice to Marine Page Functional Test Scenarios", ()=> {
 
 
  let notice:any;
  
  
  
  test.beforeEach(async ({page}) => {
      await page.goto(app.url);  
       notice = new noticetomarine(page);
       await notice.clickToNoticemarine();
    });  
      test('Does the Yearly and Weekly Drop Down is enabled',async ({page}) => {
         const yearenable = await notice.noticeMarinePageTheYearDropDownIsEnabled();
          expect(yearenable).toBeTruthy();
          const weeklyenable = await notice.noticeMarinePageTheweeklyDropDownIsEnabled();
          expect(weeklyenable).toBeTruthy();     
       })
  
      test('Does the Table Data Include the File size',async ({page}) => {
         const tablecount = await notice.noticeMarinecRecordCount('2022','16');
         expect(tablecount).toBeGreaterThan(0);   
         expect(await notice.getFileNameText()).toEqual('File Name');
         expect(await notice.getFileSizeText()).toEqual('File Size');  
         await page.waitForSelector('td')
         const fileSizeData = await page.$$("td:nth-child(2)");
         for (const table of fileSizeData)
         {
           var fileData = await (await table.innerText());
           if(fileData.includes("MB"))
          {
             expect(fileData).toContain("MB"); 
          }
          else if (fileData.includes("KB"))
          {
            expect(fileData).toContain("KB");
          }
          else if(fileData.includes("bytes"))
          {
            expect(fileData).toContain("bytes");
          }
          else{}
         }
      })
      

  
          
    })