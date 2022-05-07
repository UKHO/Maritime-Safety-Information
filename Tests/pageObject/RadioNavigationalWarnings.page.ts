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
    constructor(page:Page)
    {
        this.page = page; 
        this.reference = this.page.locator('#Reference');
        this.datetime= this.page.locator('#DateTimeGroup');
        this.description =  this.page.locator('#Summary');
        this.createNewRecord = this.page.locator('text=Create New');
        this.content=this.page.locator('#Content');
        this.create= this.page.locator('#btnCreate')
        this.warning = this.page.locator('#WarningType')
        this.warningError=  this.page.locator("#WarningType-error");
        this.referenceEror= this.page.locator("#Reference-error");
        this.datetimeError= this.page.locator("#DateTimeGroup-error");
        this.summaryError= this.page.locator("#Summary-error");
        this.contentError=this.page.locator("#Content-error");
    }

    public async clickCreateRadioNavigationalWarningsRecord()
    {
      await this.createNewRecord.click();
    }  
   
    public async clickCreateButton()
    {
     await  this.create.click();       
    }

    public async getDialogText(text:string)
    {
       await this.page.on('dialog',async(dialog)=>{
        expect(dialog.message()).toEqual(text);
        dialog.accept();
      })     
    }   

    public async getWarningErrorText(text:String)
    {
        await expect((await this.warningError.textContent()).toString()).toEqual(text);   
    }
    public async getRefrenceErrorText(text:String)
    {
        await expect((await this.referenceEror.textContent()).toString()).toEqual(text);   
    }

    public async getDateErrorText(text:String)
    {
         await expect((await this.datetimeError.textContent()).toString()).toEqual(text);   
    }
    public async getDescriptionErrorText(text:String)
    {
        await expect((await this.summaryError.textContent()).toString()).toEqual(text);   
    }
    public async getConTextErrorText(text:String)
    {
        await expect((await this.contentError.textContent()).toString()).toEqual(text);   
    }

    
    public async fillFormWithValidDetails(content:string)
    {
        await   this.warning.selectOption("1");
        await  this.reference.fill("reference");
        await this.page.waitForTimeout(3000);
        await  this.datetime.type("05072022 05533");
        await this.page.keyboard.press("ArrowDown")
        await  this.description.fill("testdata");
        await  this.content.fill(content);
    }
}