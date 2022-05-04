import { test } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';


test.describe("Create new radio navigational warnings record", ()=> {
  let radioNavigationalWarnings:RadioNavigationalWarnings;

  test.beforeEach(async ({page}) => {
    await page.goto(app.url); 
    radioNavigationalWarnings = new RadioNavigationalWarnings(page);
    radioNavigationalWarnings.clickCreateRadioNavigationalWarningsRecord();
  });

   test('With valid input details',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("testdata");  
    await radioNavigationalWarnings.clickCreateButton(); 
    await radioNavigationalWarnings.getDialogText('Record created successfully')    
  } )

  test('With invalid input as content text',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("testdata1");  
    await radioNavigationalWarnings.clickCreateButton();
    await radioNavigationalWarnings.getDialogText('Failed to create record.')    
 })  
});
