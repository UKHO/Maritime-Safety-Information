import type { PlaywrightTestConfig } from '@playwright/test';
import { devices } from '@playwright/test';


const config: PlaywrightTestConfig = {

    retries: 3,
    testDir: './AutoTests',

    /* Maximum time one test can run for. */
    timeout: 100 * 1000,

    /*  Maximum time expect() should wait for the condition to be met. */
    expect: {
        timeout: 10 * 1000
    },

    /* Sets action and navigation timeouts */
    use: {
        actionTimeout: 10 * 1000,
        navigationTimeout: 30 * 1000,
        trace: 'on-first-retry',
    },

    /* Maximum time for the entire test run */
    globalTimeout: 60 * 60 * 1000,

    /* control the maximum number of parallel worker processes - setting to one disables parallelism */
    workers: process.env.CI ? 1 : undefined,

    /* exit with an error if any tests or groups are marked as test.only() or test.describe.only() */
    forbidOnly: !!process.env.CI,

    /* aggregates results into usable format */
    reporter: [['junit', { outputFile: 'results.xml' }]],

    /* Manage which browsers toi test against */
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
