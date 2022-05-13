
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
    }

    public async goToCreateRadioNavigationalWarningsRecordList()
    {
      this.page.locator().click();
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
      return this.page.locator("").innerText().toString();
    }

    public async checkEnabledFilterButton()
    {
      return this.page.locator("").isEnabled();
    }
    
    public async getTableList()
    {
    const warningTypeCount = (await this.page.$$("#WarningType option")).length;
    const yearCount = (await this.page.$$("#Year option")).length;
    
     for(var warningType=0;warningType<warningTypeCount;warningType++)
    {
    await this.warningType.selectOption({index:warningType});
     
    for(var year=0;year<yearCount;year++)
    {
      await this.year.selectOption({index:year});
      await this.filter.click();
      
      const fileSizeData = await this.page.$$("");
      
      for await (const table of fileSizeData)
      {
          console.log((await table.innerText()).toString());
      }
    }
   
}

const editOption = await (await this.page.$$(""));

for (const editOptionList of editOption)
{
  expect(editOptionList.innerText().toString()).toContain("Edit");
}

for(var warningType=1;warningType<warningTypeCount;warningType++)
    {
        await this.warningType.selectOption({index:warningType});
        await this.filter.click();
        const fileSizeData = await this.page.$$("tbody tr td");
        expect((await fileSizeData.length)).toBeLessThanOrEqual(20);
      
      for await (const table of fileSizeData)
      {
          console.log((await table.innerText()).toString());
     
      }
      
    }
    for(var year=1;year<yearCount;year++)
    {
      await this.year.selectOption({index:year});
      await this.filter.click();
      const fileSizeData = await this.page.$$("");
      expect(await fileSizeData.length).toEqual(20);
      for await (const table of fileSizeData)
      {
          console.log((await table.innerText()).toString());
      }
    }
  }
  
  public async pagination(locator:Locator)
  {   
      expect(locator.isEnabled()).toBeTruthy();
  }
 
}