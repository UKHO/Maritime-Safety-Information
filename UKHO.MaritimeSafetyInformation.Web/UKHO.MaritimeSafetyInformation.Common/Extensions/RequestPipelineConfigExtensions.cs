using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using UKHO.MaritimeSafetyInformation.Common.Filters;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    /// <summary>
    /// Extension method used to configure the HTTP request pipeline.
    /// Replaces the previous ConfigureRequest method in RequestConfigurationExtensions.cs.
    /// Uses the WebApplication type for better integration with ASP.NET Core 6+. instead of IApplicationBuilder.
    /// and removes the isDevelopment parameter, as the environment can be checked directly within the method.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="defaultController"></param>
    /// <param name="defaultAction"></param>

    /// <returns></returns>
    public static class RequestPipelineConfigExtensions
    {
        public static WebApplication ConfigureRequestPipeline(this WebApplication app, string defaultController, string defaultAction)
        {
            app.UseHttpsRedirection();
            app.UseHsts(x => x.MaxAge(365).IncludeSubdomains());
            app.UseReferrerPolicy(x => x.NoReferrer());
            app.UseCsp(x =>
            {
                x.DefaultSources(y => y.Self());
                x.ScriptSources(y => y.Self().CustomSources(
                    "https://www.googletagmanager.com",
                    "https://cdn-ukwest.onetrust.com"
                ));
                x.StyleSources(y => y.Self().CustomSources(
                    "https://unpkg.com/%40ukho/styles@1.3.21/",
                    "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.8.1/"
                ));
                x.FontSources(y => y.Self().CustomSources(
                    "https://unpkg.com/%40ukho/styles@1.3.21/",
                    "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.8.1/"
                ));
                x.ImageSources(y => y.Self().CustomSources(
                    "data:"
                ));
            });
            app.UseCustomSecurityHeaders();
            app.UseStaticFiles();
            app.UseXfo(x => x.SameOrigin());
            app.UseXContentTypeOptions();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/error");
            app.UseRouting();

            if (app.Environment.IsDevelopment())
            {
                app.UseMockAuthSelection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: $"{{controller={defaultController}}}/{{action={defaultAction}}}/{{id?}}");

            app.MapHealthChecks("/health");

            return app;
        }
    }
}
