
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
      return dailyfileName; 
     }
   } 
     
   
    
   
