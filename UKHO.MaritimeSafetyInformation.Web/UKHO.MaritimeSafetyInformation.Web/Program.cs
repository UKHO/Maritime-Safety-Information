using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using UKHO.Logging.EventHubLogProvider;
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
                // Rhz : configure aspire resources with change to use service discovery. 
                builder.Configuration["FileShareService:BaseUrl"] = "https+http://mock-api/fssmsi/";
                builder.Configuration["CacheConfiguration:LocalConnectionString"] = builder.Configuration.GetConnectionString("local-table-connection");
                builder.Configuration["RadioNavigationalWarningsContext:ConnectionString"] = builder.Configuration.GetConnectionString("MSI-RNWDB-1");
            }

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Logging.AddAzureWebAppDiagnostics();
            // Rhz :
            // builder.Logging.AddEventHub();  //see AddCustomLogging below


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
            builder.Services.AddScoped<IWebhookService, WebhookService>(); 
            builder.Services.AddScoped<IEnterpriseEventCacheDataRequestValidator, EnterpriseEventCacheDataRequestValidator>();
            builder.Services.AddScoped<IMSIBannerNotificationService, MSIBannerNotificationService>();

            builder.Services.AddAllElasticApm(); //rhz new related to JT's change.

            builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddOptions();
            builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection("AzureAdB2C"));

            builder.Services.AddHttpContextAccessor(); //Rhz new

            builder.Services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(UkhoHeaderNames.XCorrelationId);
            });

            if (builder.Environment.IsDevelopment())
            {
                // Rhz : Add Mock Authentication Handler for and  optional mockconfig.json Development

                builder.Configuration.AddJsonFile("mockconfig.json", optional: true, reloadOnChange:true);

                builder.Services.AddAuthentication()
                        .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>("MockUser1", options => { })
                        .AddScheme<AuthenticationSchemeOptions, MockAuthHandlerDistro>("MockDistributorUser", options => { });

                //create a mock token acquisition service
                builder.Services.AddSingleton<ITokenAcquisition, MockTokenAcquisition>();

                //Login scheme selection based on LOCAL_USER_FLAG app setting defined in mockconfig.json
                var msiUserFlag = builder.Configuration["LOCAL_USER_FLAG"];
                if (!string.IsNullOrWhiteSpace(msiUserFlag))
                {
                    builder.Services.PostConfigure<AuthenticationOptions>(options =>
                    {
                        options.DefaultAuthenticateScheme = msiUserFlag;
                        options.DefaultChallengeScheme = msiUserFlag;
                    });
                }
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


            app.MapDefaultEndpoints();
            app.AddCustomLogging();  //Rhz new 

            app.UseCorrelationIdMiddleware();

            // Rhz : .UseErrorLogging(loggerFactory)
            // This will be added to CustomLogging extension later, not sure if needed.

            app.ConfigureRequestPipeline("Home", "Index");

            app.Run();
        }
    }
}
