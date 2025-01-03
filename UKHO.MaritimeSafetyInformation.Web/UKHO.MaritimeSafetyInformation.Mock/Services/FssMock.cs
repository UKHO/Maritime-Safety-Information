using WireMock.Server;

namespace UKHO.MaritimeSafetyInformation.Local.Services
{
    public static class FssMock
    {
        public static void Start(WireMockServer server)
        {
            var responses = new[]
            {
                ("Attributes.json", "/attributes/search"),
                ("AnnualFiles.json", "/batch"),
                ("DownloadFile.pdf", "/batch/*/files/*"),
                ("DownloadZipFile.zip", "/batch/*/files")
            };

            foreach (var (file, path) in responses)
            {
                var (data, contentType) = GetResponseBody(file);
                server
                    .Given(WireMock.RequestBuilders.Request.Create().WithPath(path).UsingGet())
                    .RespondWith(
                        WireMock.ResponseBuilders.Response.Create()
                            .WithBody(data)
                            .WithHeader("Content-Type", contentType)
                            .WithSuccess());
            }
        }

        private static string GetFilePath(string filename) => Path.Combine("MockResources", filename);

        private static string GetContentType(string filename) => Path.GetExtension(filename) switch
        {
            ".json" => "application/json",
            ".pdf" => "application/pdf",
            ".zip" => "application/x-zip",
            ".txt" => "application/txt",
            _ => "application/octet-stream",
        };

        private static (byte[] Data, string ContentType) GetResponseBody(string responseBodyResource)
        {
            if (string.IsNullOrWhiteSpace(responseBodyResource))
            {
                return (Array.Empty<byte>(), GetContentType(null));
            }

            var filePath = GetFilePath(responseBodyResource);
            return (File.ReadAllBytes(filePath), GetContentType(filePath));
        }
    }
}
