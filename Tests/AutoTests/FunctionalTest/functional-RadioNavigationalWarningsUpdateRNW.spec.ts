import { expect, test } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';

test.describe("Create new radio navigational warnings record", ()=> {
  let radioNavigationalWarnings:RadioNavigationalWarnings;

  test.beforeEach(async ({page}) => {
    await page.goto(app.rnwAdminUrl); 
    radioNavigationalWarnings = new RadioNavigationalWarnings(page);  
  });

  test('Update With summary,refrence and content text as blank',async({page})=>{       
    await radioNavigationalWarnings.getEditUrl();
    await radioNavigationalWarnings.clearInput(radioNavigationalWarnings.description);
    await radioNavigationalWarnings.clearInput(radioNavigationalWarnings.reference);
    await radioNavigationalWarnings.clearInput(radioNavigationalWarnings.content);
    await radioNavigationalWarnings.editRNW();
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.summaryError,'The Description field is required.'); 
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.referenceEror,"The Reference field is required.");
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError,'The Text field is required.');
   
  }) 
 
   test('Update With valid input details with UK Coastal',async({page})=>{   
    await radioNavigationalWarnings.searchListWithfilter('UK Coastal');
    await radioNavigationalWarnings.getEditUrl();
    await radioNavigationalWarnings.isDelete();
    await radioNavigationalWarnings.fillEditFormWithValidDetails("testdata");  
    await radioNavigationalWarnings.editRNW(); 
    await radioNavigationalWarnings.getDialogText('Record updated successfully!')    
  })

   test('Update With valid input details with NAVAREA',async({page})=>{  
    await radioNavigationalWarnings.searchListWithfilter('NAVAREA 1');
    await radioNavigationalWarnings.getEditUrl();
    await radioNavigationalWarnings.isDelete();
    await radioNavigationalWarnings.fillEditFormWithValidDetails("testdata");  
    await radioNavigationalWarnings.editRNW(); 
    await radioNavigationalWarnings.getDialogText('Record updated successfully!')    
  })
});
  

  


