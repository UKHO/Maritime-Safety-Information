import { expect } from '@playwright/test';
import { errors, Locator, Page } from 'playwright';

export default class noticetoMarine
{
    private page:Page;
    readonly noticeMarine:Locator;
    readonly dropDownYearly:Locator;
    readonly dropDownWeekly:Locator;
    readonly fileName:Locator;
    readonly fileSize:Locator;
    constructor(page:Page)
    {
        this.page = page; 
        this.noticeMarine =this.page.locator("#navbarSupportedContent > ul > li:nth-child(1) > a");
        this.dropDownYearly = this.page.locator("#ddlYears");
        this.dropDownWeekly = this.page.locator("#ddlWeeks");
        this.fileName=this.page.locator('text=File Name');
        this.fileSize= this.page.locator('text=File Size');
    }

public async clickToNoticemarine()
{
 await this.noticeMarine.click();
}

public async noticeMarinePageTheYearDropDownIsEnabled()
{
    const success=await this.dropDownYearly.isEnabled();
    return success;
}

public async noticeMarinePageTheweeklyDropDownIsEnabled()
{
    const success=await this.dropDownWeekly.isEnabled();
    return success;
}

public async noticeMarinecRecordCount(year:string,week:string)
{
    await this.dropDownYearly.selectOption(year);
    await this.dropDownWeekly.selectOption(week);
    await this.page.waitForSelector('tr')
    const tablerow = await this.page.$$("tr");
    return tablerow.length;
}

public async getFileSizeText()
{
  return (await this.fileSize.textContent()).toString();   
}
public async getFileNameText()
{
    return (await this.fileName.textContent()).toString();   
}

public async getTableData()
{
    const fileSizeData = await this.page.$$("td:nth-child(2)");
         
         for (const table of fileSizeData)
         {
           var fileData = await (await table.innerText());
           if(fileData!="File Size")
           {
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
          else
          {
            throw new Error("No Element");
          }
        }
         }
}
}