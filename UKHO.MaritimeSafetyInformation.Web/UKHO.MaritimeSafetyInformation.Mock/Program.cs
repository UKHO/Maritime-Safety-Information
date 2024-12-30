using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using WireMock.Server;
using WireMock.Settings;

namespace UKHO.MaritimeSafetyInformation.Mock
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RadioNavigationalWarningsContext>();
            SeedData(context);

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("FileShareService API Mock");

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Port = 5001
            });

            ConfigureServerResponses(server);

            Console.WriteLine($"Server started: {server.IsStarted}");
            Console.WriteLine($"Port: {server.Port}");
            Console.WriteLine($"URL: {server.Url}");

            Console.ReadLine();

            // Stop the server when done
            server.Stop();
        }

        private static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Add logging
            serviceCollection.AddLogging(configure => configure.AddConsole());
            var connectionString = configuration.GetConnectionString("RadioNavigationalWarningsDb");

            // Add DbContext
            serviceCollection.AddDbContext<RadioNavigationalWarningsContext>(options =>
                options.UseSqlServer(connectionString));

            // Add other services here if needed

            return serviceCollection.BuildServiceProvider();
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

        private static void ConfigureServerResponses(WireMockServer server)
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

        private static void SeedData(RadioNavigationalWarningsContext context)
        {
            if (context.RadioNavigationalWarnings.Any()) return;

            context.RadioNavigationalWarnings.AddRange(
                new RadioNavigationalWarning
                {
                    WarningType = 1,
                    Reference = "NAVAREA I 240/24",
                    DateTimeGroup = DateTime.UtcNow,
                    Summary = "SPACE WEATHER. SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.",
                    Content = @"NAVAREA
                                     NAVAREA I 240/24
                                     301040 UTC Dec 24
                                     SPACE WEATHER.
                                     SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.
                                     RADIO AND SATELLITE NAVIGATION SERVICES MAY BE AFFECTED.",
                    IsDeleted = false,
                    LastModified = DateTime.UtcNow
                },
                new RadioNavigationalWarning
                {
                    WarningType = 2,
                    Reference = "WZ 897/24",
                    DateTimeGroup = DateTime.UtcNow,
                    Summary = "HUMBER. HORNSEA 1 AND 2 WINDFARMS. TURBINE FOG SIGNALS INOPERATIVE.",
                    Content = @"UK Coastal
                                     WZ 897/24
                                     301510 UTC Dec 24
                                     HUMBER.
                                     HORNSEA 1 AND 2 WINDFARMS.
                                     1.TURBINES T25 54-00.3N 001-36.7E, A16 53-50.0N 001-58.7E AND S16 53-59.4N 001-48.3E, FOG SIGNALS INOPERATIVE.
                                     2.CANCEL WZ 895.",
                    IsDeleted = false,
                    LastModified = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }
    }
}
