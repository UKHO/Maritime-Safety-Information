using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    public class FSSHealthClient : IFSSHealthClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;

        public FSSHealthClient(IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig)
        {
            _httpClientFactory = httpClientFactory;
            _fileShareServiceConfig = fileShareServiceConfig;
        }
        public async Task<HealthCheckResult> CheckHealthAsync()
        {
            try
            {
                string accessToken = await GenerateADAccessToken();
                IFileShareApiClient fileShareApiClient = new FileShareApiClient(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                IResult<BatchAttributesSearchResponse> result = await FSSSearchAttributeAsync(fileShareApiClient);

                if (result.IsSuccess)
                {
                    return HealthCheckResult.Healthy("FSS for maritime safety information is healthy");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("FSS for maritime safety information is unhealthy");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("FSS for maritime safety information is unhealthy", new Exception(ex.Message));
            }
        }

        public async Task<IResult<BatchAttributesSearchResponse>> FSSSearchAttributeAsync(IFileShareApiClient fileShareApiClient)
        {
            string searchQuery = $"BusinessUnit eq '{_fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{_fileShareServiceConfig.Value.ProductType}' and $batch(Frequency) eq 'Weekly'";

            IResult<BatchAttributesSearchResponse> result = await fileShareApiClient.BatchAttributeSearch(searchQuery, CancellationToken.None);

            return result;
        }

        private async Task<string> GenerateADAccessToken()
        {
            DefaultAzureCredential azureCredential = new();
            TokenRequestContext tokenRequestContext = new(new string[] { _fileShareServiceConfig.Value.FssClientId + "/.default" });
            AccessToken tokenResult = await azureCredential.GetTokenAsync(tokenRequestContext);
            return tokenResult.Token;
        }
    }
}
