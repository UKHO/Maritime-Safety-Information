import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';
import * as app from "../Configuration/appConfig.json";

export default class Homepage {
  private page: Page;
  readonly siteTitle:Locator;
  readonly headingLevelOne:Locator;
  readonly admiralty:Locator;
  readonly navigationalinformation:Locator;
  readonly admiraltyDistributor:Locator;
  readonly gotoNoticeMariners:Locator;
  readonly gotoNavigationalWarnings:Locator;
  readonly ukHydrographic:Locator;
  readonly privacypolicylink:Locator;
  readonly accessibilitylink:Locator;

  
  constructor(page: Page) {
    this.page = page;
    this.siteTitle = this.page.locator('#siteTitle');
    this.headingLevelOne = this.page.locator('h1:has-text("Maritime Safety Information")');
    this.admiralty=this.page.locator('img[alt="Admiralty Maritime Data Solutions Home"]');
    this.navigationalinformation= this.page.locator('text=View the latest safety-critical navigational information');
    this.admiraltyDistributor= this.page.locator('text=If you are an ADMIRALTY Distributor, sign in to see more content');
    this.gotoNoticeMariners=this.page.locator('text=Go to Notices to Mariners');
    this.gotoNavigationalWarnings= this.page.locator('text=Go to Radio Navigation Warnings');
    this.ukHydrographic=this.page.locator('img[alt="UK Hydrographic Office Logo"]');
    this.privacypolicylink = this.page.locator('#privacypolicylink');
    this.accessibilitylink = this.page.locator('#accessibilitylink');
  }

 
  public async verifyAdmiraltyHomePage() 
  {
  expect(await this.siteTitle.innerText()).toContain('Maritime Safety Information');
  }

  public async verifyHomePageTitle() 
  {
  expect(await this.headingLevelOne.innerText()).toContain('Maritime Safety Information')
  } 
  public async verifyadmiralty()
  {
  expect(await this.admiralty.getAttribute('src')).toContain('/images/Admiralty-stacked-logo.svg');
  }
  public async verifypage()
  {
   expect(await this.gotoNoticeMariners.innerText()).toContain('Go to Notices to Mariners');
  expect(await this.gotoNavigationalWarnings.innerText()).toContain('Go to Radio Navigation Warnings'); 
  }
  public async verifyukHydrographic()
  {
  expect(await this.ukHydrographic.getAttribute('src')).toContain('/images/UKHO-stacked-logo.svg');
  }

  public async verifyprivacypolicy()
  {
  expect(await this.privacypolicylink.innerText()).toContain('Privacy and cookies');
  expect(await this.privacypolicylink.getAttribute('href')).toContain('https://www.admiralty.co.uk/cookie-policy');
  }
  public async verifyaccessibility()
  {
  expect(await this.accessibilitylink.innerText()).toContain('Accessibility');
  expect(await this.accessibilitylink.getAttribute('href')).toContain('AccessibilityStatement');
  }
}