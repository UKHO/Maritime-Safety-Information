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
                    //return "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjU3Mjg0NzI4LCJuYmYiOjE2NTcyODQ3MjgsImV4cCI6MTY1NzI4OTE3MiwiYWNyIjoiMSIsImFpbyI6IkFXUUFtLzhUQUFBQWIxbWk2OEFxNWJQY25yZ0Jrdlh2WDEvV3pETStvdDdzZnRJTmhWVGJ1QisyQ2dhOU1FOC9BL3AvdE02SzBxaEtxS3BnSDlINHZWVlJXbW9IWVR4NlBud3VaVWM5R2JQWjIxcnNRek1BOEgwaWY0QWU4MlNLbUdhQmY5VnRuM1pMIiwiYW1yIjpbInB3ZCJdLCJhcHBpZCI6IjgwNWJlMDI0LWEyMDgtNDBmYi1hYjZmLTM5OWMyNjQ3ZDMzNCIsImFwcGlkYWNyIjoiMCIsImVtYWlsIjoiQXNoaXNoMTUyNTFAbWFzdGVrLmNvbSIsImZhbWlseV9uYW1lIjoiU2hlbGFyIiwiZ2l2ZW5fbmFtZSI6IkFzaGlzaCIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2FkZDFjNTAwLWE2ZDctNGRiZC1iODkwLTdmOGNiNmY3ZDg2MS8iLCJpcGFkZHIiOiI0OS4zMi4xODQuMTAyIiwibmFtZSI6IkFzaGlzaCBTaGVsYXIiLCJvaWQiOiIwODllNWE2OS1iNWJmLTRhMGItOWZlNS01NjNlNzg2NDNlMzciLCJyaCI6IjAuQVFJQVNNbzBrVDFtQlVxV2lqR2tMd3J0UGlUZ1c0QUlvdnRBcTI4NW5DWkgwelFDQUIwLiIsInJvbGVzIjpbIkJhdGNoQ3JlYXRlIl0sInNjcCI6IlVzZXIuUmVhZCIsInN1YiI6IjNRZWRRZ2xGSHZYbDNJck9ueVJ1TXNKWWI0UHF2NjV6VUdpeTZRRDJoaVUiLCJ0aWQiOiI5MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UiLCJ1bmlxdWVfbmFtZSI6IkFzaGlzaDE1MjUxQG1hc3Rlay5jb20iLCJ1dGkiOiJmUTg5a2hEVlpVdVNtanJkQWY5TUFBIiwidmVyIjoiMS4wIn0.t41JEFJ85YvOHJndY4uLM-8WgubfVhemwuOLMT_GKzrQ0ZFL5SLBQ12sEnTjydH3FzOoDHv2h5rA0p3qyJw0b369ftqeGqJ4t033MGYtfMXMRV8YfFEph0EQ5340-ocLZpTSHnr9vxH-8x_9erOBjUIKvgPmkkpvRmTFbQrYkCCUUEE0cyIlbRzwS4fSA5qdhmUTqZbKQcSym1krXpj7PhhYPXWBk7qJrLFpmC0pZWMUVNlyW9Y_PBPcC_pQU42T3-cy8Im_SBQK040LAV_s78m-Cq8Vc0o2J6IVvyG_Uhkh6CwbsFNcQqLZmDuYhGsK3OOp2vPSiNyWPhFIzJGv_w";

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
