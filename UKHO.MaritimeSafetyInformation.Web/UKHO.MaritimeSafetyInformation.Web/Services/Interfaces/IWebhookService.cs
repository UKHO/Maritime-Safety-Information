using FluentValidation.Results;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IWebhookService
    {
        Task<ValidationResult> ValidateNewFilesPublishedEventData(FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest);
        Task<bool> DeleteBatchSearchResponseCacheData(FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest, string correlationId);
    }
}
