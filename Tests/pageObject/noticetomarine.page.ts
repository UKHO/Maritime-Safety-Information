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
        this.noticeMarine =this.page.locator("text=Notices to Mariners");
        this.dropDownYearly = this.page.locator("#ddlYears");
        this.dropDownWeekly = this.page.locator("#ddlWeeks");
        this.fileName=this.page.locator('text=File Name');
        this.fileSize= this.page.locator('text=File Size');
    }

public async clickToNoticemarine()
{
 await this.noticeMarine.click();
}

public async checkEnabledYearDropDown()
{
          
  return await this.dropDownYearly.isEnabled();
    
}

public async checkEnabledWeekDropDown()
{
     return await this.dropDownWeekly.isEnabled();
    
}

public async getRecordCountTableNoticeToMarine(year:string,week:string)
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
  const yearlyCount = (await this.page.$$("#ddlYears option")).length;
  const weekCount = (await this.page.$$("#ddlWeeks option")).length;

  for(var year=1;year<yearlyCount;year++)
  {
    await this.dropDownYearly.selectOption({index:year});

    for(var week=1;week<weekCount;week++)
    {
      await this.dropDownWeekly.selectOption({index:week});
      
     await this.page.waitForSelector("td:nth-child(2)");
      const fileSizeData = await this.page.$$("#divFilesList > table > tbody >tr >td:nth-child(2)");
         
         for await (const table of fileSizeData)
         {
           var fileData = (await (await table.innerText()).toString()).split(" ");
           
           switch(fileData[1])
           {
             case "MB":
               {
                expect(fileData[1]).toContain("MB"); 
                break;
               }
               case "KB":
                {
                 expect(fileData[1]).toContain("KB"); 
                 break;
                }
                case "GB":
                  {
                   expect(fileData[1]).toContain("GB"); 
                   break;
                  }
                  case "B":
                    {
                     expect(fileData[1]).toContain("B"); 
                     break;
                    }
            }
         }    
         
        }
      }
   }     
}