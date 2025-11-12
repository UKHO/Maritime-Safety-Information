using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Projects;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
var isInPipeline = IsRunningInPipeline();

var builder = DistributedApplication.CreateBuilder(args);

var mockApi = builder.AddProject<UKHO_ADDS_Mocks_MSI>("mock-api");

var storage = builder.AddAzureStorage("local-storage-connection").RunAsEmulator(
    azr =>
    {
        azr.WithDataVolume("local-storage");
        azr.WithContainerName("mocks-storage");
    });

var tableStorage = storage.AddTables("local-table-connection");

var sql = builder.AddSqlServer("local-sql")
    .WithDataVolume("local-msi-data-v4")
    .WithResetCommand()
    .OnResourceReady(static async Task (resource, @event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("1.Resource ready, create database if necessary");

        var connectionString = await resource.GetConnectionStringAsync() ??
                throw new InvalidOperationException("Connection string is not available.");

        var databaseName = resource.Databases.FirstOrDefault().Value ??
                throw new InvalidOperationException("Database name is not available.");

        connectionString += $";Initial Catalog={databaseName}";

        await SeedData(new SqlConnection(connectionString));

    });


var databaseName = "MSI-RNWDB-1";

var creationScript = $$"""
                        IF DB_ID('{{databaseName}}') IS NULL
                            CREATE DATABASE [{{databaseName}}];
                        GO

                        -- Use the database
                        USE [{{databaseName}}];
                        GO

                        IF OBJECT_ID(N'dbo.WarningType', N'U') IS NULL
                        BEGIN
                        CREATE TABLE [dbo].[WarningType]
                        (
                        [Id] INT IDENTITY(1,1)
                        ,[Name] VARCHAR(32) NOT NULL
                        CONSTRAINT [PK_WarningType] PRIMARY KEY CLUSTERED ([Id] ASC)
                        )
                        END
                        GO

                        IF OBJECT_ID(N'dbo.RadioNavigationalWarnings', N'U') IS NULL
                        BEGIN
                        CREATE TABLE [dbo].[RadioNavigationalWarnings] (
                        [Id] INT IDENTITY (1, 1), 
                        [WarningType] INT NOT NULL, 
                        [Reference] VARCHAR(32) NOT NULL, 
                        [DateTimeGroup] DATETIME NOT NULL, 
                        [Summary] VARCHAR(256) NOT NULL, 
                        [Content] VARCHAR(MAX) NOT NULL, 
                        [ExpiryDate] DATETIME NULL, 
                        [IsDeleted] BIT NOT NULL DEFAULT 0, 
                        [LastModified] DATETIME NOT NULL DEFAULT GETUTCDATE() INDEX [IDX_RadioNavigationalWarnings_LastModified] NONCLUSTERED, 
                        CONSTRAINT [PK_RadioNavigationalWarnings] PRIMARY KEY CLUSTERED ([Id] ASC) ,
                        CONSTRAINT [FK_WarningType] FOREIGN KEY ([WarningType]) REFERENCES [dbo].[WarningType]([Id]) ON DELETE NO ACTION )
                        END
                        GO

                        

                        
                        """;

var rnwDb = sql.AddDatabase(databaseName)
            .WithCreationScript(creationScript);
            

var mvcApp = builder.AddProject<Projects.UKHO_MaritimeSafetyInformation_Web>("ukho-msi-web")
    .WithReference(mockApi)
    .WaitFor(mockApi)
    .WithReference(rnwDb)
    .WaitFor(rnwDb)
    .WithReference(tableStorage)
    .WaitFor(tableStorage)
    .WithLoginCommand();
    

var mvcadminApp = builder.AddProject<UKHO_MaritimeSafetyInformationAdmin_Web>("ukho-msi-admin-web")
    .WithReference(rnwDb)
    .WaitFor(rnwDb)
    .WithEnvironment(callback =>
    {
        callback.EnvironmentVariables["RadioNavigationalWarningsAdminContext__ConnectionString"] = rnwDb.Resource.ConnectionStringExpression;
        callback.EnvironmentVariables["AzureAd__Instance"] = "https://login.microsoftonline.com/";
        callback.EnvironmentVariables["AzureAd__ClientId"] = "clientId";
        callback.EnvironmentVariables["AzureAd__TenantId"] = "tenant";
        callback.EnvironmentVariables["AzureAd__CallbackPath"] = "/signin-oidc";
        callback.EnvironmentVariables["AzureAd__RedirectBaseUrl"] = "localhost";
    });

if (isInPipeline)
{
    mvcApp.WithHttpEndpoint();
    mvcadminApp.WithHttpEndpoint();
}

builder.Build().Run();

bool IsRunningInPipeline()
{
    // Common environment variables for CI/CD pipelines
    var ci = Environment.GetEnvironmentVariable("CI");
    var tfBuild = Environment.GetEnvironmentVariable("TF_BUILD");
    var githubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");
    var azurePipeline = Environment.GetEnvironmentVariable("AGENT_NAME");

    return !string.IsNullOrEmpty(ci)
        || !string.IsNullOrEmpty(tfBuild)
        || !string.IsNullOrEmpty(githubActions)
        || !string.IsNullOrEmpty(azurePipeline);
}

static async Task SeedData(SqlConnection connection)
{
    var context = new RadioNavigationalWarningsContext(new DbContextOptionsBuilder<RadioNavigationalWarningsContext>().UseSqlServer(connection).Options);

    if (!context.WarningType.Any())
    {
        context.WarningType.Add(new WarningType { Name = "NAVAREA1" });
        context.WarningType.Add(new WarningType { Name = "UK Coastal" });
        await context.SaveChangesAsync();
    }

    if (context.RadioNavigationalWarnings.Any()) return;


    context.RadioNavigationalWarnings.AddRange(
        new RadioNavigationalWarning
        {
            WarningType = 1,
            Reference = "NAVAREA 1 TEST/22",
            DateTimeGroup = new DateTime(year: 2022, month: 7, day: 4, hour: 12, minute: 0, second: 0, kind: DateTimeKind.Utc),
            Summary = "SPACE WEATHER. BIG SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.",
            Content = @"NAVAREA
                         NAVAREA TEST/24
                         301040 UTC Dec 25
                         SPACE WEATHER.
                         SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.
                         RADIO AND SATELLITE NAVIGATION SERVICES MAY BE AFFECTED.",
            IsDeleted = false,
            LastModified = DateTime.UtcNow
        },
        new RadioNavigationalWarning
        {
            WarningType = 2,
            Reference = "UK Coastal TEST/22",
            DateTimeGroup = new DateTime(year: 2022, month: 7, day: 4, hour: 12, minute: 0, second: 0, kind: DateTimeKind.Utc),
            Summary = "HUMBER. RHZ HORNSEA 1 AND 2 WINDFARMS. TURBINE FOG SIGNALS INOPERATIVE.",
            Content = @"UK Coastal
                                     TEST/24
                                     301510 UTC Dec 25
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
