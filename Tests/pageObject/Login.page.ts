import { expect } from '@playwright/test';
import type { Locator, Page } from 'playwright';

export default class Login {
  private page: Page;
  readonly signIn: Locator;
  readonly username: Locator;
  readonly btnContinue: Locator;
  readonly password: Locator;
  readonly btnLogin: Locator;
  readonly loginUsername: Locator;
  readonly signOut: Locator;
  readonly emailError:Locator;
  readonly adUsername:Locator;
  readonly adNext:Locator;
  readonly adPassword:Locator;
  readonly login:Locator;
  readonly adYes:Locator;
  readonly adUserNameDropdown:Locator;
  readonly adSignOutText:Locator;
  constructor(page: Page) {
    this.page = page;
    this.signIn = this.page.locator('a:has-text("Sign in")');
    this.username = this.page.locator('[placeholder="Email"]');
    this.btnContinue = this.page.locator('[aria-label="Continue"]');
    this.password = this.page.locator('[placeholder="Password"]');
    this.btnLogin = this.page.locator('button:has-text("Sign in")');
    this.loginUsername = this.page.locator('text=Test User');
    this.signOut = this.page.locator('text=Sign out');
    this.emailError= this.page.locator('div.error.itemLevel.show');
    this.adUsername = this.page.locator('[placeholder="someone\\@ukho\\.gov\\.uk"]');
    this.adNext=  this.page.locator('text=Next');
    this.adPassword = this.page.locator('[placeholder="Password"]');
    this.login = this.page.locator('text=Sign in');
    this.adYes=this.page.locator('#idSIButton9');
    this.adUserNameDropdown=this.page.locator('#navbarDropdown');
   this.adSignOutText=this.page.locator("#navbarSupportedContent > ul > li > ul > li > a")

  }

  public async goToSignIn() {
    await this.signIn.click();
  }
 
  public async loginWithValidDetails(username: string, password: string) {
    await this.username.click();
    await this.username.fill(username);
    await this.btnContinue.click();
    await this.password.fill(password);
    await this.btnLogin.click();
    await this.loginUsername.click();
    expect((await this.loginUsername.innerText()).toString()).toContain("Test User");
     }

     public async signout()
     {
      await Promise.all([
        this.page.waitForLoadState(),
        this.loginUsername.click(),
        this.signOut.click(),
        this.page.waitForLoadState(),
        expect((await this.signIn.innerText()).toString()).toContain('Sign in')])
     }

     public async adSignout()
     {
      await Promise.all([
        this.page.waitForLoadState(),
        this.loginUsername.click(),
        this.signOut.click(),
      ])
    }

     public async adLogin(username: string, password: string)
     {
      await this.adUsername.fill(username);
      await Promise.all([
      this.page.waitForNavigation(),
      this.adNext.click()
      ]);
      await this.adPassword.fill(password);
      await this.login.click();
      await this.adYes.click();
      await this.adUserNameDropdown.click();
      expect(await this.adSignOutText.innerText().toString()).toContain('Sign out')
     }
}




