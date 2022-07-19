
import { expect } from '@playwright/test';
import { LocaleOptions } from 'luxon';
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
  readonly distributorPartner:Locator;
  readonly distributorPublic:Locator;
  readonly distributorFileNumber:Locator;
  readonly distributorFirstFileName:Locator;
  readonly distributorFirstSize:Locator;
  readonly distributorSecoundFileName:Locator;
  readonly distributorSecoundSize:Locator;
  readonly distributorThirdFileName:Locator;
  readonly distributorThirdSize:Locator;
  readonly publicFirstFileName:Locator;
  readonly publicFirstSize:Locator;
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
    this.distributorPartner=this.page.locator('text=Partner');
    this.distributorPublic=this.page.locator('text=Public');
    this.distributorFileNumber=this.page.locator("[id^='distributor']");
    this.distributorFirstFileName=this.page.locator('#filename_1');
    this.distributorFirstSize=this.page.locator('#filesize_1');
    this.distributorSecoundFileName=this.page.locator('#filename_2');
    this.distributorSecoundSize=this.page.locator('#filesize_2');
    this.distributorThirdFileName=this.page.locator('#filename_3');
    this.distributorThirdSize=this.page.locator('#filesize_3');
    this.publicFirstFileName=this.page.locator('#filename_0');
    this.publicFirstSize=this.page.locator('#filesize_0');

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

  public async checkWeeklyFileSortingWithDistributorRole()
  {
   
    await this.page.waitForSelector("[id^='filename']");
    const fileNames= await this.page.$$eval("[id^='filename']", (matches: any[]) => { return matches.map(option => option.textContent) }); ;
    const sortedFilename =fileNames.sort()
    expect(sortedFilename).toEqual(fileNames);
  
  }

  public async checkWeeklyFileSectionName()
  {
    await this.page.waitForLoadState();
    await this.page.waitForSelector('text=Partner');
    const distributorSectionName=await (await this.distributorPartner.first().innerText());
    expect(distributorSectionName).toContain('Partner');
    const publicSectionName=await (await this.distributorPublic.first().innerText());
    expect(publicSectionName).toContain('Public');

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

  public async verifyDistributorFileCount()
  { 
    await this.page.waitForLoadState();
    await this.page.waitForSelector("[id^='distributor']");
    const fileNumber=await this.distributorFileNumber.count();
    expect(fileNumber).toEqual(8);

  }

  public async verifyIntegrationTestValueForDistributor()
  {
    const distributorFileName=await this.distributorFirstFileName.first().textContent();
    expect(distributorFileName).toEqual("2022-29");
    const distributorFileSize=await this.distributorFirstSize.first().textContent();
    expect(distributorFileSize).toEqual("1 MB (.pdf)");
    const distributorFileNameSecound=await this.distributorSecoundFileName.first().textContent();
    expect(distributorFileNameSecound).toEqual("29sect4");
    const distributorFileSizeSecound=await this.distributorSecoundSize.first().textContent();
    expect(distributorFileSizeSecound).toEqual("13 KB (.rtf)");
    const distributorFileNameThird=await this.distributorThirdFileName.first().textContent();
    expect(distributorFileNameThird).toEqual("lights-2022-29");
    const distributorFileSizeThird=await this.distributorThirdSize.first().textContent();
    expect(distributorFileSizeThird).toEqual("796 KB (.xml)");

    const publicFileNameFirst=await this.publicFirstFileName.last().textContent();
    expect(publicFileNameFirst).toEqual("29snii22");
    const publicFileSizeFirst=await this.publicFirstSize.last().textContent();
    expect(publicFileSizeFirst).toEqual("650 KB (.pdf)");

  }
}