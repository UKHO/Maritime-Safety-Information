using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Extensions;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;
using UKHO.MaritimeSafetyInformation.Web.Filters;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var builder = WebApplication.CreateBuilder(args);


            // Do we actually need this here? (Rhz)  
            var kvServiceUri = builder.Configuration["KeyVaultSettings:ServiceUri"];
            if (!string.IsNullOrWhiteSpace(kvServiceUri))
            {
                builder.Configuration.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
            }

            //Enables Application Insights telemetry.
            builder.Services.AddApplicationInsightsTelemetry();

            builder.Logging.AddAzureWebAppDiagnostics();

            // Rhz : when Aspire is added.
            //builder.AddServiceDefaults();

            builder.Services.Configure<EventHubLoggingConfiguration>(builder.Configuration.GetSection("EventHubLoggingConfiguration"));
            builder.Services.Configure<RadioNavigationalWarningConfiguration>(builder.Configuration.GetSection("RadioNavigationalWarningConfiguration"));
            builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Constants.AzureAd);

            var msiDBConfiguration = new RadioNavigationalWarningsContextConfiguration();
            builder.Configuration.Bind("RadioNavigationalWarningsAdminContext", msiDBConfiguration);
            builder.Services.AddDbContext<RadioNavigationalWarningsContext>(options => options.UseSqlServer(msiDBConfiguration.ConnectionString));

            builder.Services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();
            builder.Services.AddScoped<IRNWService, RNWService>();
            builder.Services.AddScoped<IRNWRepository, RNWRepository>();
            builder.Services.AddScoped<IRNWDatabaseHealthClient, RNWDatabaseHealthClient>();

            builder.Services.AddAllElasticApm(); //rhz new related to JT's change.

            builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(UkhoHeaderNames.XCorrelationId);
            });
            builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection("AzureAd"));
            builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
            options => options.AccessDeniedPath = "/accessdenied");

            builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SaveTokens = true; // this saves the token for the downstream api
                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    context.ProtocolMessage.RedirectUri = builder.Configuration["AzureAd:RedirectBaseUrl"] + builder.Configuration["AzureAd:CallbackPath"];
                    await Task.FromResult(0);
                };
            });

            builder.Services.AddHttpClient();

            builder.Services.AddHealthChecks()
                .AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck")
                .AddCheck<RNWDatabaseHealthCheck>("RNWDatabaseHealthCheck");

            var app = builder.Build();
            //app.AddCustomLogging(ILoggerFactory factory);

            app.UseCorrelationIdMiddleware();
            //  .UseErrorLogging(loggerFactory)

            // Configure the HTTP request pipeline.
            app.ConfigureRequestPipeline("RadioNavigationalWarningsAdmin", "Index");

            //Rhz: start allow anonymous access for development purposes
            app.UseEndpoints(endpoints =>
            {
                if (app.Environment.IsDevelopment())
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                else
                    endpoints.MapControllers();
            });
            // Rhz: end

            app.Run();


        }


    }
}
