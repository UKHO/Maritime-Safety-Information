using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Web.Models;
using UKHO.MaritimeSafetyInformation.Web.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<RadioNavigationalWarningsContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("RadioNavigationalWarningsContext"));
});
builder.Services.AddScoped<IRNWRepository, RNWRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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
    pattern: "{controller=RadioNavigationalWarnings}/{action=Index}/{id?}");

app.Run();
