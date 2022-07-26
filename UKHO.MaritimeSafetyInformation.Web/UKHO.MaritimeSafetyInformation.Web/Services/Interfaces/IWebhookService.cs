using FluentValidation.Results;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;

namespace UKHO.MaritimeSafetyInformation.Web.Services.Interfaces
{
    public interface IWebhookService
    {
        Task<ValidationResult> ValidateEventGridCacheDataRequest(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest);
        Task<bool> DeleteSearchAndDownloadCacheData(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest, string correlationId);
    }
}
