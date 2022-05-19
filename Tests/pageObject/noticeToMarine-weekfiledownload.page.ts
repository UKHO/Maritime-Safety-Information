
import type { Page } from 'playwright';



export default class noticeToMarineWeekDownload
{
    private page:Page;
    readonly noticeToMarine;
    readonly year;
    readonly week;
    constructor(page:Page)
    {
        this.page = page; 
        this.noticeToMarine = this.page.locator("text=Notices to Mariners");
        this.year = this.page.locator('#ddlYears');
        this.week =this.page.locator('#ddlWeeks')
    }

    public async goToNoticeToMarine()
    {
      await this.noticeToMarine.click();
    } 
    public async checkFileDownload()
    {
     await this.year.selectOption('2022');
     await this.week.selectOption('17');
     await this.page.waitForSelector('td:nth-child(1)');
     const result = this.page.$$eval('td:nth-child(1)', (options : any[]) => {return options.map( option=>option.textContent.trim())}); 
     return result; 
     }
   } 
     
   
    
   
