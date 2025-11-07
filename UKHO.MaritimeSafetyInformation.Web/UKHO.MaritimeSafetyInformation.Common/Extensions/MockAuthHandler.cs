using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UKHO.MaritimeSafetyInformation.Common.Extensions
{
    public class MockAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public MockAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Add a name claim to the mock identity
            
            var claims = new[] { new Claim(ClaimTypes.Name, "MockUser1") };
            var identity = new ClaimsIdentity(claims, "MockDynamic");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "MockDynamic");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class MockAuthHandlerDistro : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public MockAuthHandlerDistro(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Add a name claim to the mock identity

            var claims = new[] { new Claim(ClaimTypes.Name, "MockDistributorUser"), new Claim(ClaimTypes.Role, "Distributor") };
            var identity = new ClaimsIdentity(claims, "MockDynamic");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "MockDynamic");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
