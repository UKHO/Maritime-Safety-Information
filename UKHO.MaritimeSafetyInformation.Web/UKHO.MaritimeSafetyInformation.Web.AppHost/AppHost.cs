using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mockApi = builder.AddProject<UKHO_ADDS_Mocks_MSI>("mock-api");

var storage = builder.AddAzureStorage("local-storage-connection").RunAsEmulator(
    azr =>
    {
        azr.WithDataVolume("local-storage");
    });

var tableStorage = storage.AddTables("local-table-connection");

var sql = builder.AddSqlServer("local-sql")
    .WithDataVolume("local-msi-data")
    .WithResetCommand();

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


builder.AddProject<Projects.UKHO_MaritimeSafetyInformation_Web>("ukho-msi-web")
    .WithReference(mockApi)
    .WaitFor(mockApi)
    .WithReference(rnwDb)
    .WaitFor(rnwDb)
    .WithReference(tableStorage)
    .WaitFor(tableStorage)
    .WithHttpsEndpoint();

var mvcadminApp = builder.AddProject<UKHO_MaritimeSafetyInformationAdmin_Web>("ukho-msi-admin-web")
    .WithReference(rnwDb)
    .WaitFor(rnwDb)
    .WithHttpsEndpoint()
    .WithEnvironment(callback =>
    {
        callback.EnvironmentVariables["RadioNavigationalWarningsAdminContext__ConnectionString"] = rnwDb.Resource.ConnectionStringExpression;
        callback.EnvironmentVariables["AzureAd__Instance"] = "https://login.microsoftonline.com/";
        callback.EnvironmentVariables["AzureAd__ClientId"] = "clientId";
        callback.EnvironmentVariables["AzureAd__TenantId"] = "tenant";
        callback.EnvironmentVariables["AzureAd__CallbackPath"] = "/signin-oidc";
        callback.EnvironmentVariables["AzureAd__RedirectBaseUrl"] = "localhost";
    });

builder.Build().Run();
