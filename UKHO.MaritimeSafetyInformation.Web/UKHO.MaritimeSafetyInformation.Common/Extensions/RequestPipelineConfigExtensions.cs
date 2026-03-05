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
                x.ScriptSources(y => y.Self().StrictDynamic());
                x.ObjectSources(y => y.None());
                x.BaseUris(y => y.None());
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
