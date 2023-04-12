
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
  readonly importantSafetyNotice:Locator;
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
  readonly fileNameDownload:Locator;
  readonly fileSize:Locator;
  readonly annualSection:Locator;
  readonly weelkydowanload:string;
  readonly downloadAll:Locator;
  readonly downloadPartnerAll:Locator;
  constructor(page: Page) {
    this.page = page;
    this.noticeToMarine = this.page.locator('a:has-text("Notices to Mariners")');
    this.year = this.page.locator('#ddlYears');
    this.week = this.page.locator('#ddlWeeks');
    this.daily = this.page.locator('a[role="listitem"]:has-text("Daily")');
    this.tabcumulative = this.page.locator("#cumulative-tab");

    this.importantSafetyNotice=this.page.locator('text=Important safety notice');
    this.download = this.page.locator("[id^='download'] > a");
    this.fileName = this.page.locator("[id^='filename']");
    this.distributorPartner=this.page.locator('text=Partner');
    this.distributorPublic=this.page.locator('text=Public');
    this.distributorFileNumber=this.page.locator("[id^='partner']");
    this.distributorFirstFileName=this.page.locator('#filename_0');
    this.distributorFirstSize=this.page.locator('#filesize_0');
    this.distributorSecoundFileName=this.page.locator('#filename_1');
    this.distributorSecoundSize=this.page.locator('#filesize_1');
    this.distributorThirdFileName=this.page.locator('#filename_2');
    this.distributorThirdSize=this.page.locator('#filesize_2');
    this.publicFirstFileName=this.page.locator('#filename_0');
    this.publicFirstSize=this.page.locator('#filesize_0');
    this.weelkydowanload="[id^='download'] > a";
    this.fileNameDownload= this.page.locator("[id^='filename'] > a");
    this.fileSize = this.page.locator("[id^='filesize']");
    this.annualSection = this.page.locator('[id^="section"]');
    this.downloadAll = this.page.locator('#downloadAll');
    this.downloadPartnerAll = this.page.locator('#downloadPartner');
  }

  public async goToNoticeToMariner() {
    await this.noticeToMarine.first().click();
  }

  public async goToDailyFile() {
    await this.daily.click();
  }
  public async goToCumulative()
  {
     await this.tabcumulative.click();
  }

  public async checkFileDownload() {
    await this.year.selectOption({index:1});
    await this.week.selectOption({index:1});
    await this.page.waitForSelector("[id^='filename']");
    const result = this.page.$$eval("[id^='filename']", (options: any[]) => { return options.map(option => option.textContent.trim()) });
    return result;
  }

  public async checkImportantSafetyNotice()
  {
    expect(await (await this.importantSafetyNotice.innerText()).toString()).toContain("Important safety notice")
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

  public async verifyDistributorFileCount()
  { 


    await this.year.selectOption({label:'2022'});
    await this.week.selectOption({label:'26'});
    await this.page.waitForLoadState();
    await this.page.waitForSelector("[id^='partner']");
    const fileNumber=await this.distributorFileNumber.count();
    if(fileNumber > 0){
    expect(fileNumber).toEqual(3);
    }
    else{
    expect(fileNumber).toEqual(0);
    }

}
public async verifyIntegrationDownloadAll()
{
  expect(await this.downloadAll.innerText()).toContain('Download All')
  expect(await this.downloadAll.getAttribute('href')).toContain('/NoticesToMariners/DownloadAllWeeklyZipFile?fileName=2022%20Wk%2026%20Weekly%20NMs.zip&batchId=3db9e8c4-0dea-43c8-8de3-e875be26c418&mimeType=application%2Fgzip&type=public')
} 

public async verifyIntegrationDownloadPartnerAll()
{
  expect(await this.downloadPartnerAll.innerText()).toContain('Download All')
  expect(await this.downloadPartnerAll.getAttribute('href')).toContain('/NoticesToMariners/DownloadAllWeeklyZipFile?fileName=2022%20Wk%2026%20Weekly%20NMs.zip&batchId=51a04be3-8fac-485d-86e3-295da4f924c0&mimeType=application%2Fgzip&type=partner')
} 

  public async verifyIntegrationTestValueForDistributor()
  {
    const distributorFileName=await this.distributorFirstFileName.first().textContent();
    expect(distributorFileName).toEqual("26sect4");
    const distributorFileSize=await this.distributorFirstSize.first().textContent();
    expect(distributorFileSize).toEqual("91 KB (.rtf)");
    const publicFileNameFirst=await this.publicFirstFileName.last().textContent();
    expect(publicFileNameFirst).toEqual("23wknm22");
    const publicFileSizeFirst=await this.publicFirstSize.last().textContent();
    expect(publicFileSizeFirst).toEqual("2 MB (.pdf)");

  }

  public async verifySectionWithDotsCount()
  {
    const section = this.annualSection
    const countOfDots= await section.evaluateAll((matches: any[]) => { return matches.map(option => option.textContent) });
    let count=0;
    for(let i=0;i<countOfDots.length;i++)
   
      if(countOfDots[i]=="---")
      {
        count++;
      }
      
      expect(count).toEqual(3);

    
  }

  public async verifyAnnualFileNameLink()
  {
    const fileNameLink= await this.fileNameDownload;
    const fileNameData = await fileNameLink.evaluateAll((option:any[]) =>{return option.map(element => element.getAttribute('href'))}); 
    expect(fileNameData.length).toBeGreaterThan(0);
    expect(fileNameData).toBeTruthy();
  }

  public async verifyAnnualDownloadLink()
  {
    const annualdownload= await this.download;
    const annualdownloadLink= await annualdownload.evaluateAll((option:any[]) =>{return option.map(element => element.getAttribute('href'))}); 
    expect(annualdownloadLink.length).toBeGreaterThan(0);
    expect(annualdownloadLink).toBeTruthy();
  }

  public async checkAnnualFileSize()
 {
   await this.page.waitForLoadState();
   await this.page.waitForSelector("[id^='filesize']");
   
   const annualFileSizeData= await this.fileSize;
   const dailyFileSize= await annualFileSizeData.evaluateAll((option:any[]) =>{return option.map(element => element.textContent.trim())}); 
   const regex=/^\S+\sB|MB|KB|GB|$/; 
   for(let i=0;i<=dailyFileSize.length-1;i++)
   {
    const fileSize=dailyFileSize[i].split(" ");
    expect(fileSize[1].toString().match(regex)).toBeTruthy();
   }
}


}
