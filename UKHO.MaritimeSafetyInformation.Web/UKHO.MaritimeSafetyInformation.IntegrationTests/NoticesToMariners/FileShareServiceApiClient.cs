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
        static readonly IHttpClientFactory s_httpClient;
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
        public async Task<IResult<BatchAttributesSearchResponse>> BatchAttributeSearch(string baseUrl, string businessUnit, string productType)
        {
            string searchQuery = $"BusinessUnit eq '{businessUnit}' and $batch(Product Type) eq '{productType}' and $batch(Frequency) eq 'Weekly'";
            ////////FileShareApiClient fileShareApi = new(_httpClientFactory, , accessToken);
            ////////Search(searchQuery);
            ////////return await s_httpClient.SendAsync(httpRequestMessage);
            FileShareApiClient fileShareApi = new(s_httpClient, baseUrl, "");
            IResult<BatchAttributesSearchResponse> result = await fileShareApi.BatchAttributeSearch(searchQuery, CancellationToken.None);
            return result;
        }

        //////////public async Task<BatchAttributesSearchResponse> FssSearchAttributeAsync([FromBody] D365Payload d365Payload, string accessToken = null)
        //////////{
        //////////    string uri = $"{_apiHost}/api/subscription";
        //////////    string payloadJson = JsonConvert.SerializeObject(d365Payload);
        //////////    HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(accessToken, uri, payloadJson);

        //////////    return await s_httpClient.SendAsync(httpRequestMessage);
        //////////}
    }
}
