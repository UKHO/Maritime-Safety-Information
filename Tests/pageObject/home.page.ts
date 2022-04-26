
import type { Page } from 'playwright';

import * as data from "../configuration/pageObject.json";

export default class homepage
{
    private page:Page;
    constructor(page:Page)
    {
        this.page = page; 
    }

   
}