import type { PlaywrightTestConfig } from '@playwright/test';
import { devices } from '@playwright/test';


const config: PlaywrightTestConfig = {
  testDir: './AutoTests',

    reporter:'junit',

 //  testDir: './tests/Accessebility Test',
 
  /* Maximum time one test can run for. */
  timeout: 30 * 1000,
   

  expect: {
    timeout: 10000
  },
  forbidOnly: !!process.env.CI,

  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  use: {
    actionTimeout: 0,
    trace: 'on-first-retry',
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
