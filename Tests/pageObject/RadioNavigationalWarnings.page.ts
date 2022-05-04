
import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';



export default class RadioNavigationalWarnings
{
    private page:Page;
    readonly refrence:Locator;
    readonly datetime:Locator;
    readonly description:Locator;
    readonly createnewrecord:Locator;    
    readonly content:Locator;
    readonly create:Locator;
    readonly warning:string;
    constructor(page:Page)
    {
        this.page = page; 
        this.refrence = this.page.locator('#Reference');
        this.datetime= this.page.locator('#DateTimeGroup');
        this.description =  this.page.locator('#Summary');
        this.createnewrecord = this.page.locator('text=create');
        this.content=this.page.locator('#Content');
       this.create= this.page.locator('body > main > div > div.row > div > form > div:nth-child(7) > input')
       this.warning = '#WarningType';
    }

    public async creaternwrecord()
    {
        this.createnewrecord.click();
    }

    public async warningtypedropdown(text:string)
    {
        this.page.selectOption(this.warning,text);   
        
    }

    public async refrencetext(refercomment:string)
    {
       this.refrence.fill(refercomment);
    }

    public async datetimegroup(text:string)
    {
     this.datetime.fill(text)   
    }
    public async descriptiontext(text:string)
    {
        this.description.fill(text)
    }
    public async contenttext(text:string)
    {
       this.content.fill(text)
    }
    public async createbuttonclick()
    {
       this.create.click();       
    }

     public async dialog(text:string)
    {
        this.page.on('dialog',(dialog)=>{
        expect(dialog.message).toEqual(text);
        dialog.accept();
      })
}
}