using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
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

        public async Task<AuthenticationResult> GetAuthTokenAsync()
        {
            AuthenticationResult authenticationResult;
            authenticationResult = await GenerateAccessTokenLocal();
            return authenticationResult;
            ////#if(DEBUG)
            ////            authenticationResult = await GenerateAccessTokenLocal();
            ////            return authenticationResult;
            ////#else
            ////            authenticationResult  = await GenerateADAccessToken();
            ////#endif
        }

       ////public async Task<AuthenticationResult> GenerateADAccessToken()
       ////{
       ////
       ////
       ////    string[] scopes = new string[] { azureADConfiguration.Value.Scope + "/.default" };
       ////    IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(azureADConfiguration.Value.ClientId)
       ////    .WithClientSecret(azureADConfiguration.Value.ClientSecret)
       ////    .WithAuthority(new Uri(azureADConfiguration.Value.MicrosoftOnlineLoginUrl + azureADConfiguration.Value.TenantId))
       ////    .Build(); AuthenticationResult authenticationResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
       ////    return authenticationResult;
       ////}

        public async Task<AuthenticationResult> GenerateAccessTokenLocal()
        {
            string tenantId = azureADConfiguration.Value.TenantId;
            string[] scopes = new string[] { azureADConfiguration.Value.Scope + "/.default" };

            var publicClientApplication = PublicClientApplicationBuilder
              .Create(azureADConfiguration.Value.Scope)
              .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
              .WithDefaultRedirectUri()
              .Build();


            using var cancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            ////TokenCacheHelper.EnableSerialization(publicClientApplication.UserTokenCache);
            var accounts = (await publicClientApplication.GetAccountsAsync()).ToList();
            var authenticationResult = await publicClientApplication.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                    .ExecuteAsync(cancellationSource.Token)
                    .ConfigureAwait(false);

            return authenticationResult;
        }
    }
}
