
import type { Locator, Page } from 'playwright';



export default class noticeToMarinerWeekDownload
{
    private page:Page;
    readonly noticeToMarine:Locator;
    readonly year:Locator;
    readonly week:Locator;
    readonly daily:Locator;
    constructor(page:Page)
    {
        this.page = page; 
        this.noticeToMarine = this.page.locator('a:has-text("Notices to Mariners")');
        this.year = this.page.locator('#ddlYears');
        this.week =this.page.locator('#ddlWeeks');
        this.daily = this.page.locator('a[role="tab"]:has-text("Daily")');
    }

    public async goToNoticeToMariner()
    {
      await this.noticeToMarine.click();
    } 

    public async goToDailyFile()
    {
      await this.daily.click();
    }
    public async checkFileDownload()
    {
     await this.year.selectOption('2022');
     await this.week.selectOption('17');
     await this.page.waitForSelector("[id^='filename']");
     const result = this.page.$$eval("[id^='filename']", (options : any[]) => {return options.map( option=>option.textContent.trim())}); 
     return result; 
     }

     public async checkDailyFileDownload()
     {
      await this.page.waitForSelector("[id^='filename']");
      const dailyfileName = this.page.$$eval("[id^='filename']", (options : any[]) => {return options.map( option=>option.textContent.trim())}); 
        
      if ((await dailyfileName).length > 0) {
        const fileName = dailyfileName[0];
        var dailyPageUrl = await (await (await this.page.$("[id^='download'] > a")).getAttribute('href')).trim().split("&");
        const newDailyPageUrl = dailyPageUrl[1].replace(/%20/g, " ");
        return newDailyPageUrl;
      }
      else {
        throw new Error("No download File Found")
      }

     }

     public async getFirstDailyFileName()
     {
      await this.page.waitForSelector("[id^='filename']");
      const dailyfileName = await this.page.$$eval("[id^='filename']", (options : any[]) => {return options.map( option=>option.textContent.trim())}); 
      const fileName = dailyfileName[0];
      return fileName;
     }
   } 
     
   
    
   
