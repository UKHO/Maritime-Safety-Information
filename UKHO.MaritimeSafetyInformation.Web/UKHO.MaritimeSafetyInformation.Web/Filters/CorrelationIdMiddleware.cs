using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Web.Filters
{
    [ExcludeFromCodeCoverage]
    public static class CorrelationIdMiddleware
    {
        public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder builder)
        {
            //rhz: there is room for improvement here. We could avoid multiple lookups of the same header by using TryGetValue.
            return builder.Use(async (context, next) =>
            {
                var correlationId = context.Request.Headers[UkhoHeaderNames.XCorrelationId].FirstOrDefault();

                if (string.IsNullOrEmpty(correlationId))
                {
                    correlationId = Guid.NewGuid().ToString();
                    context.Request.Headers.Append(UkhoHeaderNames.XCorrelationId, correlationId);
                }

                context.Response.Headers.Append(UkhoHeaderNames.XCorrelationId, correlationId);

                await next();
            });
        }
    }
}
