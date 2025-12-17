using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    public class MockAuthSelectionMiddleware
    {
        private readonly RequestDelegate _next;
        public MockAuthSelectionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
            {
                if (context.Request.Headers.TryGetValue("X-Mock-Auth-Scheme", out var scheme))
                {
                    var authService = context.RequestServices.GetRequiredService<IAuthenticationService>();
                    var authenticateResult = await authService.AuthenticateAsync(context, scheme.ToString());
                    if (authenticateResult?.Principal != null)
                    {
                        context.User = authenticateResult.Principal;
                    }
                }
            }
            await _next(context);
        }
    }

    public static class MockAuthSelectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseMockAuthSelection(this IApplicationBuilder app)
            => app.UseMiddleware<MockAuthSelectionMiddleware>();
    }
}
