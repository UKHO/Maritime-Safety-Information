using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Web.Filters
{
    [ExcludeFromCodeCoverage]
    public static class CorrelationIdMiddleware
    {
        public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder builder)
        {
            return builder.Use(async (context, next) =>
            {
                // Try to get the correlation ID header value directly for performance
                string correlationId;
                if (!context.Request.Headers.TryGetValue(UkhoHeaderNames.XCorrelationId, out var headerValues) ||
                    string.IsNullOrEmpty(headerValues))
                {
                    correlationId = Guid.NewGuid().ToString();
                    context.Request.Headers[UkhoHeaderNames.XCorrelationId] = correlationId;
                }
                else
                {
                    correlationId = headerValues.ToString();
                }

                //rhz : Create a new activity with the correlation ID
                using (var activity = new Activity("Request"))
                {
                    activity.SetIdFormat(ActivityIdFormat.W3C);
                    activity.SetParentId(correlationId);
                    activity.Start();
                    //activity.AddTag("correlationId", correlationId);
                    //context.Features.Set(activity);

                    context.Response.OnStarting(() =>
                    {
                        context.Response.Headers[UkhoHeaderNames.XCorrelationId] = correlationId;
                        return Task.CompletedTask;
                    });
                    await next(context);
                }
                // rhz : End created activity

                //context.Response.Headers[UkhoHeaderNames.XCorrelationId] = correlationId;

                // await next();
            });
        }
    }
}
