using Azure.Identity;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.HealthCheck;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

IWebHostEnvironment webHostEnvironment = app.Environment;
IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration configuration = app.Configuration;
IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration;
IHttpContextAccessor httpContextAccessor;
ILoggerFactory loggerFactory;

configurationBuilder.SetBasePath(webHostEnvironment.ContentRootPath);
configurationBuilder.AddJsonFile("appsettings.json", false, true);
configurationBuilder.AddEnvironmentVariables();
builder.Configuration.AddAzureKeyVault(
       new Uri(builder.Configuration["KeyVaultSettings:ServiceUri"]),
       new DefaultAzureCredential());

builder.Services.Configure<EventHubLoggingConfiguration>(configuration.GetSection("EventHubLoggingConfiguration"));

builder.Services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();

builder.Services.AddHealthChecks()
                .AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck");


//if (!string.IsNullOrWhiteSpace(eventHubLoggingConfiguration.Value.ConnectionString))
//{
//    void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
//    {
//        if (httpContextAccessor.HttpContext != null)
//        {
//            additionalValues["_Environment"] = eventHubLoggingConfiguration.Value.Environment;
//            additionalValues["_System"] = eventHubLoggingConfiguration.Value.System;
//            additionalValues["_Service"] = eventHubLoggingConfiguration.Value.Service;
//            additionalValues["_NodeName"] = eventHubLoggingConfiguration.Value.NodeName;
//            additionalValues["_RemoteIPAddress"] = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
//            additionalValues["_User-Agent"] = httpContextAccessor.HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;
//            additionalValues["_AssemblyVersion"] = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
//            additionalValues["_X-Correlation-ID"] = "";
//            // httpContextAccessor.HttpContext.Request.Headers?[CorrelationIdMiddleware.XCorrelationIdHeaderKey].FirstOrDefault() ?? string.Empty;

//            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
//            {
//                additionalValues["_UserId"] = httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
//            }
//        }
//    }
//    loggerFactory.AddEventHub(
//                 config =>
//                 {
//                     config.Environment = eventHubLoggingConfiguration.Value.Environment;
//                     config.DefaultMinimumLogLevel =
//                         (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.MinimumLoggingLevel, true);
//                     config.MinimumLogLevels["UKHO"] =
//                         (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.UkhoMinimumLoggingLevel, true);
//                     config.EventHubConnectionString = eventHubLoggingConfiguration.Value.ConnectionString;
//                     config.EventHubEntityPath = eventHubLoggingConfiguration.Value.EntityPath;
//                     config.System = eventHubLoggingConfiguration.Value.System;
//                     config.Service = eventHubLoggingConfiguration.Value.Service;
//                     config.NodeName = eventHubLoggingConfiguration.Value.NodeName;
//                     config.AdditionalValuesProvider = ConfigAdditionalValuesProvider;
//                 });
//}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
