import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';

export default class RadioNavigationalWarnings
{
    
    private page:Page;
    readonly reference:Locator;
    readonly datetime:Locator;
    readonly description:Locator;
    readonly createNewRecord:Locator;    
    readonly content:Locator;
    readonly create:Locator;
    readonly warning:Locator;
    readonly warningError:Locator;
    readonly referenceEror:Locator;
    readonly datetimeError:Locator;
    readonly summaryError:Locator;
    readonly contentError:Locator;
    readonly editNewRecord:Locator;
    readonly warningType:Locator;
    readonly year:Locator;
    readonly filter:Locator;
    readonly delete:Locator;
    readonly edit:Locator;
    constructor(page:Page)
    {
        this.page = page; 
        this.reference = this.page.locator('#Reference');
        this.datetime= this.page.locator('#DateTimeGroup');
        this.description =  this.page.locator('#Summary');
        this.createNewRecord = this.page.locator('text=Create New');
        this.content=this.page.locator('#Content');
        this.create= this.page.locator('#btnCreate');
        this.warning = this.page.locator('#WarningType');
        this.warningError=  this.page.locator("#WarningType-error");
        this.referenceEror= this.page.locator("#Reference-error");
        this.datetimeError= this.page.locator("#DateTimeGroup-error");
        this.summaryError= this.page.locator("#Summary-error");
        this.contentError=this.page.locator("#Content-error");
        this.editNewRecord = this.page.locator("#btnEdit");
        this.warningType = this.page.locator("#WarningType");
        this.year= this.page.locator("#Year");
        this.filter = this.page.locator("#BtnFilter");
        this.delete = this.page.locator('#IsDeleted');
        this.edit=this.page.locator("[id^='Edit'] > a");
    }

    public async SelectRadioNavigationalWarning()
    {
        await this.createNewRecord.click();
        await expect(this.warning).toBeTruthy();
    }  
   
    public async createRNW()
    {
        await  this.create.click();
    }

    public async isDelete()
    {
        await this.delete.uncheck();
        await expect(this.warning).toBeTruthy();
    }

    public async getDialogText(text:string)
    {
        await this.page.on('dialog', async (dialog) => {
        expect(dialog.message()).toEqual(text);
        dialog.accept();
        })     
    }   

    public async checkErrorMessage(locator:Locator,text:String)
    {
        expect((await locator.textContent()).toString()).toEqual(text);   
    }
    
    
    public async fillFormWithValidDetails(warningType:string,content:string)
    {
        await this.warning.selectOption(warningType);
        await this.reference.fill("reference");
        await this.page.waitForTimeout(3000);
        await this.datetime.type("05072022 05533");
        await this.page.keyboard.press("ArrowDown");
        await this.description.fill("testdata");
        await this.content.fill(content);
    }
    public async clearInput(locator:Locator)
    {
        const input = await locator;
        await input.click({ clickCount: 3 })
        await this.page.keyboard.press('Control+A')   
        await this.page.keyboard.press('Backspace')     
    }
    public async editRNW()
    {
       await this.editNewRecord.click();
    }

    public async getEditUrl()
    {
        const newEdit = await this.edit.first();
        await newEdit.click(); 
    }
    public async pageLoad()
    {
        const [response] = await Promise.all([
           this.page.waitForNavigation(), 
           this.createNewRecord.click(),
           ]);
    }
    public async searchListWithfilter(selectWarnings:string)
    {
  
      await this.warningType.selectOption({ label: selectWarnings });
      await this.filter.click();
     
    }
    public async fillEditFormWithValidDetails(content:string)
    {
        await this.reference.fill("reference");
        await this.page.waitForTimeout(3000);
        await this.datetime.type("05072022 05533");
        await this.page.keyboard.press("ArrowDown");
        await this.description.fill("testdata");
        await this.content.type(content);
    }
  
}