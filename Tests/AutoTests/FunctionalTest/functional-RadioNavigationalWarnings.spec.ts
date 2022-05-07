import { test } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';


test.describe("Create new radio navigational warnings record", ()=> {
  let radioNavigationalWarnings:RadioNavigationalWarnings;

  test.beforeEach(async ({page}) => {
    await page.goto(app.url); 
    radioNavigationalWarnings = new RadioNavigationalWarnings(page);
   await radioNavigationalWarnings.clickCreateRadioNavigationalWarningsRecord();
  });

  test('Without entered input fields',async()=>{ 
    await radioNavigationalWarnings.clickCreateButton();
   await radioNavigationalWarnings.getWarningErrorText('The Warning Type field is required.') ;
   await radioNavigationalWarnings.getRefrenceErrorText("The Reference field is required.");
   await radioNavigationalWarnings.getConTextErrorText('The Text field is required.');
   await radioNavigationalWarnings.getDateErrorText("The Date Time Group field is required.");
   await radioNavigationalWarnings.getDescriptionErrorText("The Description field is required."); 


 })  

 test('With content text as blank',async()=>{   
  await radioNavigationalWarnings.fillFormWithValidDetails("");  
  await radioNavigationalWarnings.clickCreateButton();
  await radioNavigationalWarnings.getConTextErrorText('The Text field is required.'); 
}) 


   test('With valid input details',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("testdata");  
    await radioNavigationalWarnings.clickCreateButton(); 
    await radioNavigationalWarnings.getDialogText('Record created successfully!')    
  } )
 
});
