using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IWebHostEnvironment env)
        {
            _configuration = BuildConfiguration(env);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Enables Application Insights telemetry.
            services.AddApplicationInsightsTelemetry();

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

            SeedData(new SqlConnection(_configuration.GetConnectionString("MSI-RNWDB-1"))).Wait();
        }


        internal bool TableExists(SqlConnection connection, string tableName)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
                
            using var command = new SqlCommand($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'", connection);
            return (int)command.ExecuteScalar() > 0;
        }

        internal async Task SeedData(SqlConnection connection)
        {
            var context = new RadioNavigationalWarningsContext(new DbContextOptionsBuilder<RadioNavigationalWarningsContext>().UseSqlServer(connection).Options);
            //context.Database.Migrate();
            context.Database.EnsureCreated();

            if (!TableExists(connection, "WarningType"))
            {
                context.Database.ExecuteSqlRaw("CREATE TABLE WarningType (Id INT PRIMARY KEY, Name NVARCHAR(50))");
            }
            if (!TableExists(connection, "RadioNavigationalWarnings"))
            {
                context.Database.ExecuteSqlRaw("CREATE TABLE RadioNavigationalWarnings (Id INT PRIMARY KEY, WarningType INT, Reference NVARCHAR(50), DateTimeGroup DATETIME, Summary NVARCHAR(100), Content NVARCHAR(500), ExpiryDate DATETIME, IsDeleted BIT, LastModified DATETIME)");
            }
            connection.Close();

            if (!context.WarningType.Any(w => w.Id == 1))
            {
                context.WarningType.Add(new WarningType { Id = 1, Name = "NAVAREA" });
                await context.SaveChangesAsync();
            }

            if (!context.WarningType.Any(w => w.Id == 2))
            {
                context.WarningType.Add(new WarningType { Id = 2, Name = "UK Coastal" });
                await context.SaveChangesAsync();
            }

            if (context.RadioNavigationalWarnings.Any()) return;


            context.RadioNavigationalWarnings.AddRange(
                new RadioNavigationalWarning
                {
                    Id = 1,
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
                    Id = 2,
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
    }
}
