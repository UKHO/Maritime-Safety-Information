using Microsoft.Identity.Client;

namespace UKHO.MaritimeSafetyInformation.Common
{
    public class AuthFssTokenProvider : IAuthFssTokenProvider
    {
        public async Task<AuthenticationResult> GetAuthTokenAsync()
        {
            string? tenantId = "9134ca48-663d-4a05-968a-31a42f0aed3e";
            string[]? scopes = new[] { $"805be024-a208-40fb-ab6f-399c2647d334/.default" };

            var publicClientApplication = PublicClientApplicationBuilder
              .Create("805be024-a208-40fb-ab6f-399c2647d334")
              .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
              .WithDefaultRedirectUri()
              .Build();


            using var cancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            TokenCacheHelper.EnableSerialization(publicClientApplication.UserTokenCache);
            var accounts = (await publicClientApplication.GetAccountsAsync()).ToList();
            var authenticationResult = await publicClientApplication.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                    .ExecuteAsync(cancellationSource.Token)
                    .ConfigureAwait(false);

            return authenticationResult;
        }
    }
}
