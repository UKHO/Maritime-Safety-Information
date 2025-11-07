using Aspire.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace Aspire.Hosting
{
     internal static class ProjectResourceBuilderExtensions
     {
         public static IResourceBuilder<ProjectResource> WithLoginCommand(this IResourceBuilder<ProjectResource> builder)
         {
             var commandOptions = new CommandOptions
             {
                 Description = "Set an environment variable (process-level). Restart of individual project resource is not currently supported; perform manual rebuild if needed.",
                 ConfirmationMessage = "Apply environment variable to host process?",
                 IconName = "ArrowClockwise",
                 IconVariant = IconVariant.Filled,
                 UpdateState = OnUpdateResourceState
             };

             builder.WithCommand(
             name: "set-env",
             displayName: "Set User Flag",
             executeCommand: context => ExecuteSetUserEnvAsync(builder, context),
             commandOptions: commandOptions);

             return builder;
         }

         private static async Task<ExecuteCommandResult> ExecuteSetUserEnvAsync(
             IResourceBuilder<ProjectResource> builder,
             ExecuteCommandContext context)
         {
             var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

             try
             {
                 // Example variable + value. You can change these or duplicate the command for different flags.
                const string variable = "LOCAL_USER_FLAG";
                var value = "MockDistributorUser";



                Environment.SetEnvironmentVariable(variable, value);

                if (logger.IsEnabled(LogLevel.Information))
                 {
                    logger.LogInformation("Set environment variable {Variable}={Value} for resource {ResourceName}. A full rebuild may be required for the app to observe new value.", variable, value, builder.Resource.Name);
                 }
             }
             catch (Exception ex)
             {
                 if (logger.IsEnabled(LogLevel.Error))
                 {
                    logger.LogError(ex, "Failed setting environment variable for resource {ResourceName}.", builder.Resource.Name);
                 }
             }

             await Task.CompletedTask;
             return CommandResults.Success();
         }

         private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
         {
             return context.ResourceSnapshot.HealthStatus == HealthStatus.Healthy
             ? ResourceCommandState.Enabled
             : ResourceCommandState.Disabled;
         }
     }
}
