using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Helpers;

namespace UKHO.MaritimeSafetyInformation.Common.HealthCheck
{
    [ExcludeFromCodeCoverage]
    public class FileShareServiceHealthClient(IHttpClientFactory httpClientFactory, IOptions<FileShareServiceConfiguration> fileShareServiceConfig, IAuthFssTokenProvider authFssTokenProvider) : IFileShareServiceHealthClient
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig = fileShareServiceConfig;
        private readonly IAuthFssTokenProvider _authFssTokenProvider = authFssTokenProvider;

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken)
        {
            try
            {
                var accessToken = await _authFssTokenProvider.GenerateADAccessToken(false, Guid.NewGuid().ToString());
                FileShareApiClient fileShareApiClient = new(_httpClientFactory, _fileShareServiceConfig.Value.BaseUrl, accessToken);

                var result = await FSSSearchAsync(fileShareApiClient, cancellationToken);

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

        private async Task<IResult<BatchSearchResponse>> FSSSearchAsync(FileShareApiClient fileShareApiClient, CancellationToken cancellationToken)
        {
            const string searchQuery = $"BusinessUnit eq 'invalid'";
            var result = await fileShareApiClient.SearchAsync(searchQuery, _fileShareServiceConfig.Value.PageSize, _fileShareServiceConfig.Value.Start, cancellationToken);
            return result;
        }
    }
}
