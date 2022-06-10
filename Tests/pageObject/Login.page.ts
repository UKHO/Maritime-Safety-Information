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
  constructor(page: Page) {
    this.page = page;
    this.signIn = this.page.locator('a:has-text("Sign in")');
    this.username = this.page.locator('[placeholder="Email"]');
    this.btnContinue = this.page.locator('[aria-label="Continue"]');
    this.password = this.page.locator('[placeholder="Password"]');
    this.btnLogin = this.page.locator('button:has-text("Sign in")');
    this.loginUsername = this.page.locator('text=Test User');
    this.signOut = this.page.locator('text=Sign out');
  }

  public async goToSignIn() {
    await this.signIn.click();
  }
  public async loginwithBlankDetails(username: string,passowrd:string) {
    await this.username.fill(username);
    await this.btnContinue.click();
    expect((await this.page.locator('div.error.itemLevel.show').innerText()).toString()).toContain('Enter your email address')
    await this.username.fill('UKHODevTestUser1@outlook.com');
    await this.btnContinue.click();
    await this.password.fill(passowrd);
    await this.btnLogin.click();
    expect((await this.page.locator('text=Please enter your password').innerText()).toString()).toContain('Please enter your password');
  }
  
  public async loginWithValidDetails(username: string, passowrd: string) {
    await this.username.click();
    await this.username.fill(username);
    await this.btnContinue.click();
    await this.password.fill(passowrd);
    await this.btnLogin.click();
    await this.loginUsername.click();
    expect((await this.loginUsername.innerText()).toString()).toContain("Test User");
    await Promise.all([
      this.page.waitForLoadState(),
      this.signOut.click()]);
    expect((await this.signIn.innerText()).toString()).toContain('Sign in')
  }


}




