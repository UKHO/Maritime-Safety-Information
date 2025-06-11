using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using UKHO.ADDS.Mocks.MSI.Extensions;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Filters;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.Validation;
namespace UKHO.MaritimeSafetyInformation.Web
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var kvServiceUri = builder.Configuration["KeyVaultSettings:ServiceUri"];
            if (!string.IsNullOrWhiteSpace(kvServiceUri))
            {
                builder.Configuration.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
            }

            
            if (builder.Environment.IsDevelopment())
            {
                // Rhz : configure aspire resources 
                builder.AddMockClientConfig(resource: "mock-api", prefix: "fssmsi/");
                builder.Configuration["FileShareService:BaseUrl"] = builder.Configuration.GetConnectionString("mock-api-https");
                builder.Configuration["CacheConfiguration:LocalConnectionString"] = builder.Configuration.GetConnectionString("local-table-connection");
                builder.Configuration["RadioNavigationalWarningsContext:ConnectionString"] = builder.Configuration.GetConnectionString("MSI-RNWDB-1");
                // Rhz : configure aspire resources end.
            }

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Logging.AddAzureWebAppDiagnostics();
            // Rhz : not yet builder.Logging.AddEventHub();


            builder.AddServiceDefaults();

            builder.Services.Configure<EventHubLoggingConfiguration>(builder.Configuration.GetSection("EventHubLoggingConfiguration"));
            builder.Services.Configure<RadioNavigationalWarningConfiguration>(builder.Configuration.GetSection("RadioNavigationalWarningConfiguration"));
            builder.Services.Configure<FileShareServiceConfiguration>(builder.Configuration.GetSection("FileShareService"));
            builder.Services.Configure<RadioNavigationalWarningsContextConfiguration>(builder.Configuration.GetSection("RadioNavigationalWarningsContext"));
            builder.Services.Configure<CacheConfiguration>(builder.Configuration.GetSection("CacheConfiguration"));
            builder.Services.Configure<BannerNotificationConfiguration>(builder.Configuration.GetSection("BannerNotificationConfiguration"));
            builder.Services.Configure<AzureAdB2C>(builder.Configuration.GetSection("AzureAdB2C"));

            var msiDBConfiguration = new RadioNavigationalWarningsContextConfiguration();
            builder.Configuration.Bind("RadioNavigationalWarningsContext", msiDBConfiguration);
            builder.Services.AddDbContext<RadioNavigationalWarningsContext>(options => options.UseSqlServer(msiDBConfiguration.ConnectionString));

            builder.Services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();
            builder.Services.AddScoped<INMDataService, NMDataService>();
            builder.Services.AddScoped<IFileShareService, FileShareService>();
            if (builder.Environment.IsDevelopment())
            {
                // Rhz : Add Mock Token Provider for Development
                builder.Services.AddScoped<IAuthFssTokenProvider, MockAuthFssTokenProvider>();
            }
            else
            {
                builder.Services.AddScoped<IAuthFssTokenProvider, AuthFssTokenProvider>();
            }
            builder.Services.AddScoped<IRNWService, RNWService>();
            builder.Services.AddScoped<IRNWRepository, RNWRepository>();
            builder.Services.AddScoped<IRNWDatabaseHealthClient, RNWDatabaseHealthClient>();
            builder.Services.AddScoped<IFileShareServiceHealthClient, FileShareServiceHealthClient>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAzureTableStorageClient, AzureTableStorageClient>();
            builder.Services.AddScoped<IFileShareServiceCache, FileShareServiceCache>();
            builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();
            builder.Services.AddScoped<IWebhookService, WebhookService>();  // What is this fore? (Rhz)
            builder.Services.AddScoped<IEnterpriseEventCacheDataRequestValidator, EnterpriseEventCacheDataRequestValidator>();
            builder.Services.AddScoped<IMSIBannerNotificationService, MSIBannerNotificationService>();

            builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

            builder.Services.AddHttpContextAccessor();

            //Configuring appsettings section AzureAdB2C, into IOptions (Rhz MAY NOT NEED THIS HERE)
            builder.Services.AddOptions();
            builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection("AzureAdB2C"));

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //(Rhz Look at implimentation, is it needed, is it correct. )
            builder.Services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(CorrelationIdMiddleware.XCorrelationIdHeaderKey);  //Rhz this is bad
            });

            // Rhz : Add Mock Authentication Handler for Development
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddAuthentication("Mock")
                        .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>("Mock", options => { });

                //create a mock token acquisition service
                builder.Services.AddSingleton<ITokenAcquisition, MockTokenAcquisition>();

                builder.Services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Mock";
                    options.DefaultChallengeScheme = "Mock";
                });
            }
            else
            {
                builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    builder.Configuration.Bind("AzureAdB2C", options);
                    options.Events ??= new OpenIdConnectEvents();
                    options.SaveTokens = true; // this saves the token for the downstream api
                    options.Events.OnRedirectToIdentityProvider = async context =>
                    {
                        context.ProtocolMessage.RedirectUri = builder.Configuration["AzureAdB2C:RedirectBaseUrl"] + builder.Configuration["AzureAdB2C:CallbackPath"];
                        await Task.FromResult(0);
                    };
                    options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                    {
                        context.ProtocolMessage.PostLogoutRedirectUri = builder.Configuration["AzureAdB2C:RedirectBaseUrl"] + builder.Configuration["AzureAdB2C:SignedOutCallbackPath"];
                        await Task.FromResult(0);
                    };
                })
                .EnableTokenAcquisitionToCallDownstreamApi(new string[] { builder.Configuration["AzureAdB2C:Scope"] })
                .AddInMemoryTokenCaches();
            }

            builder.Services.AddHttpClient();
            builder.Services.AddHealthChecks()
                .AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck")
                .AddCheck<RNWDatabaseHealthCheck>("RNWDatabaseHealthCheck")
                .AddCheck<FileShareServiceHealthCheck>("FileShareServiceHealthCheck");


            var app = builder.Build();
            // Rhz : app.AddCustomLogging(ILoggerFactory factory);

            app.UseCorrelationIdMiddleware();
            // Rhz : .UseErrorLogging(loggerFactory)

            // Configure the HTTP request pipeline. This replaces ConfigureRequest extension.
            app.ConfigureRequestPipeline("Home","Index");


            if (app.Environment.IsDevelopment())
            {
                // Build test data
                SeedData(new SqlConnection(builder.Configuration.GetConnectionString("MSI-RNWDB-1"))).Wait();
            }

            app.Run();
        }

        

        internal static async Task SeedData(SqlConnection connection)
        {
            var context = new RadioNavigationalWarningsContext(new DbContextOptionsBuilder<RadioNavigationalWarningsContext>().UseSqlServer(connection).Options);

            if (!context.WarningType.Any())
            {
                context.WarningType.Add(new WarningType { Name = "NAVAREA" });
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
                    Summary = "SPACE WEATHER. BIG SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.",
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
                    Summary = "HUMBER. RHZ HORNSEA 1 AND 2 WINDFARMS. TURBINE FOG SIGNALS INOPERATIVE.",
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
    }
}
