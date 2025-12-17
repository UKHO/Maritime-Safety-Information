using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using UKHO.MaritimeSafetyInformation.Common;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;



namespace Aspire.Hosting
{
    internal static class SqlServerResourceBuilderExtensions
    {
        public static IResourceBuilder<SqlServerServerResource> WithResetCommand(this IResourceBuilder<SqlServerServerResource> builder)
        {
            var commandOptions = new CommandOptions
            {
                UpdateState = OnUpdateResourceState,

                Description = "Reset the Local database ",
                ConfirmationMessage = "Are you sure you want to reset the database?",

                IconName = "ArrowClockwise",
                IconVariant = IconVariant.Filled
            };

            builder.WithCommand(
                name: "reset-database",
                displayName: "Reset Database",
                executeCommand: context => OnRunResetDatabaseCommandAsync(builder,context),
                commandOptions:commandOptions);

            return builder;
        }


        private static async Task<ExecuteCommandResult> OnRunResetDatabaseCommandAsync(
            IResourceBuilder<SqlServerServerResource> builder,
            ExecuteCommandContext context)
        {
            var connectionString = await builder.Resource.GetConnectionStringAsync() ??
                throw new InvalidOperationException("Connection string is not available.");

            var databaseName = builder.Resource.Databases.FirstOrDefault().Value ??
                throw new InvalidOperationException("Database name is not available.");

            connectionString += $";Initial Catalog={databaseName}";

            await using var connection = new SqlConnection(connectionString);
            var dbContext = new RadioNavigationalWarningsContext(new DbContextOptionsBuilder<RadioNavigationalWarningsContext>().UseSqlServer(connection).Options);

            var canConnect = await dbContext.Database.CanConnectAsync();

            if (canConnect)
            {
                dbContext.Database.ExecuteSqlRaw("DELETE FROM [dbo].[RadioNavigationalWarnings]");

                dbContext.AddRange(
                    new RadioNavigationalWarning
                    {
                        WarningType = 1,
                        Reference = "NAVAREA 1 TEST/22",
                        DateTimeGroup = new DateTime(year:2022,month:7,day:4,hour:12,minute:0,second:0,kind:DateTimeKind.Utc),
                        Summary = "SPACE WEATHER. BIG SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.",
                        Content = @"NAVAREA
                                     NAVAREA TEST/24
                                     301040 UTC Dec 25
                                     SPACE WEATHER.
                                     SOLAR STORM IN PROGRESS FROM 311200 UTC DEC 24.
                                     RADIO AND SATELLITE NAVIGATION SERVICES MAY BE AFFECTED.",
                        IsDeleted = false,
                        LastModified = DateTime.UtcNow
                    },
                    new RadioNavigationalWarning
                    {
                        WarningType = 2,
                        Reference = "UK Coastal TEST/22",
                        DateTimeGroup = new DateTime(year: 2022, month: 7, day: 4, hour: 12, minute: 0, second: 0, kind: DateTimeKind.Utc),
                        Summary = "HUMBER. RHZ HORNSEA 1 AND 2 WINDFARMS. TURBINE FOG SIGNALS INOPERATIVE.",
                        Content = @"UK Coastal
                                                 TEST/24
                                                 301510 UTC Dec 25
                                                 HUMBER.
                                                 HORNSEA 1 AND 2 WINDFARMS.
                                                 1.TURBINES T25 54-00.3N 001-36.7E, A16 53-50.0N 001-58.7E AND S16 53-59.4N 001-48.3E, FOG SIGNALS INOPERATIVE.
                                                 2.CANCEL WZ 895.",
                        IsDeleted = false,
                        LastModified = DateTime.UtcNow
                    });

                var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Commit initial Data");
                }

                await dbContext.SaveChangesAsync();
            }
            else
            {
                var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("No data to reset in the database");
                }

            }
            return CommandResults.Success();
        }

        private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
        {
            var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Resetting database: {ResourceSnapshot}",
                    context.ResourceSnapshot);
            }

            return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
                ? ResourceCommandState.Enabled
                : ResourceCommandState.Disabled;

            
        }
    }
}
