import { test, expect, chromium, Page, Browser, BrowserContext } from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import RadioNavigationalWarnings from '../../pageObject/RadioNavigationalWarnings.page';
import RadioNavigationalWarningsList from '../../pageObject/RadioNavigationalWarningsAdminList.page';

test.describe("Goto maritime-safety-information Home Page", ()=> {
  let rnwList:RadioNavigationalWarningsList;

test.describe("Create new radio navigational warnings record", ()=> {
  let radioNavigationalWarnings:RadioNavigationalWarnings;

  test.beforeEach(async ({page}) => {
    await page.goto(app.rnwAdminUrl);
    radioNavigationalWarnings = new RadioNavigationalWarnings(page);
    await radioNavigationalWarnings.SelectRadioNavigationalWarning();
   rnwList=new RadioNavigationalWarningsList(page);
  });

  test('Without entered input fields',async()=>{ 
    await radioNavigationalWarnings.createRNW();
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.warningError,'The Warning Type field is required.') ;
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.referenceEror,"The Reference field is required.");
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError,'The Text field is required.');
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.datetimeError,"The Date Time Group field is required.");
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.summaryError,"The Description field is required."); 
  test('Does the Warning Type,Year Drop Down is enabled and Page Header Text,Create Record Text is Displayed',async () => {
   const WarningTypeEnable = await rnwList.checkEnabledWarningTypeDropDown();
   expect(WarningTypeEnable).toBeTruthy();
   const yearEnable = await rnwList.checkEnabledYearDropDown();
   expect(yearEnable).toBeTruthy();
   const createRecordList= await rnwList.checkCreateNewrecordText();
   expect(createRecordList).toBeTruthy();
   const pageHeader = await rnwList.checkPageHeaderText();
   expect(pageHeader).toEqual("Radio Navigational Warnings Admin List");
  })  

   test('With content text as blank',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("1","");  
    await radioNavigationalWarnings.createRNW();
    await radioNavigationalWarnings.checkErrorMessage(radioNavigationalWarnings.contentError,'The Text field is required.'); 
  test('Dose filter display search result for warning types as "UK Coastal" or  "NAVAREA 1" ', async () => {
    //search UK Coastal
    await rnwList.searchWithfilter('UK Coastal','2022');
    await rnwList.verifyTableHeader();
    await rnwList.verifyTableColumnWarningTypeData('UK Coastal');
    await rnwList.verifyTableContainsEditLink();

    //search NAVAREA 1
    await rnwList.searchWithfilter('NAVAREA 1','2022');
    await rnwList.verifyTableHeader();
    await rnwList.verifyTableColumnWarningTypeData('NAVAREA 1');
    await rnwList.verifyTableContainsEditLink();

  }) 
 
   test('With valid input details with Navarea',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("1","testdata");  
    await radioNavigationalWarnings.createRNW(); 
    await radioNavigationalWarnings.getDialogText('Record created successfully!')    
  
  test('Dose filter display search result sorted in descending order', async () => {
   await rnwList.searchWithfilter('UK Coastal','2022');
   await rnwList.verifyTableHeader();
   await rnwList.verifyTableDateColumnData('2022');

  })

   test('With valid input details with UK costal',async()=>{   
    await radioNavigationalWarnings.fillFormWithValidDetails("2","testdata");  
    await radioNavigationalWarnings.createRNW(); 
    await radioNavigationalWarnings.getDialogText('Record created successfully!')    
  test('Does the Table data is displayed with Pagination',async () => {
    await rnwList.searchWithfilter('UK Coastal','2022');
    await rnwList.verifyTableHeader();
    await rnwList.checkPaginationLink(rnwList.btnFirst);
    await rnwList.checkPaginationLink(rnwList.btnLast);
    await rnwList.checkPaginationLink(rnwList.btnNext);
    await rnwList.checkPaginationLink(rnwList.btnPrevious);
  })
 
});
