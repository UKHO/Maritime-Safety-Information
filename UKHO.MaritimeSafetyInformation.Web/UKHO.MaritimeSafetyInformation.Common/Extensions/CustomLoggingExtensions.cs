using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UKHO.Logging.EventHubLogProvider;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    public static class CustomLoggingExtensions
    {
        // Rhz So wrong; This is Key is defined in too many places
        // It is defined here for expiediency, but should be moved to a common place.
        // It is also defined in UKHO.MaritimeSafetyInformation.Web/Filters/CorrelationIdMiddleware.cs
        // and UKHO.MaritimeSafetyInformationAdmin.Web/Filters/CorrelationIdMiddleware.cs
        const string XCorrelationIdHeaderKey = "X-Correlation-ID"; 

        public static WebApplication AddCustomLogging(this WebApplication app, ILoggerFactory loggerFactory, IOptions<EventHubLoggingConfiguration> eventHubLoggingConfiguration = null)
        {
            if (!string.IsNullOrEmpty(eventHubLoggingConfiguration?.Value.ConnectionString))
            {
                void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
                {
                    //if (httpContextAccessor.HttpContext != null)
                    //{
                    //    additionalValues["_Environment"] = eventHubLoggingConfiguration.Value.Environment;
                    //    additionalValues["_System"] = eventHubLoggingConfiguration.Value.System;
                    //    additionalValues["_Service"] = eventHubLoggingConfiguration.Value.Service;
                    //    additionalValues["_NodeName"] = eventHubLoggingConfiguration.Value.NodeName;
                    //    additionalValues["_RemoteIPAddress"] = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //    additionalValues["_User-Agent"] = httpContextAccessor.HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;
                    //    additionalValues["_AssemblyVersion"] = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyFileVersionAttribute>().Single().Version;
                    //    additionalValues["_X-Correlation-ID"] =
                    //        httpContextAccessor.HttpContext.Request.Headers?[XCorrelationIdHeaderKey].FirstOrDefault() ?? string.Empty;

                    //    if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    //    {
                    //        additionalValues["_UserId"] = httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                    //    }
                    //}
                }

                loggerFactory.AddEventHub(
                                     config =>
                                     {
                                         config.Environment = eventHubLoggingConfiguration.Value.Environment;
                                         config.DefaultMinimumLogLevel =
                                             (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.MinimumLoggingLevel, true);
                                         config.MinimumLogLevels["UKHO"] =
                                             (LogLevel)Enum.Parse(typeof(LogLevel), eventHubLoggingConfiguration.Value.UkhoMinimumLoggingLevel, true);
                                         config.EventHubConnectionString = eventHubLoggingConfiguration.Value.ConnectionString;
                                         config.EventHubEntityPath = eventHubLoggingConfiguration.Value.EntityPath;
                                         config.System = eventHubLoggingConfiguration.Value.System;
                                         config.Service = eventHubLoggingConfiguration.Value.Service;
                                         config.NodeName = eventHubLoggingConfiguration.Value.NodeName;
                                         config.AdditionalValuesProvider = ConfigAdditionalValuesProvider;
                                     });
            };


            return app;
        }
    }
}
