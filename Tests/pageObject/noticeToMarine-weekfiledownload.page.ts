
import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';


export default class noticeToMarinerWeekDownload {
  private page: Page;
  readonly noticeToMarine: Locator;
  readonly year: Locator;
  readonly week: Locator;
  readonly daily: Locator;
  readonly download: Locator;
  readonly fileName: Locator;
  readonly fileSize: Locator;
  readonly fileSizeHeaderText : Locator;
  readonly fileNameHeaderText: Locator;
  constructor(page: Page) {
    this.page = page;
    this.noticeToMarine = this.page.locator('a:has-text("Notices to Mariners")');
    this.year = this.page.locator('#ddlYears');
    this.week = this.page.locator('#ddlWeeks');
    this.daily = this.page.locator('a[role="listitem"]:has-text("Daily")');
    this.download = this.page.locator("[id^='download'] > a");
    this.fileName = this.page.locator("[id^='filename']");
    this.fileNameHeaderText=this.page.locator('text=File Name');
    this.fileSizeHeaderText= this.page.locator('text=File Size');
  }

  public async goToNoticeToMariner() {
    await this.noticeToMarine.click();
  }

  public async goToDailyFile() {
    await this.daily.click();
  }
  public async checkFileDownload() {
    await this.year.selectOption({index:1});
    await this.week.selectOption({index:1});
    await this.page.waitForSelector("[id^='filename']");
    const result = this.page.$$eval("[id^='filename']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
    return result;
  }

  public async verifySortDailyFileDownload()
  {
    await this.page.waitForSelector("[id^='filename']");
    const fileNameData = await this.page.$$eval('td[id^=filename]' , (matches: any[]) => { return matches.map(option => option.textContent) });  
    const beforeSortFilename= fileNameData;
    fileNameData.sort();
    const afterSortFileName = fileNameData;
    expect(beforeSortFilename).toEqual(afterSortFileName);
  }

  public async checkDailyFileDownload() {
    await this.page.waitForSelector("[id^='filename']");
    const dailyfileName = await this.fileName.first().evaluate((name) => name.textContent);
    expect(dailyfileName?.length).toBeGreaterThan(0)
    
    if ((await dailyfileName).length > 0) {
      const dailyDownloadPageUrl = await (await this.download.first().getAttribute('href')).trim().split("&");
      const downloadurl = dailyDownloadPageUrl[1].replace(/%20/g, " ");
      expect(downloadurl).toContain(`fileName=${dailyfileName}`);
    }
    else {
      throw new Error("No download File Found")
    }
  }

  public async checkFileSizeText()
    {
     return (await this.fileSizeHeaderText.first().textContent()).toString();   
    }
    public async checkFileNameText()
    {
     return (await this.fileNameHeaderText.first().textContent()).toString();   
    }
   
  
}




