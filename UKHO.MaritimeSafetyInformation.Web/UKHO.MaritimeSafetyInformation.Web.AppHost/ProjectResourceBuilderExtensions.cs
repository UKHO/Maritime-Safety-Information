using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Aspire.Hosting
{
     internal static class ProjectResourceBuilderExtensions
     {
         public static IResourceBuilder<ProjectResource> WithLoginCommand(this IResourceBuilder<ProjectResource> builder)
         {
            var commandOptions = new CommandOptions
             {
                 //Description = "Set an environment variable (process-level). Restart of individual project resource is not currently supported; perform manual rebuild if needed.",
                 IconName = "ArrowClockwise",
                 IconVariant = IconVariant.Filled,
                 UpdateState = OnUpdateResourceState
             };

             builder.WithCommand(
             name: "set-env",
             displayName: "Login User ",
             executeCommand: context => ExecuteSetUserEnvAsync(builder, context),
             commandOptions: commandOptions);

             return builder;
         }

         private static async Task<ExecuteCommandResult> ExecuteSetUserEnvAsync(
             IResourceBuilder<ProjectResource> builder,
             ExecuteCommandContext context)
         {
            //=====================
#pragma warning disable ASPIREINTERACTION001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var interactionService = context.ServiceProvider.GetRequiredService<IInteractionService>();

            var inputs = new List<InteractionInput>
            {
                new()
                {
                    Name = "Active User",
                    InputType = InputType.Choice,
                    Required = true,
                    Options =
                    [
                        new ("", "Select a login"),
                        new("MockUser1", "Basic"),
                        new("MockDistributorUser", "Distributor"),
                        new("none", "None")
                    ]
                }
            };

            var appConfigurationInput = await interactionService.PromptInputsAsync(
                title: "Login User Configuration",
                message: "Set login user :",
                inputs: inputs);

            const string variable = "LOCAL_USER_FLAG";
            string? selectedUser = null;
            var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

            if (!appConfigurationInput.Canceled)
            {
                selectedUser = appConfigurationInput.Data[0].Value;
                selectedUser = selectedUser == "none" ? null : selectedUser;
                Environment.SetEnvironmentVariable(variable, selectedUser);
            }
            else
            {
                Environment.SetEnvironmentVariable(variable, null);
            }

            // Update mockconfig.json with the selected user flag.
            try
            {
                // Attempt to resolve the project directory via reflection (Aspire internal type).
                string? projectDir = null;
                try
                {
                    var projectResource = builder.Resource;
                    var annotation = projectResource.Annotations[0];  // there is probably a better way to get this

                    var projectPathProp = annotation.GetType()
                        .GetProperty("ProjectPath", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var projectPath = projectPathProp?.GetValue(annotation) as string;
                    
                    if (!string.IsNullOrWhiteSpace(projectPath) && File.Exists(projectPath))
                    {
                        projectDir = Path.GetDirectoryName(projectPath);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogDebug(ex, "Could not reflect ProjectPath from ProjectResource.");
                }

                // Fallback: use base directory.
                projectDir ??= AppContext.BaseDirectory;

                var mockConfigPath = Path.Combine(projectDir, "mockconfig.json");

                JsonNode rootNode;
                if (File.Exists(mockConfigPath))
                {
                    try
                    {
                        await using var readStream = File.Open(mockConfigPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        rootNode = await JsonNode.ParseAsync(readStream) ?? new JsonObject();
                    }
                    catch (Exception ex)
                    {
                        logger?.LogWarning(ex, "Failed to parse existing mockconfig.json. Recreating file.");
                        rootNode = new JsonObject();
                    }
                }
                else
                {
                    rootNode = new JsonObject();
                }

                if (rootNode is not JsonObject obj)
                {
                    // If the root is not an object, replace it.
                    obj = new JsonObject();
                    rootNode = obj;
                }

                // Optional: also mirror the environment variable name if consumers expect that.
                obj["LOCAL_USER_FLAG"] = string.IsNullOrWhiteSpace(selectedUser) ? null : selectedUser;

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                // Write atomically.
                var tempFile = mockConfigPath + ".tmp";
                await using (var writeStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await JsonSerializer.SerializeAsync(writeStream, rootNode, jsonOptions);
                    await writeStream.FlushAsync();
                }

                // Replace original.
                File.Copy(tempFile, mockConfigPath, overwrite: true);
                File.Delete(tempFile);

                logger?.LogInformation("Updated mockconfig.json at {Path} with UserFlag='{UserFlag}'", mockConfigPath, selectedUser);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to update mockconfig.json with user flag.");
                // Continue without failing the command.
            }

            if (builder.Resource.TryGetAnnotationsOfType<ResourceCommandAnnotation>(out var cmd))
            {
                var cmdadnotation = cmd.First(a => a.Name == "resource-restart");

                if (cmdadnotation != null)
                {
                    var result = await cmdadnotation.ExecuteCommand(context);
                    if (!result.Success)
                    {
                        return CommandResults.Failure($"Failed to restart resource: {result.ErrorMessage}");
                    }
                }
            }

            return CommandResults.Success();
#pragma warning restore ASPIREINTERACTION001

            
         }

         private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
         {
             return context.ResourceSnapshot.HealthStatus == HealthStatus.Healthy
             ? ResourceCommandState.Enabled
             : ResourceCommandState.Disabled;
         }

        
    }
}
