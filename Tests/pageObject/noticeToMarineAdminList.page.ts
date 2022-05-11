
import { expect } from '@playwright/test';
import type { Page } from 'playwright';

export default class noticeToMarineList
{
    private page:Page;

    constructor(page:Page)
    {
        this.page = page; 
        this.warningType = this.page.locator("#WarningType");
        this.year= this.page.locator("#Year");
        this.filter = this.page.locator("#BtnFilter")
        this.createNewRecordText= this.page.locator("#BtnCreate");
    }
    
    public async checkEnabledWarningTypeDropDown()
    {
        return this.warningType.isenabled();
    }
    public async checkEnabledYearDropDown()
    {
        return this.year.isenabled();
    }

    public async checkCreateNewrecordText()
    {
        return this.createNewRecordText.innerText().toString();
    }

    public async getTableList()
    {
    const WarningTypeCount = (await this.page.$$("#WarningType option")).length;
    const YearCount = (await this.page.$$("#Year option")).length;
    
     for(var WarningType=0;WarningType<WarningTypeCount;WarningType++)
    {
    await this.warningType.selectOption({index:WarningType});

    for(var year=0;year<YearCount;year++)
    {
      await this.year.selectOption({index:week});
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

for(var WarningType=1;WarningType<WarningTypeCount;WarningType++)
    {
        await this.warningType.selectOption({index:WarningType});
        await this.filter.click();
        const fileSizeData = await this.page.$$("tbody tr td");
         
      for await (const table of fileSizeData)
      {
          console.log((await table.innerText()).toString());
          expect((await table.innerText()).toString().length).toEqual(20);
  
      }
    }
    for(var year=1;year<YearCount;year++)
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
}