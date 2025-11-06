using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace UKHO.MaritimeSafetyInformation.Common.Filters
{
    [ExcludeFromCodeCoverage] //Used in Startup.cs
    public class CustomSecurityHeadersMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public Task Invoke(HttpContext httpContext)
        {

            httpContext.Response.Headers.Append("Permissions-Policy", "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");
            return _next(httpContext);
        }
    }

    public static class CustomSecurityHeadersMiddlewareExtensions
    {
        /// <summary>
        /// Extension method used to add the security header middleware to the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder app) => app.UseMiddleware<CustomSecurityHeadersMiddleware>();
    }
}
