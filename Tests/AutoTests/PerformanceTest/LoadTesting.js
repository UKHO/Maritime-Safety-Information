import { sleep, group } from 'k6'
import http from 'k6/http'
import { SharedArray } from 'k6/data';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";

const config = JSON.parse(open('./appConfig.json'));


export const options = {
  scenarios: {
    DownloadNMFiles: {
          exec: 'msiHomePage',
          exec: 'noticedToMariners()',
          exec: 'noticedToMarinersDailyFiles',
          exec: 'DownloadNMDailyFiles',
          exec: 'DownloadNMWeeklyFiles',
          exec: 'allWarningsRNW',
          executor: 'per-vu-iterations',
          startTime: '5s',
          gracefulStop: '5s',
          vus: 540,
          iterations: 6,
          maxDuration: '1h'
          
      }
    }
  }

const batchDaily = new SharedArray('batchIdDailyURl', function () {
    return JSON.parse(open('./url.json')).batchIdDailyURl; 
    });

    const batchWeely = new SharedArray('batchIdweeklyURl', function () {
     return JSON.parse(open('./url.json')).batchIdweeklyURl; 
              });

 export function msiHomePage()
  {
               
     http.get(config.url);
             
  }
   export function noticedToMariners(){

    http.get(`${config.url}/NoticestoMariners`);
    
  }
            
  export function noticedToMarinersDailyFiles(){

    http.get(`${config.url}/NoticestoMariners/ShowDailyFiles`);
  }
  
  export function DownloadNMDailyFiles()
  {
    const dailyurldata = batchDaily[Math.floor(Math.random() * batchDaily.length)]; 
    for(let i=0 ; i<=dailyurldata.length-1;i++)
    {
     http.get(config.url,`NoticesToMariners/DownloadDailyFile?batchId=${batchDaily[i].batchid}&fileName=${batchDaily[i].fileName}&mimeType=application%2Fgzip`);
    }  
  }

  export function DownloadNMWeeklyFiles()
  {
    const weeklydata = batchWeely[Math.floor(Math.random() * batchWeely.length)]; 
    for(let i=0 ; i<=weeklydata.length-1;i++)
    {
     http.get(config.url,`NoticesToMariners/DownloadWeeklyFile?=${batchWeely[i].fileName}&batchId=${batchWeely[i].batchid}&mimeType=application%2Fpdf`);
    }  
  }
  

export function allWarningsRNW()
{

  http.get(`${config.url}/RadioNavigationalWarnings`);
      
}

export function handleSummary(data) {
    console.log("Preparing the end-of-test summary...")
    return {
    ["summary/LoadTesting" + new Date().toISOString().substr(0, 19).replace(/(:|-)/g, "").replace("T", "_") + ".html"]: htmlReport(data),
    stdout: textSummary(data, { indent: " ", enableColors: true })
    }
  };

