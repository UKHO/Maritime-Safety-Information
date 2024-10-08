﻿using System.Text;
using Microsoft.Extensions.Options;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;


namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class FileShareService(IOptions<FileShareServiceConfiguration> fileShareServiceConfig, ILogger<FileShareService> logger) : IFileShareService
    {
        private readonly IOptions<FileShareServiceConfiguration> _fileShareServiceConfig = fileShareServiceConfig;
        private readonly ILogger<FileShareService> _logger = logger;

        public async Task<IResult<BatchSearchResponse>> FSSBatchSearchAsync(string searchText, string accessToken, string correlationId, IFileShareApiClient fileShareApiClient)
        {
            try
            {
                string searchQuery = $"BusinessUnit eq '{_fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{_fileShareServiceConfig.Value.ProductType}' " + searchText;

                _logger.LogInformation(EventIds.FSSBatchSearchResponseStarted.ToEventId(), "Maritime safety information request for FSS to get NM batch search response started for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);

                IResult<BatchSearchResponse> result = await fileShareApiClient.SearchAsync(searchQuery, _fileShareServiceConfig.Value.PageSize, _fileShareServiceConfig.Value.Start, CancellationToken.None);

                _logger.LogInformation(EventIds.FSSBatchSearchResponseCompleted.ToEventId(), "Maritime safety information request for FSS to get NM batch search response completed for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSBatchSearchResponseFailed.ToEventId(), "Failed to get batch search response from FSS with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<IResult<BatchAttributesSearchResponse>> FSSSearchAttributeAsync(string accessToken, string correlationId, IFileShareApiClient fileShareApiClient)
        {
            try
            {
                string searchQuery = $"BusinessUnit eq '{_fileShareServiceConfig.Value.BusinessUnit}' and $batch(Product Type) eq '{_fileShareServiceConfig.Value.ProductType}' and $batch(Frequency) eq 'Weekly'";

                _logger.LogInformation(EventIds.FSSSearchAttributeResponseStarted.ToEventId(), "Maritime safety information request for FSS to get NM batch search attribute response started for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);

                IResult<BatchAttributesSearchResponse> result = await fileShareApiClient.BatchAttributeSearchAsync(searchQuery, _fileShareServiceConfig.Value.MaxAttributeValuesCount, CancellationToken.None);

                _logger.LogInformation(EventIds.FSSSearchAttributeResponseCompleted.ToEventId(), "Maritime safety information request for FSS to get NM batch search attribute response completed for correlationId:{correlationId} and searchQuery:{searchQuery}", correlationId, searchQuery);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSSearchAttributeResponseError.ToEventId(), "Failed to get NM batch search attribute with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", ex.Message, correlationId);
                throw;
            }
        }

        public async Task<Stream> FSSDownloadFileAsync(string batchId, string fileName, string accessToken, string correlationId, IFileShareApiClient fileShareApiClient, string frequency)
        {
            try
            {
                _logger.LogInformation(EventIds.FSSGetSingleNMFileStarted.ToEventId(), "Maritime safety information request for FSS to get single {frequency} NM file started for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", frequency, batchId, fileName, correlationId);

                Stream stream = await fileShareApiClient.DownloadFileAsync(batchId, fileName);

                _logger.LogInformation(EventIds.FSSGetSingleNMFileCompleted.ToEventId(), "Maritime safety information request for FSS to get single {frequency} NM file completed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", frequency, batchId, fileName, correlationId);
                return stream;
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSGetSingleNMFileResponseFailed.ToEventId(), "Failed to get single {frequency} NM file from FSS for batchId:{batchId} and fileName:{fileName} with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", frequency, batchId, fileName, ex.Message, correlationId);
                throw;
            }
        }

        public async Task<Stream> FSSDownloadZipFileAsync(string batchId, string fileName, string accessToken, string correlationId, IFileShareApiClient fileShareApiClient)
        {
            try
            {
                _logger.LogInformation(EventIds.FSSGetNMZipFileStarted.ToEventId(), "Maritime safety information request for FSS to get NM zip file started for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);

                IResult<Stream> stream = await fileShareApiClient.DownloadZipFileAsync(batchId, CancellationToken.None);
                if (stream.IsSuccess)
                {
                    _logger.LogInformation(EventIds.FSSGetNMZipFileCompleted.ToEventId(), "Maritime safety information request for FSS to get NM zip file completed for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", batchId, fileName, correlationId);
                    return stream.Data;
                }

                _logger.LogInformation(EventIds.FSSGetNMZipFileReturnIsSuccessFalse.ToEventId(), "Maritime safety information request for FSS to get NM zip file returns IsSuccess false with StatusCode {StatusCode} for batchId:{batchId} and fileName:{fileName} with _X-Correlation-ID:{correlationId}", stream.StatusCode, batchId, fileName, correlationId);

                if (stream.Errors.Count > 0)
                {
                    StringBuilder error = new();
                    foreach (Error item in stream.Errors)
                        error.AppendLine(item.Description);

                    _logger.LogInformation(EventIds.FSSDownloadZipFileAsyncHasError.ToEventId(), "Maritime safety information request for FSS to get NM zip file has error for batchId:{batchId} and fileName:{fileName} with error:{error} for _X-Correlation-ID:{correlationId}", batchId, fileName, error, correlationId);
                    throw new ArgumentException(error.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.FSSGetNMZipFileResponseFailed.ToEventId(), "Failed to get NM zip file from FSS for batchId:{batchId} and fileName:{fileName} with exception:{exceptionMessage} for _X-Correlation-ID:{CorrelationId}", batchId, ex.Message, fileName, correlationId);
                throw;
            }
            return null;
        }
    }
}
