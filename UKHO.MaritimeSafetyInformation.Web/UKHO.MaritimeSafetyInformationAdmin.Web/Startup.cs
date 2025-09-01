using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using UKHO.Logging.EventHubLogProvider;
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
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup(IWebHostEnvironment env)
        {
            configuration = BuildConfiguration(env);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Enables Application Insights telemetry.
            services.AddApplicationInsightsTelemetry();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddAzureWebAppDiagnostics();
            });
            services.Configure<EventHubLoggingConfiguration>(configuration.GetSection("EventHubLoggingConfiguration"));
            services.Configure<RadioNavigationalWarningConfiguration>(configuration.GetSection("RadioNavigationalWarningConfiguration"));
            services.AddMicrosoftIdentityWebAppAuthentication(configuration, Constants.AzureAd);
            var msiDBConfiguration = new RadioNavigationalWarningsContextConfiguration();
            configuration.Bind("RadioNavigationalWarningsAdminContext", msiDBConfiguration);
            services.AddDbContext<RadioNavigationalWarningsContext>(options => options.UseSqlServer(msiDBConfiguration.ConnectionString));

            services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();
            services.AddScoped<IRNWService, RNWService>();
            services.AddScoped<IRNWRepository, RNWRepository>();
            services.AddScoped<IRNWDatabaseHealthClient, RNWDatabaseHealthClient>();

            services.AddAllElasticApm();

            services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(UkhoHeaderNames.XCorrelationId);
            });
            services.Configure<OpenIdConnectOptions>(configuration.GetSection("AzureAd"));
            services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
            options => options.AccessDeniedPath = "/accessdenied");

            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SaveTokens = true; // this saves the token for the downstream api
                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    context.ProtocolMessage.RedirectUri = configuration["AzureAd:RedirectBaseUrl"] + configuration["AzureAd:CallbackPath"];
                    await Task.FromResult(0);
                };
            });

            services.AddHttpClient();

            services.AddHealthChecks()
                .AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck")
                .AddCheck<RNWDatabaseHealthCheck>("RNWDatabaseHealthCheck");
            services.AddApplicationInsightsTelemetry();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              ILoggerFactory loggerFactory,
                              IHttpContextAccessor httpContextAccessor,
                              IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration)
        {
            ConfigureLogging(app, loggerFactory, httpContextAccessor, eventHubLoggingConfiguration);
            app.ConfigureRequest("RadioNavigationalWarningsAdmin", "Index", env.IsDevelopment());
        }

        protected IConfigurationRoot BuildConfiguration(IWebHostEnvironment hostingEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true);

            builder.AddEnvironmentVariables();
            IConfigurationRoot tempConfig = builder.Build();
            string kvServiceUri = tempConfig["KeyVaultSettings:ServiceUri"];

            if (!string.IsNullOrWhiteSpace(kvServiceUri))
            {
                builder.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
            }

            return builder.Build();
        }

        private void ConfigureLogging(IApplicationBuilder app, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor,
                                      IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration)
        {
            if (!string.IsNullOrEmpty(eventHubLoggingConfiguration.Value.ConnectionString))
            {
                void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
                {
                    if (httpContextAccessor.HttpContext != null)
                    {
                        additionalValues["_Environment"] = eventHubLoggingConfiguration.Value.Environment;
                        additionalValues["_System"] = eventHubLoggingConfiguration.Value.System;
                        additionalValues["_Service"] = eventHubLoggingConfiguration.Value.Service;
                        additionalValues["_NodeName"] = eventHubLoggingConfiguration.Value.NodeName;
                        additionalValues["_RemoteIPAddress"] = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        additionalValues["_User-Agent"] = httpContextAccessor.HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;
                        additionalValues["_AssemblyVersion"] = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
                        additionalValues["_X-Correlation-ID"] =
                            httpContextAccessor.HttpContext.Request.Headers?[UkhoHeaderNames.XCorrelationId].FirstOrDefault() ?? string.Empty;

                        if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                        {
                            additionalValues["_UserId"] = httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                        }
                    }
                }

                loggerFactory.AddEventHub(
                                     config =>
                                     {
                                         config.Environment = eventHubLoggingConfiguration.Value.Environment;
                                         config.DefaultMinimumLogLevel =
                                             (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.MinimumLoggingLevel, true);
                                         config.MinimumLogLevels["UKHO"] =
                                             (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.UkhoMinimumLoggingLevel, true);
                                         config.EventHubConnectionString = eventHubLoggingConfiguration.Value.ConnectionString;
                                         config.EventHubEntityPath = eventHubLoggingConfiguration.Value.EntityPath;
                                         config.System = eventHubLoggingConfiguration.Value.System;
                                         config.Service = eventHubLoggingConfiguration.Value.Service;
                                         config.NodeName = eventHubLoggingConfiguration.Value.NodeName;
                                         config.AdditionalValuesProvider = ConfigAdditionalValuesProvider;
                                     });
            }

#if DEBUG
            //Add file based logger for development
            loggerFactory.AddFile(configuration.GetSection("Logging"));
#endif

            app.UseCorrelationIdMiddleware()
            .UseErrorLogging(loggerFactory);
        }
    }
}
