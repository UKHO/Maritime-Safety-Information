using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using UKHO.Logging.EventHubLogProvider;
using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.HealthCheck;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
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
            services.Configure<FileShareServiceConfiguration>(configuration.GetSection("FileShareService"));
            services.Configure<RadioNavigationalWarningsContextConfiguration>(configuration.GetSection("RadioNavigationalWarningsContext"));
            services.Configure<CacheConfiguration>(configuration.GetSection("CacheConfiguration"));

            services.Configure<AzureAdB2C>(configuration.GetSection("AzureAdB2C"));

            var msiDBConfiguration = new RadioNavigationalWarningsContextConfiguration();
            configuration.Bind("RadioNavigationalWarningsContext", msiDBConfiguration);
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

            services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

            //Configuring appsettings section AzureAdB2C, into IOptions
            services.AddOptions();
            services.Configure<OpenIdConnectOptions>(configuration.GetSection("AzureAdB2C"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHeaderPropagation(options =>
            {
                options.Headers.Add(CorrelationIdMiddleware.XCorrelationIdHeaderKey);
            });

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                configuration.Bind("AzureAdB2C", options);
                options.Events ??= new OpenIdConnectEvents();
                options.SaveTokens = true; // this saves the token for the downstream api
                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    context.ProtocolMessage.RedirectUri = configuration["AzureAdB2C:RedirectBaseUrl"] + configuration["AzureAdB2C:CallbackPath"];
                    await Task.FromResult(0);
                };
                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    context.ProtocolMessage.PostLogoutRedirectUri = configuration["AzureAdB2C:RedirectBaseUrl"] + configuration["AzureAdB2C:SignedOutCallbackPath"];
                    await Task.FromResult(0);
                };
            })
            .EnableTokenAcquisitionToCallDownstreamApi(new string[] { configuration["AzureAdB2C:Scope"] })
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseExceptionHandler("/error");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health");
            });
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

        [SuppressMessage("Code Smell", "S1172:Unused method parameters should be removed", Justification = "httpContextAccessor is used in action delegate")]
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

#if (DEBUG)
            //Add file based logger for development
            loggerFactory.AddFile(configuration.GetSection("Logging"));
#endif   

            app.UseCorrelationIdMiddleware()
            .UseErrorLogging(loggerFactory);
        }
    }
}
