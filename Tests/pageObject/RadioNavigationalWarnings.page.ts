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
    constructor(page:Page)
    {
        this.page = page; 
        this.reference = this.page.locator('#Reference');
        this.datetime= this.page.locator('#DateTimeGroup');
        this.description =  this.page.locator('#Summary');
        this.createNewRecord = this.page.locator('text=create');
        this.content=this.page.locator('#Content');
        this.create= this.page.locator('#btnCreate')
        this.warning = this.page.locator('#WarningType');
    }

    public async clickCreateRadioNavigationalWarningsRecord()
    {
        this.createNewRecord.click();
    }  
   
    public async clickCreateButton()
    {
       this.create.click();       
    }

    public async getDialogText(text:string)
    {
        this.page.on('dialog',(dialog)=>{
        expect(dialog.message).toEqual(text);
        dialog.accept();
      })     
    }   
    
    public async fillFormWithValidDetails(content:string)
    {
        this.warning.selectOption("NavArea");
        this.reference.fill("reference");
        this.datetime.fill("05042022");
        this.description.fill("testdata");
        this.content.fill(content);
    }
}