using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using UKHO.Logging.EventHubLogProvider;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Filters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//*****************************************************

IWebHostEnvironment webHostEnvironment = builder.Environment;
IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
ILoggerFactory loggerFactory = new LoggerFactory();
IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
IConfiguration configuration = builder.Configuration;
EventHubLoggingConfiguration eventHubLoggingConfiguration;

configurationBuilder.SetBasePath(webHostEnvironment.ContentRootPath);
configurationBuilder.AddJsonFile("appsettings.json", false, true);
configurationBuilder.AddEnvironmentVariables();

if (!string.IsNullOrWhiteSpace(configuration["KeyVaultSettings:ServiceUri"]))
{
    builder.Configuration.AddAzureKeyVault(
       new Uri(configuration["KeyVaultSettings:ServiceUri"]),
       new DefaultAzureCredential());
}

builder.Services.Configure<EventHubLoggingConfiguration>(configuration.GetSection("EventHubLoggingConfiguration"));
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
    loggingBuilder.AddAzureWebAppDiagnostics();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddHeaderPropagation(options =>
{
    options.Headers.Add(CorrelationIdMiddleware.XCorrelationIdHeaderKey);
});
builder.Services.AddApplicationInsightsTelemetry();

eventHubLoggingConfiguration = configuration.GetSection("EventHubLoggingConfiguration").Get<EventHubLoggingConfiguration>();

//*****************************************************

WebApplication app = builder.Build();

ConfigureLogging(app, loggerFactory, httpContextAccessor, eventHubLoggingConfiguration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


void ConfigureLogging(WebApplication app, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor, EventHubLoggingConfiguration eventHubLoggingConfiguration)
{
    if (!string.IsNullOrWhiteSpace(eventHubLoggingConfiguration?.ConnectionString))
    {
        void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                additionalValues["_Environment"] = eventHubLoggingConfiguration.Environment;
                additionalValues["_System"] = eventHubLoggingConfiguration.System;
                additionalValues["_Service"] = eventHubLoggingConfiguration.Service;
                additionalValues["_NodeName"] = eventHubLoggingConfiguration.NodeName;
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
                                     config.Environment = eventHubLoggingConfiguration.Environment;
                                     config.DefaultMinimumLogLevel =
                                         (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.MinimumLoggingLevel, true);
                                     config.MinimumLogLevels["UKHO"] =
                                         (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.UkhoMinimumLoggingLevel, true);
                                     config.EventHubConnectionString = eventHubLoggingConfiguration.ConnectionString;
                                     config.EventHubEntityPath = eventHubLoggingConfiguration.EntityPath;
                                     config.System = eventHubLoggingConfiguration.System;
                                     config.Service = eventHubLoggingConfiguration.Service;
                                     config.NodeName = eventHubLoggingConfiguration.NodeName;
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


