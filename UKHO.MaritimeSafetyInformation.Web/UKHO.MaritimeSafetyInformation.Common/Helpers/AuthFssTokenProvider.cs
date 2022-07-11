using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    [ExcludeFromCodeCoverage]
    public class AuthFssTokenProvider : IAuthFssTokenProvider
    {
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfiguration;
        private readonly ILogger<AuthFssTokenProvider> _logger;

        public AuthFssTokenProvider(IOptions<FileShareServiceConfiguration> fileShareServiceConfiguration, ILogger<AuthFssTokenProvider> logger)
        {
            _fileShareServiceConfiguration = fileShareServiceConfiguration;
            _logger = logger;
        }

        public async Task<string> GenerateADAccessToken(string correlationId)
        {
            try
            {
                //DefaultAzureCredential azureCredential = new();
                //TokenRequestContext tokenRequestContext = new(new string[] { _fileShareServiceConfiguration.Value.FssClientId + "/.default" });
                //AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);               
                //return tokenResult.Token;
                return "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiI4MDViZTAyNC1hMjA4LTQwZmItYWI2Zi0zOTljMjY0N2QzMzQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC85MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UvIiwiaWF0IjoxNjU3NTUwMTg3LCJuYmYiOjE2NTc1NTAxODcsImV4cCI6MTY1NzU1NTEzMSwiYWNyIjoiMSIsImFpbyI6IkFZUUFlLzhUQUFBQXk2K1FFMjQ0NU9GMWovMHc3SEFQT01VVytQKzI5emM0aS9BNmljbUdLY1NkUTl2S0NNeUpyZ2ptL2Q0VlVkanJGQTkrQTZoMzZTVGIxQndNdkJuLy9TYklXYlRYR2ZHOWF6ZE5LMnNSL0s5amdCeXhRb0wvQWdUQ1JLUEFFaE9DRnRpcUVjYmE4NjFlekRPMCtCNytKS2h6SDU2RGZPSG4zbmM0aFowSVpQcz0iLCJhbXIiOlsicHdkIiwibWZhIl0sImFwcGlkIjoiODA1YmUwMjQtYTIwOC00MGZiLWFiNmYtMzk5YzI2NDdkMzM0IiwiYXBwaWRhY3IiOiIwIiwiZW1haWwiOiJ2aXBpbjE0OTA0QG1hc3Rlay5jb20iLCJmYW1pbHlfbmFtZSI6Ik1haGFqYW4iLCJnaXZlbl9uYW1lIjoiVmlwaW4iLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9hZGQxYzUwMC1hNmQ3LTRkYmQtYjg5MC03ZjhjYjZmN2Q4NjEvIiwiaXBhZGRyIjoiNDkuMzIuMjM0Ljg3IiwibmFtZSI6IlZpcGluIE1haGFqYW4iLCJvaWQiOiI0ZWYwOGZhNS0wODdlLTRlMTEtOWQxMS1lZmU0NWJkYmRlZDIiLCJyaCI6IjAuQVFJQVNNbzBrVDFtQlVxV2lqR2tMd3J0UGlUZ1c0QUlvdnRBcTI4NW5DWkgwelFDQUlJLiIsInNjcCI6IlVzZXIuUmVhZCIsInN1YiI6IkQyUGhmZUVLTi02VGJuRjVQMUMzeU1sbDA4a1VjWFpCZkdqZS1ET21LTmsiLCJ0aWQiOiI5MTM0Y2E0OC02NjNkLTRhMDUtOTY4YS0zMWE0MmYwYWVkM2UiLCJ1bmlxdWVfbmFtZSI6InZpcGluMTQ5MDRAbWFzdGVrLmNvbSIsInV0aSI6IndESWlSYW1zLWtPcy1IYi1fRnd3QUEiLCJ2ZXIiOiIxLjAifQ.CSQ5il3GI1uhMgC8W1l_HQ5sz58qTVIJwj89ULDgsj8L6HoF4fdz7stoliptUGXN_3vrFQwcdwrlRSc6fBhVzSSXfrJq3jrXDQM61A76yHNKfXB4U7NWlvGD1dvZLF5cDbH7crZCMgcW7LoKWdmqLtHzweRgMAJxjOl7dE9WTAXuO31rBRtBuOzhKMkpN7rblIMt8XfLgHHgqg-zTb4ijETmWNcsSLuUZL9XjFg1siCPy9kQrhvFrbH_AdcRjIyZoAu0BrZ93o-3kMjxddfDTQpVhvw-SS5f9Z6kLpf60LMKkSe2QLzFUloTdisedLN-9Wm6HhJN4TrD6Zo9HlQFmQ";
            }
            catch (Exception ex)
            {
                _logger.LogInformation("AD Authentication failed with message:{ex} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                return string.Empty;
            }
        }
    }
}
