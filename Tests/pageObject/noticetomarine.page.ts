
import type { Locator, Page } from 'playwright';



export default class noticetomarine
{
    private page:Page;
    readonly noticemarine:Locator;
    readonly dropdownyearly:Locator;
    readonly dropdownweekly:Locator;
    
    constructor(page:Page)
    {
        this.page = page; 
        this.noticemarine =this.page.locator("#navbarSupportedContent > ul > li:nth-child(1) > a");
        this.dropdownyearly = this.page.locator("#ddlYears");
        this.dropdownweekly = this.page.locator("#ddlWeeks");
        
    }

public async clicktonoticemarine()
{
   this.noticemarine.click();
}

public async NoticeMarinePageTheYearDropDownIsEnabled()
{
    const success=this.dropdownyearly.isEnabled();
    return success;
}

public async NoticeMarinePageTheweeklyDropDownIsEnabled()
{
    const success=this.dropdownweekly.isEnabled();
    return success;
}

public async noticemarinecrecordcount(year:string,week:string)
{
    this.dropdownyearly.selectOption("2022")
    this.dropdownweekly.selectOption("12");
    const tablerow =  this.page.$$("#divFilesList")
    return tablerow;
}






   

}