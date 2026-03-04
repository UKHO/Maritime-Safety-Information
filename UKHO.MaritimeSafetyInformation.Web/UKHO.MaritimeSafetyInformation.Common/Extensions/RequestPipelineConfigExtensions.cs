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
            app.UseCspReportOnly(x =>
            {
                x.DefaultSources(y => y.Self());
                x.ScriptSources(y => y.Self().StrictDynamic().CustomSources(
#if DEBUG
                    "https://localhost:*",
                    "http://localhost:*",
#endif
                    "https://www.googletagmanager.com",
                    "https://*.hubspot.com",
                    "https://*.hs-scripts.com"
                ));
                x.StyleSources(y => y.Self().UnsafeInline().CustomSources(
                    "https://unpkg.com",
                    "https://cdn.jsdelivr.net"
                ));
                x.FontSources(y => y.Self().CustomSources(
                    "https://unpkg.com",
                    "https://cdn.jsdelivr.net"
                ));
                x.ImageSources(y => y.Self().CustomSources(
                    "data:",
                    "https://www.googletagmanager.com",
                    "https://www.google.com",
                    "https://www.google.co.uk",
                    "https://*.hubspot.com",
                    "https://perf-eu1.hsforms.com",
                    "https://px.ads.linkedin.com",
                    "https://bat.bing.com",
                    "https://*.svc.dynamics.com"
                ));
                x.ConnectSources(y => y.Self().CustomSources(
#if DEBUG
                    "https://localhost:*",
                    "http://localhost:*",
                    "ws://localhost:*",
                    "wss://localhost:*",
                    "http://127.0.0.1:*",
                    "https://127.0.0.1:*",
                    "ws://127.0.0.1:*",
                    "wss://127.0.0.1:*",
#endif
                    "https://www.googletagmanager.com",
                    "https://www.google.com",
                    "https://region1.google-analytics.com",
                    "https://cdn-ukwest.onetrust.com",
                    "https://privacyportal-uk.onetrust.com",
                    "https://api-eu1.hubapi.com",
                    "https://static.hsappstatic.net",
                    "https://px.ads.linkedin.com",
                    "https://pagead2.googlesyndication.com",
                    "https://bat.bing.com",
                    "https://vc.hotjar.io",
                    "https://*.hubspot.com",
                    "https://*.hs-scripts.com"
                ));
                x.FrameSources(y => y.Self().CustomSources(
                    "https://www.googletagmanager.com"
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
