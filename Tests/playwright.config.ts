import type { PlaywrightTestConfig } from '@playwright/test';
import { devices } from '@playwright/test';


const config: PlaywrightTestConfig = {
  testDir: './AutoTests',
<<<<<<< HEAD
    reporter: [ ['junit', { outputFile: 'a.xml' }] ],

 //  testDir: './tests/Accessebility Test',
 
  /* Maximum time one test can run for. */
  timeout: 30 * 1000,
=======
    timeout: 30 * 1000,
  reporter: [['junit', { outputFile: 'junit.xml' }]],
>>>>>>> ec53106fcc3ace770ab9e57d3a4a2159d238b7ee
  expect: {
    timeout: 10000
  },
  forbidOnly: !!process.env.CI,

  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
<<<<<<< HEAD
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
 // reporter: 'html',
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
=======
>>>>>>> ec53106fcc3ace770ab9e57d3a4a2159d238b7ee
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
