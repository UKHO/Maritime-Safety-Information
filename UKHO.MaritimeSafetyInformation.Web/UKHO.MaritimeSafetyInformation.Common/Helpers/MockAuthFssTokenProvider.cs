using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public class MockAuthFssTokenProvider : IAuthFssTokenProvider
    {
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfiguration;
        private readonly ILogger<AuthFssTokenProvider> _logger;
        private readonly IOptions<AzureAdB2C> _azureAdB2C;
        private readonly ITokenAcquisition _tokenAcquisition;

        public MockAuthFssTokenProvider(IOptions<FileShareServiceConfiguration> fileShareServiceConfiguration, ILogger<AuthFssTokenProvider> logger, IOptions<AzureAdB2C> azureAdB2C, ITokenAcquisition tokenAcquisition)
        {
            _fileShareServiceConfiguration = fileShareServiceConfiguration;
            _logger = logger;
            _azureAdB2C = azureAdB2C;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<string> GenerateADAccessToken(bool isDistributorUser, string correlationId)
        {
            try
            {
                if (isDistributorUser)
                {
                    return await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { _azureAdB2C.Value.Scope });
                }
                else
                {
                    var mockToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { _fileShareServiceConfiguration.Value.FssClientId + "/.default" });
                    return mockToken;

                }
            }
            catch (MicrosoftIdentityWebChallengeUserException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Mock AD Authentication failed with message:{ex} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                return string.Empty;
            }
        }

    }
}
