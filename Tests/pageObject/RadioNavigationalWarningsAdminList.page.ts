import { expect } from '@playwright/test';
import { Locator, Page } from 'playwright';

export default class RadioNavigationalWarningsList
{
    private page:Page;
    readonly warningType:Locator;
    readonly year:Locator;
    readonly filter:Locator;
    readonly createNewRecordText:Locator;
    readonly btnFirst:Locator;
    readonly btnPrevious:Locator;
    readonly btnNext:Locator;
    readonly btnLast:Locator;
    readonly headerLocator:Locator;
    readonly tableHeader:Locator
    readonly tableHeaderText =['Sr No.','Warning Type','Reference','Date/Time','Description','Text','Expiry Date','Status','Action'];
    constructor(page:Page)
    {
        this.page = page; 
        this.warningType = this.page.locator("#WarningType");
        this.year= this.page.locator("#Year");
        this.filter = this.page.locator("#BtnFilter");
        this.createNewRecordText= this.page.locator("#BtnCreate");
        this.btnFirst= this.page.locator("#BtnFirst");
        this.btnPrevious =this.page.locator("#BtnPrevious");
        this.btnNext= this.page.locator("#BtnNext");
        this.btnLast =this.page.locator("#BtnLast");
        this.headerLocator=this.page.locator(".container-fluid >h1");
        this.tableHeader=this.page.locator(".table>thead>tr>th");
        
    }

    
    public async checkEnabledWarningTypeDropDown()
    {
        return this.warningType.isEnabled();
    }
    public async checkEnabledYearDropDown()
    {
        return this.year.isEnabled();
    }

    public async checkCreateNewrecordText()
    {
        return this.createNewRecordText.innerText().toString();
    }

    public async checkPageHeaderText()
    {
        return await this.headerLocator.textContent();
    }

    public async checkEnabledFilterButton()
    {
      return this.filter.isEnabled();
    }   
    
  public async checkPaginationLink(locator:Locator)
  {   
      expect(locator).toBeTruthy();
  }

  public async searchWithfilter(selectWarnings:string, selectYear:string)
  {

    await this.warningType.selectOption({ label: selectWarnings });
    await this.year.selectOption({ label: selectYear });
    await this.filter.click();
    await expect(this.tableHeader).toBeTruthy();
  }

  public async verifyTableHeader()
  {
    let tableColsHeader = await this.page.$$eval('.table>thead>tr>th', (options: any[]) => { return options.map(option => option.textContent.trim()) });
    tableColsHeader=tableColsHeader.filter(Boolean);
    var match = (this.tableHeaderText.length == tableColsHeader.length) && this.tableHeaderText.every(function (element, index) {
      return element === tableColsHeader[index];
    });

    expect(match).toBeTruthy();
  }

  public async verifyTableColumnWarningTypeData(expectedText:string)
  {
    const result= await this.page.$$eval('[id^="WarningTypeName"]' , (matches: any[]) => { return matches.map(option => option.textContent) });

    //fail if there are no matching selections
    expect(result.length).toBeGreaterThan(0);

    for(let i=0;i<result.length;i++)
       {
         expect(result[i].trim()).toEqual(expectedText);
       }

  }

  public async verifyTableDateColumnData(yearString:string)
  {
    const resultYear= await this.page.$$eval('[id^="DateTimeGroupRnwFormat"]' , (matches: any[]) => { return matches.map(option => option.textContent.trim().slice(-2)) });

    //fail if there are no matching selections
    expect(resultYear.length).toBeGreaterThan(0);

    //Verify records belongs to selected year
    for(let i=0;i<resultYear.length;i++)
       {
         expect(resultYear[i]).toEqual(yearString.slice(-2));
       }

    //Verify Dates are descending order   
    const resultdate= await this.page.$$eval('[id^="DateTimeGroupRnwFormat"]' , (matches: any[]) => { return matches.map(option => option.textContent.trim().slice(6)) });
    const sortedDesc = resultdate.sort((objA, objB) => objB.date - objA.date , );
    expect(sortedDesc).toBeTruthy();

  }

  public async verifyTableContainsEditLink()
  {
    const resultLinks= await this.page.$$eval('[id^="Edit"]' , (matches: any[]) => { return matches.map(option => option.textContent) });
    for(let i=0;i<resultLinks.length;i++)
    {
      expect(resultLinks[i].trim()).toEqual("Edit");
    }
  }

}

