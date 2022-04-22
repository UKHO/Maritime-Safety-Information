

using Azure.Identity;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.HealthCheck;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

IWebHostEnvironment webHostEnvironment = builder.Environment;
IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
IConfiguration configuration = builder.Configuration;
EventHubLoggingConfiguration eventHubLoggingConfiguration;


configurationBuilder.SetBasePath( webHostEnvironment.ContentRootPath);
configurationBuilder.AddJsonFile("appsettings.json", false, true);
configurationBuilder.AddEnvironmentVariables();
builder.Configuration.AddAzureKeyVault(
       new Uri(configuration["KeyVaultSettings:ServiceUri"]),
       new DefaultAzureCredential());

builder.Services.Configure<EventHubLoggingConfiguration>(configuration.GetSection("EventHubLoggingConfiguration"));
builder.Services.AddScoped<IEventHubLoggingHealthClient, EventHubLoggingHealthClient>();
builder.Services.AddHealthChecks()
                .AddCheck<EventHubLoggingHealthCheck>("EventHubLoggingHealthCheck");

eventHubLoggingConfiguration = configuration.GetSection("EventHubLoggingConfiguration").Get<EventHubLoggingConfiguration>();

WebApplication app = builder.Build();

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();
