
import { expect } from '@playwright/test';
import { DateTime, LocaleOptions } from 'luxon';
import type { Locator, Page } from 'playwright';




export default class noticeToMarinerWeekDownload {
  private page: Page;
  readonly noticeToMarine: Locator;
  readonly year: Locator;
  readonly week: Locator;
  readonly daily: Locator;
  readonly download: Locator;
  readonly fileName: Locator;
  readonly tabcumulative:Locator;
  readonly menuLeisureFolios:Locator;
  readonly leisureFolios:Locator;
  readonly importantSafetyNotice:Locator;
  readonly leisureFoliosFileName:Locator;
  readonly leisureFoliosFileSize:Locator;
  constructor(page: Page) {
    this.page = page;
    this.noticeToMarine = this.page.locator('a:has-text("Notices to Mariners")');
    this.year = this.page.locator('#ddlYears');
    this.week = this.page.locator('#ddlWeeks');
    this.daily = this.page.locator('a[role="listitem"]:has-text("Daily")');
    this.tabcumulative = this.page.locator("#cumulative-tab");

    this.menuLeisureFolios = this.page.locator('text=Leisure Folios');
    this.importantSafetyNotice=this.page.locator('text=Important safety notice');
    this.download = this.page.locator("[id^='download'] > a");
    this.fileName = this.page.locator("[id^='filename']");
    this.leisureFolios=this.page.locator('div > p > a');
  }

  public async goToNoticeToMariner() {
    await this.noticeToMarine.click();
  }

  public async goToDailyFile() {
    await this.daily.click();
  }
  public async goToCumulative()
  {
     await this.tabcumulative.click();
  }
  public async goToLeisureFolios()
  {
   await this.menuLeisureFolios.click();
  }

  public async checkFileDownload() {
    await this.year.selectOption({index:1});
    await this.week.selectOption({index:1});
    await this.page.waitForSelector("[id^='filename']");
    const result = this.page.$$eval("[id^='filename']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
    return result;
  }

  public async checkFurtherInformation()
  {  
    expect(await this.leisureFolios.getAttribute("aria-label")).toContain('Click here for Further Information');
  }
  public async checkImportantSafetyNotice()
  {
    expect(await (await this.importantSafetyNotice.innerText()).toString()).toContain("Important safety notice")
  }
  public async verifyleisureFoliosFileName() {
    await this.page.waitForSelector("td[id^='filename']");
    const leisurefileName  = await this.page.$$eval("td[id^='filename']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
    expect((await leisurefileName).length).toBeGreaterThan(0);
    const sortedDesc = leisurefileName.sort();
    expect(leisurefileName).toEqual(sortedDesc);
  }
  public async verifyleisureFoliosFileNameDownload()
  {
    const resultLinks= await this.page.$$eval('[id^="download"] > a' , (matches: any[]) => { return matches.map(option => option.textContent) });
    for(let i=0;i<resultLinks.length;i++)
    {
      expect(resultLinks[i].trim()).toEqual("Download");
    }
    
    await this.page.waitForSelector("[id^='filename']");
    const leisurefileName = await this.fileName.first().evaluate((name) => name.textContent);
    const leisureDownloadPageUrl = await (await this.download.first().getAttribute('href')).trim().split("&");
    const downloadurl = leisureDownloadPageUrl[0].replace(/%20/g, " ");
    expect(downloadurl).toContain(`fileName=${leisurefileName}`);
  }

  public async checkDailyFileDownload() {
    await this.page.waitForSelector("[id^='filename']");
    const dailyfileName = await this.fileName.first().evaluate((name) => name.textContent);

    if ((await dailyfileName).length > 0) {
      const dailyDownloadPageUrl = await (await this.download.first().getAttribute('href')).trim().split("&");
      const downloadurl = dailyDownloadPageUrl[1].replace(/%20/g, " ");
      expect(downloadurl).toContain(`fileName=${dailyfileName}`);
    }
    else {
      throw new Error("No download File Found")
    }
  }
  public async verifyCumulativeFileName() {
    await this.page.waitForSelector("td[id^='FileName']");
    const cumulativefileName = await this.page.$$eval("td[id^='FileName']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
    expect((await cumulativefileName).length).toBeGreaterThan(0);
    const sortedDesc = cumulativefileName.sort((objA, objB) => objB.date - objA.date,);
    expect(cumulativefileName).toEqual(sortedDesc);
  }
  public async verifyCumulativeFileNameDownload()
  {
    const resultLinks= await this.page.$$eval('[id^="download"]' , (matches: any[]) => { return matches.map(option => option.textContent) });
    for(let i=0;i<resultLinks.length;i++)
    {
      expect(resultLinks[i].trim()).toEqual("Download");
    }
  }

  public async checkDailyFileName()
 {
  await this.page.waitForLoadState();
  await this.page.waitForSelector("[id^='filename']");
 const dailyFileName = await this.page.$$eval("[id^='filename']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
 for(let i=0;i<=dailyFileName.length-1;i++)
 {
 expect(dailyFileName[i].trim()).toContain("Daily")
 expect(dailyFileName[i].trim()).toContain("zip")
const dailyfileNameData = dailyFileName[i].slice(6,14)
 expect(DateTime.fromFormat(dailyfileNameData,"dd-mm-yy")).toBeTruthy();
 }
 const sortedDesc = dailyFileName.sort();
 expect(sortedDesc).toEqual(dailyFileName);
 }

 public async checkDailyWeekFileName()
 {
   await this.page.waitForLoadState();
   await this.page.waitForSelector("[id^='caption']");
   const dailyWeekFileName = await this.page.$$eval("[id^='caption']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
   const regex=/^\d{4}\,\s[a-zA-Z]{4}\s\d{2}$/;
   for(let i=0;i<=dailyWeekFileName.length-1;i++)
   {
     expect(dailyWeekFileName[i].toString().match(regex)).toBeTruthy();
   }

   const sortedDesc = dailyWeekFileName.sort((objA,objB) => objB - objA);
   expect(sortedDesc).toEqual(dailyWeekFileName);

 } 
 public async checkDailyFileSize()
 {
   await this.page.waitForLoadState();
   await this.page.waitForSelector("[id^='filesize']");
   const dailyFileSize = await this.page.$$eval("[id^='filesize']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
   const regex=/^\S+\sB|MB|KB|GB|$/; 
   for(let i=0;i<=dailyFileSize.length-1;i++)
   {
    const fileSize=dailyFileSize[i].split(" ");
    expect(fileSize[1].toString().match(regex)).toBeTruthy();
   }
}
}