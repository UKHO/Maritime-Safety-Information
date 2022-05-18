
import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';

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
       readonly pageHeaderText:Locator;
       constructor(page:Page)
       {
       this.page = page; 
       this.warningType = this.page.locator("#WarningType");
       this.year= this.page.locator("#Year");
       this.filter = this.page.locator("#BtnFilter");
       this.createNewRecordText= this.page.locator("text=Create New Warning");
       this.pageHeaderText=this.page.locator("text=Radio Navigational Warnings List");
       this.btnFirst= this.page.locator("#BtnFirst");
       this.btnPrevious =this.page.locator("#BtnPrevious");
       this.btnNext= this.page.locator("#BtnNext");
       this.btnLast =this.page.locator("#BtnLast");
       }
    
       public async checkEnabledWarningTypeDropDown()
       {
        return this.warningType.isEnabled();
       }
       public async checkEnabledYearDropDown()
       {
       return  this.year.isEnabled();
       }

       public async checkCreateNewrecordText()
       {
       return this.createNewRecordText.textContent();
       }

       public async checkPageHeaderText()
       {
       return await this.pageHeaderText.textContent();
       }

       public async checkEnabledFilterButton()
       {
       return await this.filter.isEnabled();
       }
    
       public async getTableList()
       {
       const warningTypeCount = (await this.page.$$("#WarningType option")).length;
       const yearCount = (await this.page.$$("#Year option")).length;

       for(var warningType=1;warningType<=warningTypeCount-1;warningType++)
       {
       await this.warningType.selectOption({index:warningType});
    
       for(var year=1;year<=yearCount-1;year++)
       {
       await this.year.selectOption({index:year});
       await this.filter.click();
      
       const fileSizeData = await this.page.$$("table > tbody > tr >td:nth-child(2)");
       const edit = await this.page.$$("table > tbody > tr >td:nth-child(9)");
      
       expect(fileSizeData.length).toBeLessThanOrEqual(20);
       
       for await (const tableEdit of edit)
       {
        expect((await tableEdit.innerText()).toString()).toEqual("Edit");
       }      
       }
       }
       }
       public async pagination(locator:Locator,text:string)
       {  
        expect((await (await locator.textContent()).toString())).toEqual(text);
       } 
       public async warningTypeData(option:string,text:string)
       {
        await this.warningType.selectOption(option);
        await this.filter.click();

        const fileSizeData = await this.page.$$("table > tbody > tr >td:nth-child(2)");
        for await (const table of fileSizeData)
        {
        expect((await table.innerText()).toString()).toEqual(text);
        }       
        } 
        public async yearData()
        {
        const yearCount = (await this.page.$$("#Year option")).length;
        for(var year=1;year<=yearCount-1;year++)
        {
        await this.year.selectOption({index:year});
        await this.filter.click();
        let newYear= await this.page.$eval<string, HTMLSelectElement>("#Year",ele => ele.value);
        newYear=newYear.slice(2,4);
        let fileSizeData = await this.page.$$("table > tbody > tr >td:nth-child(4)");
        
        for await (const table of fileSizeData)
        {
        let newTableData=(await table.innerText()).toString();
        newTableData=newTableData.slice(15,17);
        expect((await newTableData.toString())).toEqual(newYear);
        }  
      }
               
      } 

    

       


}