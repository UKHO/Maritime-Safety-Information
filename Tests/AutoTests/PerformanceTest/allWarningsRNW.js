// Creator: k6 Browser Recorder 0.6.2

import { sleep, group } from 'k6'
import http from 'k6/http'


import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";

export const options = { vus: 100, duration: '60m' }


export default function main() {
  let response
  
  group('page_1 - https://msi-dev-webapp.azurewebsites.net/RadioNavigationalWarnings', function () {
    response = http.get('https://msi-dev-webapp.azurewebsites.net/RadioNavigationalWarnings', {
      headers: {
        'upgrade-insecure-requests': '1',
        'sec-ch-ua': '" Not A;Brand";v="99", "Chromium";v="100", "Google Chrome";v="100"',
        'sec-ch-ua-mobile': '?0',
        'sec-ch-ua-platform': '"Windows"',
      },
    })
  })

  // Automatically added sleep
  sleep(1)
}

export function handleSummary(data) {
  console.log("Preparing the end-of-test summary...")
  return {
  ["summary/allWarningsRNW_Output" + new Date().toISOString().substr(0, 19).replace(/(:|-)/g, "").replace("T", "_") + ".html"]: htmlReport(data),
  stdout: textSummary(data, { indent: " ", enableColors: true })
  }
};