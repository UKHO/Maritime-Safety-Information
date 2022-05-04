
import type { Page } from 'playwright';



export default class RadioNavigationalWarnings
{
    private page:Page;
    constructor(page:Page)
    {
        this.page = page; 
    }

    public async creaternwrecord()
    {
        this.page.click('body > main > p > a')
    }

    public async warningtypedropdown()
    {
     this.page.selectOption('#WarningType','1');   
    }

    public async refrencetext()
    {
        this.page.locator('#Reference');
    }

    public async datetimegroup()
    {
        this.page.locator('#DateTimeGroup')
    }
    public async descriptiontext()
    {
        this.page.locator('#Summary')
    }
    public async contenttext()
    {
        this.page.locator('#Content')
    }
    public async createbuttonclick()
    {
        this.page.locator('body > main > div > div.row > div > form > div:nth-child(7) > input')
    }



   
}