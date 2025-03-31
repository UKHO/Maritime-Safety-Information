using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using Azure.Identity;
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
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Web.Filters;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using UKHO.MaritimeSafetyInformation.Web.Validation;

namespace UKHO.MaritimeSafetyInformation.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Enables Application Insights telemetry.
            services.AddApplicationInsightsTelemetry();

            //RHZ Testing Start:
            //if development environment, then use the aspire connection string for the database
            var sqlConnecton = _configuration.GetSection("ConnectionStrings").GetSection("MSI-RNWDB-1").Value;
            _configuration.GetSection("RadioNavigationalWarningConfiguration").GetSection("ConnectionString").Value = sqlConnecton;
            //the one below we actually need:
            //_configuration.GetSection("RadioNavigationalWarningsContext").GetSection("ConnectionString").Value = sqlConnecton;

            //if development environment, then use the aspire connection for adds mock
            var addsMockUrl = _configuration.GetSection("services").GetSection("adds-mock").GetSection("mock-endpoint").GetSection("0").Value;
            _configuration.GetSection("FileShareService").GetSection("BaseUrl").Value = new UriBuilder(addsMockUrl) { Path = "fss" }.Uri.ToString();

            //RHZ Testing End:


            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(_configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddAzureWebAppDiagnostics();
            });

            services.Configure<EventHubLoggingConfiguration>(_configuration.GetSection("EventHubLoggingConfiguration"));
            services.Configure<RadioNavigationalWarningConfiguration>(_configuration.GetSection("RadioNavigationalWarningConfiguration"));
            services.Configure<FileShareServiceConfiguration>(_configuration.GetSection("FileShareService"));
            services.Configure<RadioNavigationalWarningsContextConfiguration>(_configuration.GetSection("RadioNavigationalWarningsContext"));
            services.Configure<CacheConfiguration>(_configuration.GetSection("CacheConfiguration"));
            services.Configure<BannerNotificationConfiguration>(_configuration.GetSection("BannerNotificationConfiguration"));
            services.Configure<AzureAdB2C>(_configuration.GetSection("AzureAdB2C"));

            var msiDBConfiguration = new RadioNavigationalWarningsContextConfiguration();
            _configuration.Bind("RadioNavigationalWarningsContext", msiDBConfiguration);
            services.AddDbContext<RadioNavigationalWarningsContext>(options => options.UseSqlServer(msiDBConfiguration.ConnectionString));

            services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();
            services.AddScoped<INMDataService, NMDataService>();
            services.AddScoped<IFileShareService, FileShareService>();
            services.AddScoped<IAuthFssTokenProvider, AuthFssTokenProvider>();
            services.AddScoped<IRNWService, RNWService>();
            services.AddScoped<IRNWRepository, RNWRepository>();
            services.AddScoped<IRNWDatabaseHealthClient, RNWDatabaseHealthClient>();
            services.AddScoped<IFileShareServiceHealthClient, FileShareServiceHealthClient>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAzureTableStorageClient, AzureTableStorageClient>();
            services.AddScoped<IFileShareServiceCache, FileShareServiceCache>();
            services.AddScoped<IAzureStorageService, AzureStorageService>();
            services.AddScoped<IWebhookService, WebhookService>();
            services.AddScoped<IEnterpriseEventCacheDataRequestValidator, EnterpriseEventCacheDataRequestValidator>();
            services.AddScoped<IMSIBannerNotificationService, MSIBannerNotificationService>();

            services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

            //Configuring appsettings section AzureAdB2C, into IOptions
            services.AddOptions();
            services.Configure<OpenIdConnectOptions>(_configuration.GetSection("AzureAdB2C"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(CorrelationIdMiddleware.XCorrelationIdHeaderKey);
            });

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                _configuration.Bind("AzureAdB2C", options);
                options.Events ??= new OpenIdConnectEvents();
                options.SaveTokens = true; // this saves the token for the downstream api
                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    context.ProtocolMessage.RedirectUri = _configuration["AzureAdB2C:RedirectBaseUrl"] + _configuration["AzureAdB2C:CallbackPath"];
                    await Task.FromResult(0);
                };
                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    context.ProtocolMessage.PostLogoutRedirectUri = _configuration["AzureAdB2C:RedirectBaseUrl"] + _configuration["AzureAdB2C:SignedOutCallbackPath"];
                    await Task.FromResult(0);
                };
            })
            .EnableTokenAcquisitionToCallDownstreamApi(new string[] { _configuration["AzureAdB2C:Scope"] })
            .AddInMemoryTokenCaches();

            services.AddHttpClient();
            services.AddHealthChecks()
                .AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck")
                .AddCheck<RNWDatabaseHealthCheck>("RNWDatabaseHealthCheck")
                .AddCheck<FileShareServiceHealthCheck>("FileShareServiceHealthCheck");
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
            app.ConfigureRequest("Home", "Index", env.IsDevelopment());
        }

        //protected IConfigurationRoot BuildConfiguration(IWebHostEnvironment hostingEnvironment)
        //{
        //    IConfigurationBuilder builder = new ConfigurationBuilder()
        //        .SetBasePath(hostingEnvironment.ContentRootPath)
        //        .AddJsonFile("appsettings.json", false, true)
        //        .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true);

        //    builder.AddEnvironmentVariables();
        //    IConfigurationRoot tempConfig = builder.Build();
        //    string kvServiceUri = tempConfig["KeyVaultSettings:ServiceUri"];

        //    if (!string.IsNullOrWhiteSpace(kvServiceUri))
        //    {
        //        builder.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
        //    }

        //    return builder.Build();
        //}

        protected IConfigurationRoot BuildConfiguration(HostApplicationBuilder builder)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);

            builder.AddServiceDefaults();

            configBuilder.AddEnvironmentVariables();
            IConfigurationRoot tempConfig = configBuilder.Build();
            string kvServiceUri = tempConfig["KeyVaultSettings:ServiceUri"];

            if (!string.IsNullOrWhiteSpace(kvServiceUri))
            {
                configBuilder.AddAzureKeyVault(new Uri(kvServiceUri), new DefaultAzureCredential());
            }

            return configBuilder.Build();
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
                            httpContextAccessor.HttpContext.Request.Headers?[CorrelationIdMiddleware.XCorrelationIdHeaderKey].FirstOrDefault() ?? string.Empty;

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
            loggerFactory.AddFile(_configuration.GetSection("Logging"));
#endif

            app.UseCorrelationIdMiddleware()
            .UseErrorLogging(loggerFactory);
        }
    }
}
