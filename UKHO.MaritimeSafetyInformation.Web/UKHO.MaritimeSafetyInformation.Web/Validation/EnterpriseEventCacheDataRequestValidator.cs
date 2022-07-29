using FluentValidation;
using FluentValidation.Results;
using System.Net;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;

namespace UKHO.MaritimeSafetyInformation.Web.Validation
{

    public interface IEnterpriseEventCacheDataRequestValidator
    {
        Task<ValidationResult> Validate(FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest);
    }

    public class EnterpriseEventCacheDataRequestValidator : AbstractValidator<FSSNewFilesPublishedEventData>, IEnterpriseEventCacheDataRequestValidator
    {
        
        public EnterpriseEventCacheDataRequestValidator()
        {
            RuleFor(v => v.BusinessUnit).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Business unit cannot be blank or null.");

            RuleFor(v => v.Attributes).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Attributes cannot be blank or null.");

            RuleFor(b => b.Attributes)
              .Must(at => at.All(a => !string.IsNullOrWhiteSpace(a.Key) && !string.IsNullOrWhiteSpace(a.Value))).OverridePropertyName("Attributes")
              .When(ru => ru.Attributes != null)
              .WithErrorCode(HttpStatusCode.OK.ToString())
              .WithMessage("Attribute key or value cannot be null.");
        }
        Task<ValidationResult> IEnterpriseEventCacheDataRequestValidator.Validate(FSSNewFilesPublishedEventData enterpriseEventCacheDataRequest)
        {
            return ValidateAsync(enterpriseEventCacheDataRequest);
        }
    }
}
