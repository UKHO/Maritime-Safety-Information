
import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';
import * as app from "../Configuration/appConfig.json"
import { DateTime } from 'luxon';


export default class RadioNavigationalWarningsListEndUser {
  private page: Page;
  readonly radioNavigationalWarningsPage: Locator;
  readonly radioNavigationalWarningsEndUser: Locator;
  readonly radioWarningEndUser: Locator;
  readonly aboutEndUser: Locator;
  readonly allWarningEndUser: Locator;
  readonly navAreaEndUser: Locator;
  readonly ukCostalEnduser: Locator;
  readonly tableHeader: Locator;
  readonly showSelection: Locator;
  readonly selectCheckBox: Locator;
  readonly btnShowSelection: Locator;
  readonly selectAll: Locator;
  readonly backToAllWarning: Locator;
  readonly refrence: Locator;
  readonly dateTimeGroupRnwFormat: Locator;
  readonly detailsReference: Locator;
  readonly detailsDateTimeGroupRnwFormat: Locator;
  readonly print: Locator;
  readonly viewDetails: Locator;
  readonly detailWarningType: Locator; 
  readonly about:Locator;
  readonly aboutRNW:Locator;
  readonly radioNavigationalWarnings;
  
  readonly tableHeaderText = ['Reference', 'Date Time Group', 'Description', 'Select all', 'Select'];

  constructor(page: Page) {
    this.page = page;
    this.radioNavigationalWarningsPage = this.page.locator('a:has-text("Radio Navigation Warnings")')
    this.radioNavigationalWarnings = this.page.locator('#navbarSupportedContent > ul > li:nth-child(2) > a')
    this.radioNavigationalWarningsEndUser = this.page.locator('#headingLevelOne')
    this.radioWarningEndUser = this.page.locator('text=Radio Warnings')
    this.aboutEndUser = this.page.locator('text=About')
    this.allWarningEndUser = this.page.locator('#allwarnings-tab')
    this.navAreaEndUser = this.page.locator('#NAVAREA1-tab')
    this.ukCostalEnduser = this.page.locator('#ukcoastal-tab')
    this.showSelection = this.page.locator('#showSelectionId')
    this.selectCheckBox = this.page.locator("[id^='checkbox'] > input")
    this.btnShowSelection = this.page.locator("#BtnShowSelection")
    this.selectAll = this.page.locator('#select_button')
    this.backToAllWarning = this.page.locator('text=Back to all warnings')
    this.refrence = this.page.locator('[id^="Reference"]')
    this.dateTimeGroupRnwFormat = this.page.locator('[id^="DateTimeGroupRnwFormat"]')
    this.detailsReference = this.page.locator('[id^="Details_Reference"]')
    this.detailsDateTimeGroupRnwFormat = this.page.locator('[id^="Details_DateTimeGroupRnwFormat"]')
    this.print = this.page.locator('#Print')
    this.viewDetails=this.page.locator('[id^="Viewdetails"] > button > span.view_details')
    this.detailWarningType=this.page.locator('[id^="Details_WarningType"]')
    this.about = this.page.locator('a:has-text("IHO WWNWS-SC")');
    this.aboutRNW = this.page.locator(" div >  p:nth-child(3)")
   
    
  }

  public async goToRadioWarning() {
    await this.radioNavigationalWarnings.click();
  }

  public async checkText(locator: Locator) {
    return await locator.innerText();
  }

  public async verifyTableDateColumnData() {
    const resultYear = await this.page.$$eval('[id^="DateTimeGroupRnwFormat"]', (matches: any[]) => { return matches.map(option => option.textContent.trim().slice(-2)) });

    //fail if there are no matching selections
    expect(resultYear.length).toBeGreaterThan(0);

    //Verify Dates are descending order   
    const resultdate = await this.page.$$eval('[id^="DateTimeGroupRnwFormat"]', (matches: any[]) => { return matches.map(option => option.textContent.trim().slice(6)) });
    const sortedDesc = resultdate.sort((objA, objB) => objB.date - objA.date,);
    expect(sortedDesc).toBeTruthy();

  }

  public async verifyTableContainsViewDetailsLink() {
    const resultLinks = await this.page.$$eval('[id^="Viewdetails"] > button > span.view_details', (matches: any[]) => { return matches.map(option => option.textContent.trim()) });

    for (let i = 0; i < resultLinks.length; i++) {
      expect(resultLinks[i].trim()).toEqual("View details");
    }
  }

  public async verifyTableHeader() {
    let tableColsHeader = await this.page.$$eval('.table>thead>tr>th', (options: any[]) => { return options.map(option => option.textContent.trim()) });
    let selectAllHeader = await this.selectAll.inputValue();
    tableColsHeader.splice(3, 0, selectAllHeader);
    tableColsHeader = tableColsHeader.filter(Boolean);
    var match = (this.tableHeaderText.length == tableColsHeader.length) && this.tableHeaderText.every(function (element, index) {
      return element === tableColsHeader[index];
    });
    expect(match).toBeTruthy();
  }

