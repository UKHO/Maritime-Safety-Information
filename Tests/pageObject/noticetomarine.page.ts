import { expect } from '@playwright/test';
import { errors, Locator, Page } from 'playwright';
import * as app from "../Configuration/appConfig.json";

export default class noticetoMarine
{
    private page:Page;
    readonly noticeMarine:Locator;
    readonly dropDownYearly:Locator;
    readonly dropDownWeekly:Locator;
    readonly fileName:Locator;
    readonly fileSize:Locator;
    readonly menuNoticeToMarine:Locator;
    readonly menuValueAddedResellers:Locator;
    readonly menuAbout:Locator;
    readonly tabweekly:Locator;
    readonly tabdaily:Locator;
    readonly tabcumulative:Locator;
    readonly tabannual:Locator;
    readonly radioNavigationalWarnings: Locator;
    readonly navareatab: Locator;
    readonly ukcoastaltab:Locator;
    constructor(page:Page)
    {
     this.page = page; 
     this.noticeMarine =this.page.locator('a:has-text("Notices to Mariners")');
     this.radioNavigationalWarnings = this.page.locator('a:has-text("Radio Navigation Warnings")');
     this.dropDownYearly = this.page.locator("#ddlYears");
     this.dropDownWeekly = this.page.locator("#ddlWeeks");
     this.fileName=this.page.locator('#weekly >> text=File Name');
     this.fileSize= this.page.locator('#weekly >> text=File Size');
     this.menuNoticeToMarine = this.page.locator('#navbarSupportedContent >> text=Notices to Mariners');
     this.menuValueAddedResellers = this.page.locator('text=Value Added Resellers');
     this.menuAbout = this.page.locator('text=About');
     this.tabweekly = this.page.locator('#weekly-tab');
     this.tabdaily = this.page.locator("#daily-tab");
     this.tabcumulative = this.page.locator("#cumulative-tab");
     this.tabannual = this.page.locator("#annual-tab");
     this.navareatab = this.page.locator("#NAVAREA1-tab");
     this.ukcoastaltab = this.page.locator("#ukcoastal-tab");
     
    }
    
    public async clickToNoticemarine()
    {
     await this.noticeMarine.first().click();
    }

    public async clickToNoticemarineAbout()
    {
        await this.menuAbout.click();
    }
    public async clickToNoticemarineAnnual()
    {
     await this.tabannual.click();   
    }

    public async checkEnabledYearDropDown()
    {      
     return await this.dropDownYearly.isEnabled();   
    }

    public async checkEnabledWeekDropDown()
    {
     return await this.dropDownWeekly.isEnabled();
    }

    public async checkpageurl(url:string,title:string)
    {
        await expect(this.page).toHaveURL(`${app.url}`);
        await expect(this.page).toHaveTitle(title)
    }

    public async checkurl(locator:Locator,url:string,title:string)
    {
        
        await locator.click();
        expect(this.page).toHaveURL(`${app.url}/${url}`);
        expect(this.page).toHaveTitle(title)
    }
    public async checkNavareaUrl(locator:Locator,url:string,title:string)
    {
        await locator.click();
        await expect(this.page.url()).toContain(`${app.url}/${url}#navarea1`);
        await expect(this.page).toHaveTitle(title);
    }
    public async checkUkcoastalUrl(locator:Locator,url:string,title:string)
    {
        await locator.click();
        await expect(this.page.url()).toContain(`${app.url}/${url}#ukcoastal`);
        await expect(this.page).toHaveTitle(title);
    }
   
    public async checkText(locator:Locator)
    {
     return (await locator.innerText()).toString();
    }

    public async checkTableRecordCount()
    {
     const yearlylength = (await this.page.$$("#ddlYears option")).length; 
     await this.dropDownYearly.selectOption({index:1});
     const weeklength = (await this.page.$$("#ddlWeeks option")).length;
     await this.dropDownWeekly.selectOption({index:weeklength-1});
     const result= await this.page.$$eval('td[id^=filename]' , (matches: any[]) => { return matches.map(option => option.textContent) });
     return result.length;
    }
    public async checkFileSizeText()
    {
     return (await this.fileSize.textContent()).toString();   
    }
    public async checkFileNameText()
    {
     return (await this.fileName.textContent()).toString();   
    }
   
    public async verifyTableContainsDownloadLink()
    {
     const downloadLinks= await this.page.$$eval('td[id^=download] > a' , (matches: any[]) => { return matches.map(option => option.textContent.trim()) });
     for(let i=0;i<downloadLinks.length;i++)
     {
     expect(downloadLinks[i]).toEqual("Download");
     }
    }

    public async checkFileNameSort()
    {
     const yearlylength = (await this.page.$$("#ddlYears option")).length; 
     await this.dropDownYearly.selectOption({index:1});
     const weeklength = (await this.page.$$("#ddlWeeks option")).length;
     await this.dropDownWeekly.selectOption({index:weeklength-1});
      
     const fileNameData = await this.page.$$eval('td[id^=filename]' , (matches: any[]) => { return matches.map(option => option.textContent) });  
     const beforeSortFilename= fileNameData;
     fileNameData.sort();
     const afterSortFileName = fileNameData;
     expect(beforeSortFilename).toEqual(afterSortFileName);
    }

     public async checkFileSizeData()
     {
     await this.page.waitForLoadState();
     await this.page.waitForSelector("#ddlYears");   
     const yearlyCount = (await this.page.$$("#ddlYears option")).length;
 
     for(var year=1;year<=yearlyCount-1;year++)
     {
     await this.dropDownYearly.selectOption({index:year});
     const weekCount = (await this.page.$$("#ddlWeeks option")).length;

     for(var week=1;week<=1;week++)
     {
     
     await this.dropDownWeekly.selectOption({index:weekCount-1});
     const fileSizeData = await this.page.$$eval('td[id^=filesize]' , (matches: any[]) => { return matches.map(option => option.textContent) }); ;
     expect(fileSizeData.length).toBeGreaterThan(0); 
     expect(await this.checkFileNameText()).toEqual('File Name');
     expect(await this.checkFileSizeText()).toEqual('File Size');   
     let boolFileSize = false;  
     for await (const tableCell of fileSizeData)
     {
     var fileData =  await tableCell.trim().split(" ");
           
     switch(fileData[1])
     {
     case "MB":
     {
     boolFileSize=true;
     break;
     }
     case "KB":
     {
     boolFileSize=true;
     break;
     }
     case "GB":
     {
     boolFileSize=true;
     break;
     }
     case "B":
     {
     boolFileSize=true;  
     break;
     }
     }
     expect(boolFileSize).toBeTruthy();
     }   

     }
     }
     }     
}
