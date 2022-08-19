import { sleep, group } from 'k6'
import http from 'k6/http'
import { SharedArray } from 'k6/data';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";

const config = JSON.parse(open('./appConfig.json'));




export const options = {
  scenarios: {
    MSIPages: {
           exec: 'msiHomePage',
           exec: 'noticedToMarinersDailyFiles',
           exec: 'noticeToMarinersWeeklyFiles',
           exec: 'allWarningsRNW',
           executor: 'ramping-vus',
          startVUs: 0,
          stages: [
            { duration: '30m', target: 5 },
            { duration: '25', target: 2 },
            { duration: '5m', target: 0 }
          ] 
          
      },
      NMDailyFiles: {
        exec: 'noticedToMarinersDailyFiles',
        executor: 'ramping-vus',
          startVUs: 0,
      stages: [
        { duration: '30m', target: 5 },
        { duration: '25m', target: 2 },
        { duration: '5m', target: 0 }
      ] 
      },
      NMWeeklyFiles: {
        exec: 'noticeToMarinersWeeklyFiles',
        executor: 'ramping-vus',
          startVUs: 0,
      stages: [
        { duration: '5m', target: 5 },
        { duration: '5m', target: 2 },
        { duration: '10m', target: 4 },
        { duration: '20m', target: 6 },
        { duration: '15m', target: 2 },
        { duration: '5m', target: 0 }
      ] 
      },
     
      DownloadNMWeeklyFiles: {
        exec: 'DownloadNMWeeklyFiles',
        executor: 'ramping-vus',
          startVUs: 0,
      stages: [
        { duration: '30m', target: 5 },
        { duration: '25m', target: 2 },
        { duration: '5m', target: 0 }
      ] 
       },
      DownloadNMDailyFiles: {
        exec: 'DownloadNMDailyFiles',
        executor: 'ramping-vus',
          startVUs: 0,
      stages: [
        { duration: '30m', target: 5 },
        { duration: '25m', target: 2 },
        { duration: '5m', target: 0 }
      ] 
    },
     NMCummulativePage: {
      exec: 'noticedToMarinersCumulative',
      executor: 'ramping-vus',
        startVUs: 0,
    stages: [
      { duration: '30m', target: 5 },
      { duration: '25m', target: 2 },
      { duration: '5m', target: 0 }
    
    ] 
  },
  NMAnnualPage: {
    exec: 'noticedToMarinersAnnual',
    executor: 'ramping-vus',
      startVUs: 0,
  stages: [
    { duration: '30m', target: 5 },
    { duration: '25m', target: 2 },
    { duration: '5m', target: 0 }
  ] 
},
NMLeisurePage: {
  exec: 'noticedToMarinersLeisure',
  executor: 'ramping-vus',
    startVUs: 0,
stages: [
  { duration: '30m', target: 5 },
  { duration: '25m', target: 2 },
  { duration: '5m', target: 0 }
      ] 
}
  }}

const batchDaily = new SharedArray('batchIdDailyURl', function () {
    return JSON.parse(open('./url.json')).batchIdDailyURl; 
    });

const batchWeely = new SharedArray('batchIdweeklyURl', function () {
     return JSON.parse(open('./url.json')).batchIdweeklyURl; 
              });

const weeklyFiles = new SharedArray('weeklyFileQueries', function () {
        return JSON.parse(open('./url.json')).weeklyFileQueries;

          });  
    

 export function msiHomePage()
  {
               
     http.get(config.url);
             
  }
  export function noticedToMarinersDailyFiles(){

    const dailyPage=`${config.url}/NoticesToMariners/Daily`;
    http.get(dailyPage);
  }
  
  export function noticeToMarinersWeeklyFiles(){
    
    const weekURL = weeklyFiles.length;
     
    for(var i=0 ; i<weekURL;i++)
    {
      const weekly=weeklyFiles[Math.floor(Math.random()*weeklyFiles.length)];
     http.post(`${config.url}/NoticesToMariners/Weekly?year=${weekly.year}&week=${weekly.week}`);
    
     
    }  

  }

  export function DownloadNMDailyFiles()
  {
    const dailyurldata = batchDaily.length;
    for(let i=0 ; i<dailyurldata;i++)
    {
      const dailyfile=batchDaily[Math.floor(Math.random()*batchDaily.length)];
     http.get(config.url,`NoticesToMariners/DownloadDailyFile?batchId=${dailyfile.batchid}&fileName=${dailyfile.fileName}&mimeType=application%2Fgzip`);
    
    }  
  }

  export function DownloadNMWeeklyFiles()
  
  {

    const weeklydata = batchWeely.length;
    for(let i=0 ; i<weeklydata;i++)
    {
      const weekfile=batchWeely[Math.floor(Math.random()*batchWeely.length)];
     http.get(config.url,`NoticesToMariners/DownloadFile?=${weekfile.fileName}&batchId=${weekfile.batchid}&mimeType=application%2Fpdf&frequency=Weekly`);
    
    }  
  }



  export function noticedToMarinersCumulative()
  {
    http.get(`${config.url}/NoticesToMariners/Cumulative`);
    
  } 

  export function noticedToMarinersAnnual()
  {
    http.get(`${config.url}/NoticesToMariners/Annual`);
  } 
  
  export function noticedToMarinersLeisure()
  {
    http.get(`${config.url}/NoticesToMariners/Leisure`);
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

