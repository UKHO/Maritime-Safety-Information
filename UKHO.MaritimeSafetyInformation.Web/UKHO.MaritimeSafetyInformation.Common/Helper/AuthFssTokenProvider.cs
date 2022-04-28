using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformation.Common
{
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
                AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);
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
