using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Web.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<RadioNavigationalWarningsContext>(o =>
{
    o.UseSqlServer(builder.Configuration["ConnectionString"]);
});

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
