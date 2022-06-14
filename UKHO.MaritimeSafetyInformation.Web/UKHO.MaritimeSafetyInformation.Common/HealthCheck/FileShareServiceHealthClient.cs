using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    [ExcludeFromCodeCoverage]
    public class FileShareServiceHealthClient : IFileShareServiceHealthClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig;
        private readonly IAuthFssTokenProvider _authFssTokenProvider;

        public FileShareServiceHealthClient(IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig, IAuthFssTokenProvider authFssTokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _fileShareServiceConfig = fileShareServiceConfig;
            _authFssTokenProvider = authFssTokenProvider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken)
        {
            try
            {
                string accessToken = await _authFssTokenProvider.GenerateADAccessToken(Guid.NewGuid().ToString());
                FileShareApiClient fileShareApiClient = new(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                IResult<BatchSearchResponse> result = await FSSSearchAsync(fileShareApiClient, cancellationToken);

                if (result.IsSuccess)
                {
                    return HealthCheckResult.Healthy("File Share Service is healthy");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("File Share Service is unhealthy", new Exception("Batch search response is not success, Please check configuration."));
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("File Share Service is unhealthy", new Exception(ex.Message));
            }
        }

        private async Task<IResult<BatchSearchResponse>> FSSSearchAsync(IFileShareApiClient fileShareApiClient, CancellationToken cancellationToken)
        {
            string searchQuery = $"BusinessUnit eq 'invalid'";
            IResult<BatchSearchResponse> result = await fileShareApiClient.Search(searchQuery, _fileShareServiceConfig.Value.PageSize, _fileShareServiceConfig.Value.Start, cancellationToken);
            return result;
        }
    }
}
