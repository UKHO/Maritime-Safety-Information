using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AuthFssTokenProvider : IAuthFssTokenProvider
    {
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfiguration;
        private readonly ILogger<AuthFssTokenProvider> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;

        public AuthFssTokenProvider(IOptions<FileShareServiceConfiguration> fileShareServiceConfiguration, ILogger<AuthFssTokenProvider> logger, ITokenAcquisition tokenAcquisition)
        {
            _fileShareServiceConfiguration = fileShareServiceConfiguration;
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<string> GenerateADAccessToken(bool isDistributorUser, string correlationId)
        {
            try
            {
                if (isDistributorUser)
                {
                    return await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "https://MGIAIDTESTB2C.onmicrosoft.com/FileShareServiceAPIQA/Public" });
                }
                else
                {
                    DefaultAzureCredential azureCredential = new();
                    TokenRequestContext tokenRequestContext = new(new string[] { _fileShareServiceConfiguration.Value.FssClientId + "/.default" });
                    AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);
                    return tokenResult.Token;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("AD Authentication failed with message:{ex} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                return string.Empty;
            }
        }
    }
}
