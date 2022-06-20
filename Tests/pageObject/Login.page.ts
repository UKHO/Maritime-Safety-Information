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
  constructor(page: Page) {
    this.page = page;
    this.signIn = this.page.locator('a:has-text("Sign in")');
    this.username = this.page.locator('[placeholder="Email"]');
    this.btnContinue = this.page.locator('[aria-label="Continue"]');
    this.password = this.page.locator('[placeholder="Password"]');
    this.btnLogin = this.page.locator('button:has-text("Sign in")');
    this.loginUsername = this.page.locator('text=Test User');
    this.signOut = this.page.locator('#navbarSupportedContent > ul > li.nav-item.dropdown > ul > li > a');
    this.emailError= this.page.locator('div.error.itemLevel.show');
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
        this.signOut.click(),
        expect((await this.signIn.innerText()).toString()).toContain('Sign in')  
      ]) 
     }
}