  public async verifyTableViewDetailsUrl() {
    const viewDetails = await this.page.$('[id^="Viewdetails"] > button > span.view_details');
    const beforeReference = await (await this.page.$('[id^="Reference"]')).innerText();
    const beforeDatetime = await (await this.page.$('[id^="DateTimeGroupRnwFormat"]')).innerText();
    const beforeViewDetails = await (await this.page.$('[id^="Viewdetails"] > button')).getAttribute("aria-expanded");
    expect(beforeViewDetails).toEqual("false");
    await viewDetails.click({ force: true });
    const newDetails = await (await this.page.$('[id^="Viewdetails"] > button')).getAttribute("aria-expanded");
    expect(newDetails).toBeTruthy();
    const afterReference = await (await this.page.$('[id^="Details_Reference"]')).innerText();
    const afterDateTime = await (await this.page.$('[id^="Details_DateTimeGroupRnwFormat"]')).innerText();
    expect(beforeReference).toEqual(afterReference);
    expect(beforeDatetime).toEqual(afterDateTime);
  }

  public async verifyImportantBlock() {
    
    const rnwHeader = (await this.page.locator("#rnwInfo > p").innerText()).toString();
    await this.ImportantBlock(rnwHeader)
  }

  public async ImportantBlock(rnwHeader:string){
    const rnwHeaderText = (await rnwHeader).split(":");

    const rnwMessageText = rnwHeaderText[0];
    expect(rnwMessageText).toContain("NAVAREA 1 and UK Coastal");

    const currentDateTime = new Date().getTime();
    const rnwDateTime = rnwHeaderText[1].replace('UTC', "").trim();

    const rnwModifiedDateTime = DateTime.fromFormat(rnwDateTime, "ddhhmm  MMM yy").toString();
    const lastModifiedDateTime = new Date(rnwModifiedDateTime);

    const compareDate = (currentDateTime, lastModifiedDateTime) => {
      if (lastModifiedDateTime < currentDateTime) {
        return true;
      }
      else { return false; }
    }
    expect(compareDate).toBeTruthy();
  }

  public async verifySelectOptionText() {
    expect(await this.selectAll.inputValue()).toEqual("Select all");
    let currentValue = await this.showSelection.getAttribute("value");
    expect(currentValue).toBe('');

    await this.selectAll.click({ force: true });
    expect(await this.selectCheckBox.first().isChecked()).toBeTruthy();
    currentValue = await this.showSelection.getAttribute("value");
    expect(currentValue).not.toBe('');

    await this.page.waitForLoadState('domcontentloaded')
    expect(await this.selectAll.inputValue()).toEqual("Clear all");
    await this.selectAll.click({ force: true });
    expect(await this.selectCheckBox.first().isChecked()).not.toBeTruthy();
    currentValue = await this.showSelection.getAttribute("value");
    expect(currentValue).toBe('');

    await this.page.waitForLoadState('domcontentloaded')
    expect(await this.selectAll.inputValue()).toEqual("Select all");
    await this.selectAll.click({ force: true });
  }

  public async verifySelectOptionCheckBox() {
    await this.selectAll.click({ force: true });
    expect(await this.selectCheckBox.first().isEnabled()).toBeTruthy();
    const detailsReference = await this.refrence.first().innerText();
    expect(detailsReference.length).toBeGreaterThan(0);
    const beforeDetailsReference = await (await this.refrence.first().innerText()).trim();
    const beforeDetailsDateTimeGroupRnwFormat = await (await this.dateTimeGroupRnwFormat.first().innerText()).trim();
    await this.selectCheckBox.first().click();
    await this.btnShowSelection.click();
    const afterDetailsReference = await (await this.detailsReference.first().innerText()).trim();
    const afterDetailsDateTimeGroupRnwFormat = await (await this.detailsDateTimeGroupRnwFormat.first().innerText()).trim();
    expect(beforeDetailsDateTimeGroupRnwFormat).toEqual(afterDetailsDateTimeGroupRnwFormat);
    expect(beforeDetailsReference).toEqual(afterDetailsReference);
    await this.backToAllWarning.click();
  }

  public async verifyPrint() {
    await this.selectCheckBox.first().click();
    await this.btnShowSelection.click();
    await this.page.waitForLoadState();
    expect(this.print.isEnabled()).toBeTruthy();
    expect((await this.print.innerText()).toString()).toContain("Print");
  
  }
  public async verifyNavareaAndUkCostalFilter(locator:Locator,text:string){
 
    await locator.click();
    await this.viewDetails.first().click();
    const detailWarnigType = await (await this.detailWarningType).first().innerText();
    expect(detailWarnigType).toContain(text) 
    const resultdate = await this.page.$$eval('[id^="DateTimeGroupRnwFormat"]', (matches: any[]) => { return matches.map(option => option.textContent.trim().slice(6)) })
    const sortedDesc = resultdate.sort((objA, objB) => objB.date - objA.date,);
    expect(resultdate).toEqual(sortedDesc);
    this.page.waitForTimeout(5000)
    const anchor= await locator.getAttribute("href");
    this.page.waitForTimeout(5000)
    const urlName=`${app.url}/RadioNavigationalWarnings${anchor}`;
    this.page.waitForTimeout(5000)
    expect(this.page.url()).toEqual(urlName);
    this.page.waitForTimeout(5000)

  }
 
  public async verifyAboutRnw()
  {
    await this.aboutEndUser.click();
    expect(await this.about.evaluate(option => option.getAttribute('href'))).toContain('https://iho.int/navigation-warnings-on-the-web')
  }

  public async verifyAboutRNWImportantBlock() {
    const rnwHeader = (await this.aboutRNW.last().innerText()).toString();
    await this.ImportantBlock(rnwHeader)
  }
}
