
import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';

export default class RadioNavigationalWarningsListEndUser
{
    private page:Page;
    readonly radioNavigationalWarningsPage:Locator;
    readonly radioNavigationalWarningsEndUser:Locator;
    readonly radioWarningEndUser:Locator;
    readonly aboutEndUser:Locator;
    readonly allWarningEndUser:Locator;
    readonly navAreaEndUser:Locator;
    readonly ukCostalEnduser:Locator;
    readonly tableHeader:Locator;
    readonly tableHeaderText =['Reference','Date Time Group','Description','Select all','Select'];
    constructor(page:Page)
    {
         this.page = page; 
         this.radioNavigationalWarningsPage = this.page.locator('text=Radio Navigation Warnings')
         this.radioNavigationalWarningsEndUser = this.page.locator('#headingLevelOne')
         this.radioWarningEndUser = this.page.locator('text=Radio Warnings')
         this.aboutEndUser = this.page.locator('text=About')
         this.allWarningEndUser = this.page.locator('#allwarnings-tab')
         this.navAreaEndUser = this.page.locator('#NAVAREA1-tab')
         this.ukCostalEnduser = this.page.locator('#ukcoastal-tab')
         
    }

    public async goToRadioWarning()
    {
      await this.radioNavigationalWarningsPage.click();
    } 
    
    public async checkText(locator:Locator)
    {
     return await locator.innerText();
    }

    public async verifyTableDateColumnData()
    {
      const resultYear= await this.page.$$eval('[id^="DateTimeGroupRnwFormat"]' , (matches: any[]) => { return matches.map(option => option.textContent.trim().slice(-2)) });
  
      //fail if there are no matching selections
      expect(resultYear.length).toBeGreaterThan(0);
  
  
      //Verify Dates are descending order   
      const resultdate= await this.page.$$eval('[id^="DateTimeGroupRnwFormat"]' , (matches: any[]) => { return matches.map(option => option.textContent.trim().slice(6)) });
      const sortedDesc = resultdate.sort((objA, objB) => objB.date - objA.date , );
      expect(sortedDesc).toBeTruthy();  
    }
  
    public async verifyTableContainsViewDetailsLink()
    {
      const resultLinks= await this.page.$$eval('[id^="Viewdetails"]' , (matches: any[]) => { return matches.map(option => option.textContent) });
      for(let i=0;i<resultLinks.length;i++)
      {
        expect(resultLinks[i].trim()).toEqual("View details");
      }
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
  
  
    
   

}