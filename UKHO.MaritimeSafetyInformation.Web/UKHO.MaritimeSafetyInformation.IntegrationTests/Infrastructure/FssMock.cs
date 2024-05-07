using System.IO;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.Infrastructure
{
    public class FssMock
    {
        private readonly WireMockServer wireMockServer;

        public int Port { get; }

        public FssMock()
        {
            wireMockServer = WireMockServer.Start();
            Port = wireMockServer.Port;
        }

        public void Stop()
        {
            wireMockServer.Stop();
        }

        public void Reset()
        {
            wireMockServer.Reset();
        }

        public IRequestBuilder SetupSearch(string responseBodyResource, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/batch")
                .WithParam("$filter", "BusinessUnit eq 'MaritimeSafetyInformationIntegrationTest' and $batch(Product Type) eq 'Notices to Mariners'  and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '2022' and $batch(Week Number) eq '18'")
                .WithParam("limit", "100")
                .WithParam("start", "0");

            var responseBody = string.IsNullOrWhiteSpace(responseBodyResource) ? System.Array.Empty<byte>() : File.ReadAllBytes(responseBodyResource);

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
