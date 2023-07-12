using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace UKHO.MaritimeSafetyInformation.Common.Filters
{
    [ExcludeFromCodeCoverage] //Used in Startup.cs
    public class CustomSecurityHeadersMiddleware
    {
        private readonly RequestDelegate next;

        public CustomSecurityHeadersMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("Permissions-Policy", "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");
            return next(httpContext);
        }
    }

    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class CustomSecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder builder) => builder.UseMiddleware<CustomSecurityHeadersMiddleware>();
    }
}
