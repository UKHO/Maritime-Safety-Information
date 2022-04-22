using System.Diagnostics.CodeAnalysis;
using System.Net;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformation.Web.Filters
{
    [ExcludeFromCodeCoverage]
    public static class LoggingMiddleware
    {

        public static IApplicationBuilder UseErrorLogging(this IApplicationBuilder appBuilder, ILoggerFactory loggerFactory)
        {
            return appBuilder.Use(async (context, func) =>
            {
                try
                {
                    await func();
                }
                catch (Exception e)
                {
                    loggerFactory
                        .CreateLogger(context.Request.Path)
                        .LogError(EventIds.UnhandledControllerException.ToEventId(), e, "Unhandled controller exception {Exception}", e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            });
        }
    }

}
