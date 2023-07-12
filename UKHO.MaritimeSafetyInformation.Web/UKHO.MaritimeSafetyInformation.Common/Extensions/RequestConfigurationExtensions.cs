using Microsoft.AspNetCore.Builder;
using UKHO.MaritimeSafetyInformation.Common.Filters;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    public static class RequestConfigurationExtensions
    {
		/// <summary>
		/// Extension method used to configure the HTTP request pipeline.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="defaultController"></param>
		/// <param name="defaultAction"></param>
		/// <param name="isDevelopment"></param>
		/// <returns></returns>
		public static IApplicationBuilder ConfigureRequest(this IApplicationBuilder app, string defaultController, string defaultAction, bool isDevelopment)
        {
            app.UseHttpsRedirection();
            app.UseHsts(x => x.MaxAge(365).IncludeSubdomains());
            app.UseReferrerPolicy(x => x.NoReferrer());
            app.UseCsp(x =>
            {
                x.DefaultSources(y => y.Self());
                x.ScriptSources(y => y.Self());
            });
            app.UseCustomSecurityHeaders();
            app.UseStaticFiles();
            app.UseXfo(x => x.SameOrigin());
            app.UseXContentTypeOptions();

            if (isDevelopment)
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
                    pattern: $"{{controller={defaultController}}}/{{action={defaultAction}}}/{{id?}}");
                endpoints.MapHealthChecks("/health");
            });

            return app;
        }
	}
}
