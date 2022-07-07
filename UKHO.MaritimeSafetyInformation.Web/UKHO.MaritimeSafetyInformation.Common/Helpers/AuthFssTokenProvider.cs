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
                    //DefaultAzureCredential azureCredential = new();
                    //TokenRequestContext tokenRequestContext = new(new string[] { _fileShareServiceConfiguration.Value.FssClientId + "/.default" });
                    //AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);
                    //return tokenResult.Token;

                    return "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjU3MTg2MDk1LCJuYmYiOjE2NTcxODYwOTUsImV4cCI6MTY1NzE5MDkzNiwiYWNyIjoiMSIsImFpbyI6IkFZUUFlLzhUQUFBQWpObXdqcUZ3K2FxSmZ5eXNxV253RlJObFVROXFaVURsN2doU2RTbGFibXJMbWhIcjZPWXdnR21hdHhNR0M4SE5VeHlVSjBudnN1TkEzSnNwZFQyV0Z0TXBOdUJCU3g1bDhleGhHQUQ2cnhoZFFObUE1MHc0bVlvblpRekdWNDRPanlCM29ZczdJSlU5MDB0WExkcWl4VVpzbDJzRFNXQ1dBazNsT0FmN1hWRT0iLCJhbXIiOlsicHdkIiwibWZhIl0sImFwcGlkIjoiODA1YmUwMjQtYTIwOC00MGZiLWFiNmYtMzk5YzI2NDdkMzM0IiwiYXBwaWRhY3IiOiIwIiwiZW1haWwiOiJtb2hhbW1lMTUzMTVAbWFzdGVrLmNvbSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2FkZDFjNTAwLWE2ZDctNGRiZC1iODkwLTdmOGNiNmY3ZDg2MS8iLCJpcGFkZHIiOiIxMDMuMTc4LjE2OS4xMTQiLCJuYW1lIjoiTW9oYW1tZWQgS2hhbiIsIm9pZCI6IjAxMzA1NWNiLWVkNjItNDZkMi1hZGU4LTM4ZmNjZDY0MGFhNiIsInJoIjoiMC5BUUlBU01vMGtUMW1CVXFXaWpHa0x3cnRQaVRnVzRBSW92dEFxMjg1bkNaSDB6UUNBQTQuIiwicm9sZXMiOlsiQmF0Y2hDcmVhdGUiXSwic2NwIjoiVXNlci5SZWFkIiwic3ViIjoiZ2RwR3VBN3VTZk9HY0RuS2VmZk41MXZBUDZXa2hKNldxbHdOaVpSMk9qNCIsInRpZCI6IjkxMzRjYTQ4LTY2M2QtNGEwNS05NjhhLTMxYTQyZjBhZWQzZSIsInVuaXF1ZV9uYW1lIjoibW9oYW1tZTE1MzE1QG1hc3Rlay5jb20iLCJ1dGkiOiJEd2ItUEoyWE9VT3J5SHM5QUxnTkFBIiwidmVyIjoiMS4wIn0.X7GKDjdjooJeT1Gw6uUe8Vmqa1BsggqOOMXExrpusryn4sUpMWW8ADIdjTAeL7IXua9Jn7-jaJzXdQXRvL2pz56YhvlLWLn4gE-gPJbKIisX2QvL_HguVFzNh_IStqq3h6cO17KXc8-kUnT1S3nv5PVZsQYWgrE7tGkg0o-YdgBHVSx3af_VBRD8qM5ZcsD8rfIxDTQOtoy6xmHJsQS-7nKYYQqZrg5eApH2RAO90X1H32z8LcCENWS_BIsQhdyVYDaUAQm1bcXB7KcN1JKJqUbZWvv941YMhtlvTefu7_gbdhUxB65jBZhAhN26DINBEa_s3Dd76mqQxyOJsrvrCg";
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
