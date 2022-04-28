using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common
{
    public class AuthFssTokenProvider : IAuthFssTokenProvider
    {

        private readonly IOptions<AzureADConfiguration> azureADConfiguration;

        public AuthFssTokenProvider(IOptions<AzureADConfiguration> _azureADConfiguration)
        {
            azureADConfiguration = _azureADConfiguration;
        }

        public async Task<string> GenerateADAccessToken()
        {
            DefaultAzureCredential azureCredential = new();
            TokenRequestContext tokenRequestContext = new(new string[] { azureADConfiguration.Value.ClientId + "/.default" });
            AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);
            return tokenResult.Token;
        }
    }
}
