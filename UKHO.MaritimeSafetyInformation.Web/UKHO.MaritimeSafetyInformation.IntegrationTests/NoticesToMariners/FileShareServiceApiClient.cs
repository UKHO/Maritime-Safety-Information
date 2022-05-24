using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    public class FileShareServiceApiClient
    {
        ////////////static readonly IHttpClientFactory _httpClient;
        private readonly string _apiHost;

        public FileShareServiceApiClient(string apiHost)
        {
            _apiHost = apiHost;
        }

        /// <summary>
        /// Post Subscription request
        /// </summary>
        /// <param name="d365Payload"></param>
        /// <param name="accessToken">Access Token, pass NULL to skip auth header</param>
        /// <returns></returns>
        ////////public async Task<IResult<BatchAttributesSearchResponse>> BatchAttributeSearch( string businessUnit, string productType)
        ////////{
        ////////    //////string searchQuery = $"BusinessUnit eq '{businessUnit}' and $batch(Product Type) eq '{productType}' and $batch(Frequency) eq 'Weekly'";
        ////////    ////////need to access token as well
        ////////    //////string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjUzMzY4NzU4LCJuYmYiOjE2NTMzNjg3NTgsImV4cCI6MTY1MzM3MzE5NCwiYWNyIjoiMSIsImFpbyI6IkFaUUFhLzhUQUFBQXFhem9kdkRwT2w0UU1UMU45NXdudXRVQU9OOXREN1o2UCsyU1VKSXI3TVllekJQWDBNcGdydHV0U2NaM3I1UmdDRlRFMHo5clA1d1VDc0FLK2NObUdVbDNnQ2I1RGFiVWU5bUkvZ2Zmb1BWWmQ3SGFNdXhtZXVSSDAzWWFvN0VQemZSZGVwTC9HeDJjVGRRZ09rK3VEM1JOb3V4UWk3OVg2S016bko4OHJ1VGk1VktTWklQeUsyZEdBZ2xVWmlWTCIsImFtciI6WyJwd2QiLCJyc2EiLCJtZmEiXSwiYXBwaWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6IlNoaXJpbjE0OTI2QG1hc3Rlay5jb20iLCJmYW1pbHlfbmFtZSI6IlRhbGF3ZGVrYXIiLCJnaXZlbl9uYW1lIjoiU2hpcmluIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYWRkMWM1MDAtYTZkNy00ZGJkLWI4OTAtN2Y4Y2I2ZjdkODYxLyIsImlwYWRkciI6IjQ5LjMyLjIwNS4xMjciLCJuYW1lIjoiU2hpcmluIFRhbGF3ZGVrYXIiLCJvaWQiOiIzYmMxOWEzMS0wZDhmLTRmYjAtYmNlNy1jOTA5NzBjMDA4ZTkiLCJyaCI6IjAuQVZNQVNNbzBrVDFtQlVxV2lqR2tMd3J0UGlUZ1c0QUlvdnRBcTI4NW5DWkgwelFDQU9VLiIsInJvbGVzIjpbIkJhdGNoQ3JlYXRlIl0sInNjcCI6IlVzZXIuUmVhZCIsInN1YiI6IjNBaFJYQ0xLWXNkZkw0S0x2Vl9vTlJBS1dfdkJ1ZjY3bXJlU3BxcUpCaUkiLCJ0aWQiOiI5MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UiLCJ1bmlxdWVfbmFtZSI6IlNoaXJpbjE0OTI2QG1hc3Rlay5jb20iLCJ1dGkiOiI1WWNJV1JkajdFZWoyUWxaY0ZabEFBIiwidmVyIjoiMS4wIn0.j0ulCnh16A8YK7YxMoM9SS0KSLbOZmfHOv-hhVW5x9ibh8KDcvMCXU9PDz6dICG9xnXICCdJq9ybY3kIR9RJ0Rq6O_wukdLJLA4lMElLNbSgKpVqB1pgnAaBe_9TvvcOCnAWYhDREpZQiaQB4SCcaqPR7AhwYCBuvyVSNqm7Pqe8S6WyR2-zn27caZ9FvFGH6jP7A04F-YkKWIJw-wtBdHFtAK6yKmXFtDB3moGppsPO13Ssqryvrq-WCV_McGSg7CVml7FzqTY318K5L9gmLe9_vEjfl1CCjzrrQXR930E7FTwtdFGzxZBUW6vcabbye-9ulLk4SGuB8Gklkk8pXQ";
        ////////    //////////////FileShareApiClient fileShareApi = new(_httpClientFactory, , accessToken);
        ////////    //////////////Search(searchQuery);
        ////////    //////////////return await s_httpClient.SendAsync(httpRequestMessage);
        ////////    //////FileShareApiClient fileShareApi = new(_httpClient, _apiHost, token);
        ////////    //////IResult<BatchAttributesSearchResponse> result = await fileShareApi.BatchAttributeSearch(searchQuery, CancellationToken.None);
        ////////    return result;
        ////////}

        //////////public async Task<BatchAttributesSearchResponse> FssSearchAttributeAsync([FromBody] D365Payload d365Payload, string accessToken = null)
        //////////{
        //////////    string uri = $"{_apiHost}/api/subscription";
        //////////    string payloadJson = JsonConvert.SerializeObject(d365Payload);
        //////////    HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(accessToken, uri, payloadJson);

        //////////    return await s_httpClient.SendAsync(httpRequestMessage);
        //////////}
    }
}
