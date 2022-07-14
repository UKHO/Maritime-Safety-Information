
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
  readonly distributorPartner:Locator;
  readonly distributorPublic:Locator;
  constructor(page: Page) {
    this.page = page;
    this.noticeToMarine = this.page.locator('a:has-text("Notices to Mariners")');
    this.year = this.page.locator('#ddlYears');
    this.week = this.page.locator('#ddlWeeks');
    this.daily = this.page.locator('a[role="listitem"]:has-text("Daily")');
    this.download = this.page.locator("[id^='download'] > a");
    this.fileName = this.page.locator("[id^='filename']");
    this.distributorPartner=this.page.locator('text=Partner');
    this.distributorPublic=this.page.locator('text=Partner');

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

  public async checkWeeklyFileSortingWithDistributorRole()
  {
   
    await this.page.waitForSelector("[id^='filename']");
    const fileNames= await this.page.$$eval("[id^='filename']", (matches: any[]) => { return matches.map(option => option.textContent) }); ;
    const sortedFilename =fileNames.sort()
    expect(sortedFilename).toEqual(fileNames);
  
  }

  public async checkWeeklyFileSectionName()
  {
    const distributorSectionName=await (await this.distributorPartner.first().innerText());
    expect(distributorSectionName).toContain('Partner');
    const publicSectionName=await (await this.distributorPublic.first().innerText());
    expect(publicSectionName).toContain('Public');

  }
}




