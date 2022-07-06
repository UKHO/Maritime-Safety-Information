import type { PlaywrightTestConfig } from '@playwright/test';
import { devices } from '@playwright/test';


const config: PlaywrightTestConfig = {
  testDir: './AutoTests',
  timeout: 30 * 3000,
  expect: {
    timeout: 10000
  },
  fullyParallel:true,
  forbidOnly: !!process.env.CI,

  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [ ['junit', { outputFile: 'results.xml' }] ],
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
     {
      name: 'firefox',
      use: {
        ...devices['Desktop Firefox'],
        
      },
    },
     {
      name: 'edge',
      use: {
        ...devices['Desktop Edge'],
        
      },
    },
    {
      name: 'safari',
      use: {
        ...devices['Desktop Safari'],
        
      },
    },
    
    

  ],

};

export default config;
