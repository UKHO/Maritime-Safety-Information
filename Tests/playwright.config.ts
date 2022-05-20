import type { PlaywrightTestConfig } from '@playwright/test';
import { devices } from '@playwright/test';


const config: PlaywrightTestConfig = {
  testDir: './AutoTests',
  timeout: 30 * 1000,
  expect: {
    timeout: 10000
  },
  forbidOnly: !!process.env.CI,

  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [ ['junit', { outputFile: 'results.xml' }] ],
  use: {
    actionTimeout: 0,
    trace: 'on-first-retry',
    headless: false,
  },
  projects: [
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        
      },
    },

  ],

};

export default config;
