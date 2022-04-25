using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using UKHO.MaritimeSafetyInformation.Common.Models;

namespace UKHO.MaritimeSafetyInformation.Common.Helper
{
    public class AuthTokenProvider
    {
        public async Task<AccessTokenItem> GetNewAuthToken(string resource)
        {
            var tokenCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { VisualStudioTenantId = "9134ca48-663d-4a05-968a-31a42f0aed3e", ManagedIdentityClientId = "644e4406-4e92-4e5d-bdc5-3b233884f900" });
            var accessToken = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new string[] { resource + "/.default" }) { }
            );
            return new AccessTokenItem
            {
                ExpiresIn = DateTime.Now.AddHours(1)/*accessToken.ExpiresOn.UtcDateTime*/,
                AccessToken = !String.IsNullOrEmpty(resource) ? "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyIsImtpZCI6ImpTMVhvMU9XRGpfNTJ2YndHTmd2UU8yVnpNYyJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjQ5MjM4OTIwLCJuYmYiOjE2NDkyMzg5MjAsImV4cCI6MTY0OTI0NDU2OSwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQW5YVFVTMWtDL3l4ZTJ5R1Nlc1JBVkk5NkJkNXFnTThYNDNkWlorQ1l4eGFGWFFySWpVSkVGVkJsVVMrZDJaUkJOQ3JrNXpMaEVXOW5XK2s4aElHSE9YckU1V1FsNnR1YlhDSURiUTZTZkpRPSIsImFtciI6WyJwd2QiLCJyc2EiXSwiYXBwaWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJhcHBpZGFjciI6IjAiLCJlbWFpbCI6Im1vaGFtbWUxNTMxNUBtYXN0ZWsuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvYWRkMWM1MDAtYTZkNy00ZGJkLWI4OTAtN2Y4Y2I2ZjdkODYxLyIsImlwYWRkciI6IjIyMy4xODQuMjU0LjE1NCIsIm5hbWUiOiJNb2hhbW1lZCBLaGFuIiwib2lkIjoiMDEzMDU1Y2ItZWQ2Mi00NmQyLWFkZTgtMzhmY2NkNjQwYWE2IiwicmgiOiIwLkFWTUFTTW8wa1QxbUJVcVdpakdrTHdydFBpVGdXNEFJb3Z0QXEyODVuQ1pIMHpRQ0FBNC4iLCJyb2xlcyI6WyJCYXRjaENyZWF0ZSJdLCJzY3AiOiJVc2VyLlJlYWQiLCJzdWIiOiJnZHBHdUE3dVNmT0djRG5LZWZmTjUxdkFQNldraEo2V3Fsd05pWlIyT2o0IiwidGlkIjoiOTEzNGNhNDgtNjYzZC00YTA1LTk2OGEtMzFhNDJmMGFlZDNlIiwidW5pcXVlX25hbWUiOiJtb2hhbW1lMTUzMTVAbWFzdGVrLmNvbSIsInV0aSI6IlcxeHZYbllfLVVpeS1POVZTTGU4QUEiLCJ2ZXIiOiIxLjAifQ.l0I0fST2hJoNKAZrNiCWINcCJf9E9odSTVPAegqF9ra2AHYS3Ba4WFHxP6KwT6KhreVc3nsRDQkASlmUOvqBxKhP0c5Xrl2l6w4I_6MmqqT81z1D3p9zbYKF7x4zUMfBlvzX6LW5czjTiocGC4iU42Mnil_H4ufVOPbXeu8dOfm05LZ2Rl8YKbyzRwg2V0l9XePXhWQpe9uFoKyDSfplmf2aeHETv1OwtY3sDVnjEXK5fuS5N9KsXM8eNfnq930IkszLAy11lj05yUXoQa7TTe8VZBN2mo9KTFGG6EYzDE4OFbGcRgQCORjT9ifr606p0Kc-fc2U9ayX4h0_Nvb9eg" : resource/*accessToken.Token*/
            };

        }
    }
}
