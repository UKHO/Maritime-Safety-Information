using Azure.Data.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Local.Services;
using WireMock.Server;
using WireMock.Settings;

namespace UKHO.MaritimeSafetyInformation.Local
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServices();

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RadioNavigationalWarningsContext>();
            

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            var sqlTask = Task.Run(async () =>
            {
                logger.LogInformation("SQL container is starting");
                await SqlLDocker.StartContainerAsync();
                logger.LogInformation("SQL container is started");
            });

            var azuriteTask = Task.Run(async () =>
            {
                logger.LogInformation("Azurite container is starting");
                await AzuriteDocker.StartContainerAsync();
                logger.LogInformation("Azurite container is started");
            });

            await Task.WhenAll(sqlTask, azuriteTask);
            
            await SeedDataAsync(context);

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Port = 5001
            });

            FssMock.Start(server);

            Console.WriteLine($"Server started: {server.IsStarted}");
            Console.WriteLine($"Port: {server.Port}");
            Console.WriteLine($"URL: {server.Url}");

            await InitializeTableClientAsync();

            Console.ReadLine();

            // Stop the services when done
            await SqlLDocker.StopContainerAsync();
            await AzuriteDocker.StopContainerAsync();
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
                options.UseSqlServer(connectionString,
                    c => c.EnableRetryOnFailure()));

            // Add other services here if needed

            return serviceCollection.BuildServiceProvider();
        }
        
        static async Task SeedDataAsync(RadioNavigationalWarningsContext context)
        {
           // if (!await IsSqlServerAvailableAsync(context.Database.GetDbConnection().ConnectionString)) return;

            await context.Database.EnsureCreatedAsync();

            if (!context.WarningType.Any(w => w.Id == 1))
            {
                context.WarningType.Add(new WarningType { Name = "NAVAREA" });
                await context.SaveChangesAsync();
            }

            if (!context.WarningType.Any(w => w.Id == 2))
            {
                context.WarningType.Add(new WarningType { Name = "UK Coastal" });
                await context.SaveChangesAsync();
            }

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

           await context.SaveChangesAsync();
        }

        private static async Task InitializeTableClientAsync()
        {
            var storageAccountName = "devstoreaccount1";
            var storageAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
            var connectionString = $@"AccountName={storageAccountName};AccountKey={storageAccountKey};DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/{storageAccountName};QueueEndpoint=http://127.0.0.1:10001/{storageAccountName};TableEndpoint=http://127.0.0.1:10002/{storageAccountName};";

            var serviceClient = new TableServiceClient(connectionString);
            TableClient tableClient = serviceClient.GetTableClient("MsiBannerNotificationTable");
            await tableClient.CreateIfNotExistsAsync();
        }
    }
}
