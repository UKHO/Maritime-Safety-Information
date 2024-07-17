using System;
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

        public string BaseUrl { get; }

        public FssMock(Configuration configuration)
        {
            wireMockServer = WireMockServer.Start();
            BaseUrl = wireMockServer.Url;
            this.configuration = configuration;
        }

        public void Stop()
        {
            wireMockServer.Stop();
        }

        private static (byte[] Data, string ContentType) GetResponseBody(string responseBodyResource)
        {
            if (string.IsNullOrWhiteSpace(responseBodyResource))
            {
                return (Array.Empty<byte>(), GetContentType(null));
            }
            else
            {
                var filePath = GetFilePath(responseBodyResource);
                return (File.ReadAllBytes(filePath), GetContentType(filePath));
            }
        }

        private static string GetFilePath(string filename) => $"MockResources/{filename}";

        private static string GetContentType(string filename) => Path.GetExtension(filename) switch
        {
            ".json" => "application/json",
            ".pdf" => "application/pdf",
            ".zip" => "application/x-zip",
            ".txt" => "application/txt",
            _ => "application/octet-stream",
        };

        private void BuildResponse(IRequestBuilder request, string responseBodyResource, int statusCode)
        {
            var (data, contentType) = GetResponseBody(responseBodyResource);

            wireMockServer.Given(request).RespondWith(
                Response.Create()
                .WithStatusCode(statusCode)
                .WithHeader("content-type", contentType)
                .WithBody(data)
                );
        }

        /// <summary>
        /// Set up GET to /attributes/search path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file stored under MockResources containing data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupBatchAttributeSearchWeekly(string responseBodyResource, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/attributes/search")
                .WithParam("$filter", $"BusinessUnit eq '{configuration.BusinessUnit}' and $batch(Product Type) eq '{configuration.ProductType}' and $batch(Frequency) eq 'Weekly'")
                .WithParam("maxAttributeValueCount", configuration.MaxAttributeValuesCount.ToString());
            BuildResponse(request, responseBodyResource, statusCode);
            return request;
        }

        /// <summary>
        /// Set up GET to /batch path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file stored under MockResources containing data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="year"></param>
        /// <param name="weekNumber"></param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupSearchWeekly(string responseBodyResource, int year, int weekNumber, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/batch")
                .WithParam("$filter", $"BusinessUnit eq '{configuration.BusinessUnit}' and $batch(Product Type) eq '{configuration.ProductType}'  and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{weekNumber}'")
                .WithParam("limit", "100")
                .WithParam("start", "0");
            BuildResponse(request, responseBodyResource, statusCode);
            return request;
        }

        /// <summary>
        /// Set up GET to /batch path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file stored under MockResources containing data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupSearchDaily(string responseBodyResource, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/batch")
                .WithParam("$filter", $"BusinessUnit eq '{configuration.BusinessUnit}' and $batch(Product Type) eq '{configuration.ProductType}'  and $batch(Frequency) eq 'Daily' and $batch(Content) eq null ")
                .WithParam("limit", "100")
                .WithParam("start", "0");
            BuildResponse(request, responseBodyResource, statusCode);
            return request;
        }

        /// <summary>
        /// Set up GET to /batch path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file stored under MockResources containing data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupSearchCumulative(string responseBodyResource, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/batch")
                .WithParam("$filter", $"BusinessUnit eq '{configuration.BusinessUnit}' and $batch(Product Type) eq '{configuration.ProductType}'  and $batch(Frequency) eq 'Cumulative'")
                .WithParam("limit", "100")
                .WithParam("start", "0");
            BuildResponse(request, responseBodyResource, statusCode);
            return request;
        }

        /// <summary>
        /// Set up GET to /batch path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file stored under MockResources containing data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupSearchAnnual(string responseBodyResource, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath("/batch")
                .WithParam("$filter", $"BusinessUnit eq '{configuration.BusinessUnit}' and $batch(Product Type) eq '{configuration.ProductType}'  and $batch(Frequency) eq 'Annual'")
                .WithParam("limit", "100")
                .WithParam("start", "0");
            BuildResponse(request, responseBodyResource, statusCode);
            return request;
        }

        /// <summary>
        /// Set up GET to /batch/{batchId}/files/{filename} path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file stored under MockResources containing data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="batchId">A test BatchId GUID.</param>
        /// <param name="filename">A test filename. This doesn't have to match the responseBodyResource.</param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupDownloadFile(string responseBodyResource, string batchId, string filename, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath($"/batch/{batchId}/files/{filename}");
            BuildResponse(request, responseBodyResource, statusCode);
            return request;
        }

        /// <summary>
        /// Set up GET to /batch/{batchId}/files path.
        /// </summary>
        /// <param name="responseBodyResource">The name of the file stored under MockResources containing data for the mock endpoint to return. Use null or empty for no data.</param>
        /// <param name="batchId">A test BatchId GUID.</param>
        /// <param name="statusCode">The status code for the mock endpoint to return.</param>
        /// <returns></returns>
        public IRequestBuilder SetupDownloadZipFile(string responseBodyResource, string batchId, int statusCode = 200)
        {
            var request = Request.Create()
                .UsingGet()
                .WithPath($"/batch/{batchId}/files");
            BuildResponse(request, responseBodyResource, statusCode);
            return request;
        }
    }
}
