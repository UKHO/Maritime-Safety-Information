import { test } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';


test.describe("Create new radio navigational warnings record", ()=> {
  let radioNavigationalWarnings:RadioNavigationalWarnings;

  test.beforeEach(async ({page}) => {
    await page.goto(app.rnwAdminUrl);
    radioNavigationalWarnings = new RadioNavigationalWarnings(page);
    await radioNavigationalWarnings.SelectRadioNavigationalWarning();
  });

  test('Without entered input fields',async()=>{ 
    await radioNavigationalWarnings.createRNW();
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.warningError,'The Warning Type field is required.') ;
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.referenceEror,"The Reference field is required.");
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError,'The Text field is required.');
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.datetimeError,"The Date Time Group field is required.");
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.summaryError,"The Description field is required."); 
  })  

   test('With content text as blank',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("1","");  
    await radioNavigationalWarnings.createRNW();
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError,'The Text field is required.'); 
  }) 
 
   test('With valid input details with Navarea',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("1","testdata");  
    await radioNavigationalWarnings.createRNW(); 
    await radioNavigationalWarnings.getDialogText('Record created successfully!')    
  })

   test('With valid input details with UK costal',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("2","testdata");  
    await radioNavigationalWarnings.createRNW(); 
    await radioNavigationalWarnings.getDialogText('Record created successfully!')    
  })
 
});