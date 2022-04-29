using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common
{
    [ExcludeFromCodeCoverage]
    public class AuthFssTokenProvider : IAuthFssTokenProvider
    {
        private readonly IOptions<AzureADConfiguration> _azureADConfiguration;
        private readonly ILogger<AuthFssTokenProvider> _logger;

        public AuthFssTokenProvider(IOptions<AzureADConfiguration> azureADConfiguration, ILogger<AuthFssTokenProvider> logger)
        {
            _azureADConfiguration = azureADConfiguration;
            _logger = logger;
        }

        public async Task<string> GenerateADAccessToken(string correlationId)
        {
            try
            {
                DefaultAzureCredential azureCredential = new();
                TokenRequestContext tokenRequestContext = new(new string[] { _azureADConfiguration.Value.FssClientId + "/.default" });
                AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);               
                return tokenResult.Token;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("AD Authentication failed with message:{ex} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                return string.Empty;
            }
        }
    }
}
