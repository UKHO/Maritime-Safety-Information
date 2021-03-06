import { test} from '@playwright/test';
import * as app from "../../Configuration/appConfig.json";
import { checkA11y, injectAxe, Options } from 'axe-playwright';
import noticeToMarinerWeekDownload from '../../pageObject/noticeToMarine-weekfiledownload.page';


import loginPage from '../../pageObject/Login.page';


  test.describe("A11y tests", ()=> {
   const defaultCheckA11yOptions: Options = {
    axeOptions: {
      runOnly: {
        type: 'tag',
        values: ['wcag2aa'],
      },
      reporter: 'v2'
    },
    detailedReport: true,
    detailedReportOptions: { html: true }
  };
 
  test.beforeEach(async ({page}) => {
    await page.goto(app.url);    
    const noticeFileDownload =new noticeToMarinerWeekDownload(page);
    await noticeFileDownload.goToNoticeToMariner();
    await noticeFileDownload.goToLeisureFolios();
  });

  test('Notices to Mariners Leisure Folios page should be accessible', async ({page}) => {
   
    await injectAxe(page);
    
    await checkA11y(page, undefined, defaultCheckA11yOptions);
    
  })
 
});