using System.IO;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.MockServices
{
    public class FssMock
    {
        private readonly WireMockServer wireMockServer;
        private readonly Configuration configuration;

        public int Port { get; }

        public FssMock(Configuration configuration)
        {
            wireMockServer = WireMockServer.Start();
            Port = wireMockServer.Port;
            this.configuration = configuration;
        }

        public void Stop()
        {
            wireMockServer.Stop();
        }

        public void Reset()
        {
            wireMockServer.Reset();
        }

        private static byte[] GetResponseBody(string responseBodyResource) => string.IsNullOrWhiteSpace(responseBodyResource) ? System.Array.Empty<byte>() : File.ReadAllBytes($"MockResources/{responseBodyResource}.json");

        /// <summary>
        /// Set up GET to /attributes/search path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file without extension stored under MockResources containing JSON data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupBatchAttributeSearch(string responseBodyResource, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/attributes/search")
                .WithParam("$filter", $"BusinessUnit eq '{configuration.BusinessUnit}' and $batch(Product Type) eq '{configuration.ProductType}' and $batch(Frequency) eq 'Weekly'")
                .WithParam("maxAttributeValueCount", configuration.MaxAttributeValuesCount.ToString());

            var responseBody = GetResponseBody(responseBodyResource);

            wireMockServer.Given(request).RespondWith(
                Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("content-type", "application/json")
                .WithBody(responseBody)
                );

            return request;
        }

        /// <summary>
        /// Set up GET to /batch path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file without extension stored under MockResources containing JSON data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="year"></param>
        /// <param name="weekNumber"></param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupSearch(string responseBodyResource, int year, int weekNumber, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/batch")
                .WithParam("$filter", $"BusinessUnit eq '{configuration.BusinessUnit}' and $batch(Product Type) eq '{configuration.ProductType}'  and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{weekNumber}'")
                .WithParam("limit", "100")
                .WithParam("start", "0");

            var responseBody = GetResponseBody(responseBodyResource);

            wireMockServer.Given(request).RespondWith(
                Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("content-type", "application/json")
                .WithBody(responseBody)
                );

            return request;
        }
    }
}
