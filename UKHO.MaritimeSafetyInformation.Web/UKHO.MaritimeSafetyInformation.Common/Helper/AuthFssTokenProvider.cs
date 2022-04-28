using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common
{
    [ExcludeFromCodeCoverage]
    public class AuthFssTokenProvider : IAuthFssTokenProvider
    {
        private readonly IOptions<AzureADConfiguration> azureADConfiguration;
        private readonly ILogger<AuthFssTokenProvider> logger;

        public AuthFssTokenProvider(IOptions<AzureADConfiguration> _azureADConfiguration, ILogger<AuthFssTokenProvider> _logger)
        {
            azureADConfiguration = _azureADConfiguration;
            logger = _logger;
        }

        public async Task<string> GenerateADAccessToken(string correlationId)
        {
            try
            {
                DefaultAzureCredential azureCredential = new();
                TokenRequestContext tokenRequestContext = new(new string[] { azureADConfiguration.Value.ClientId + "/.default" });
                logger.LogInformation("AD Authentication- call defaultAzureCredential for ClientId:{ClientId} and _X-Correlation-ID:{CorrelationId}", azureADConfiguration.Value.ClientId, correlationId);
                AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);

                logger.LogInformation("AD Authentication- call defaultAzureCredential post gettokenasync for tokenResult:{tokenResult} and _X-Correlation-ID:{CorrelationId}", tokenResult, correlationId);
                return tokenResult.Token;
            }
            catch (Exception ex)
            {
                logger.LogInformation("AD Authentication failed with message:{ex} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                return string.Empty;
            }
        }
    }
}
