using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    public class MockTokenAcquisition : Microsoft.Identity.Web.ITokenAcquisition
    {
        public Task<string> GetAccessTokenForAppAsync(string[] scopes, string tenantId = null, string correlationId = null)
        {
            // Return a mock token for testing purposes
            return Task.FromResult("mock-access-token");
        }
        // Other methods can be implemented as needed for testing purposes
        public Task<string> GetAccessTokenForUserAsync(string[] scopes, ClaimsPrincipal user, string tenantId = null, string correlationId = null)
        {
            return Task.FromResult("mock-user-access-token");
        }
        public Task<string> GetAccessTokenForAppAsync(string scope, string tenantId = null, string correlationId = null)
        {
            return Task.FromResult("mock-app-access-token");
        }

        public Task<string> GetAccessTokenForUserAsync(IEnumerable<string> scopes, string authenticationScheme, string tenantId = null, string userFlow = null, ClaimsPrincipal user = null, TokenAcquisitionOptions tokenAcquisitionOptions = null) => Task.FromResult("mock-user-access-token");
        public Task<AuthenticationResult> GetAuthenticationResultForUserAsync(IEnumerable<string> scopes, string authenticationScheme, string tenantId = null, string userFlow = null, ClaimsPrincipal user = null, TokenAcquisitionOptions tokenAcquisitionOptions = null) => throw new NotImplementedException();
        public Task<string> GetAccessTokenForAppAsync(string scope, string authenticationScheme, string tenant = null, TokenAcquisitionOptions tokenAcquisitionOptions = null) => throw new NotImplementedException();
        public Task<AuthenticationResult> GetAuthenticationResultForAppAsync(string scope, string authenticationScheme, string tenant = null, TokenAcquisitionOptions tokenAcquisitionOptions = null) => throw new NotImplementedException();
        public void ReplyForbiddenWithWwwAuthenticateHeader(IEnumerable<string> scopes, MsalUiRequiredException msalServiceException, string authenticationScheme, HttpResponse httpResponse = null) => throw new NotImplementedException();
        public string GetEffectiveAuthenticationScheme(string authenticationScheme) => throw new NotImplementedException();
        public Task ReplyForbiddenWithWwwAuthenticateHeaderAsync(IEnumerable<string> scopes, MsalUiRequiredException msalServiceException, HttpResponse httpResponse = null) => throw new NotImplementedException();
    }
}
