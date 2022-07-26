using FluentValidation;
using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;

namespace UKHO.MaritimeSafetyInformation.Web.Validation
{
    
    public interface IEnterpriseEventCacheDataRequestValidator
    {
        Task<ValidationResult> Validate(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest);
    }

    public class EnterpriseEventCacheDataRequestValidator : AbstractValidator<EnterpriseEventCacheDataRequest>, IEnterpriseEventCacheDataRequestValidator
    {
        
        public EnterpriseEventCacheDataRequestValidator()
        {
            RuleFor(v => v.BatchId).NotNull().NotEmpty()
               .WithErrorCode(HttpStatusCode.OK.ToString())
               .WithMessage("BatchId cannot be blank or null.");

            RuleFor(v => v.BusinessUnit).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Business unit cannot be blank or null.");

            RuleFor(v => v.BatchPublishedDate).NotNull().NotEmpty()
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Batch published date cannot be blank or null.");

            RuleFor(v => v.Links).NotNull().NotEmpty().OverridePropertyName("Links")
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("Links detail cannot be blank or null.");

            RuleFor(v => v.Links.BatchDetails).NotNull().NotEmpty().OverridePropertyName("BatchDetails")
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Links != null)
                .WithMessage("Links batch detail cannot be blank or null.");

            RuleFor(v => v.Links.BatchStatus).NotNull().NotEmpty().OverridePropertyName("BatchStatus")
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Links != null)
                .WithMessage("Links batch status cannot be blank or null.");

            RuleFor(v => v.Links.BatchDetails.Href).NotNull().NotEmpty().OverridePropertyName("BatchDetailsUri")
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Links != null && x.Links.BatchDetails != null)
                .WithMessage("Links batch detail uri cannot be blank or null.");

            RuleFor(v => v.Links.BatchStatus.Href).NotNull().NotEmpty().OverridePropertyName("BatchStatusUri")
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Links != null && x.Links.BatchStatus != null)
                .WithMessage("Links batch status uri cannot be blank or null.");

            RuleFor(v => v.Files).NotNull().NotEmpty().OverridePropertyName("Links")
                .Must(at => at.All(a => a.Links != null && a.Links.Get != null && a.Links.Get.Href != null && a.Links.Get.Href != ""))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .WithMessage("File links cannot be blank or null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("MimeType")
                .Must(at => at.All(a => a.MimeType != null))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Files != null)
                .WithMessage("File Mime type cannot be null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("Hash")
                .Must(at => at.All(a => a.Hash != null))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Files != null)
                .WithMessage("File hash cannot be null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("Filename")
                .Must(at => at.All(a => a.Filename != null))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Files != null)
                .WithMessage("File name cannot be null.");

            RuleFor(b => b.Files).NotNull().NotEmpty().OverridePropertyName("FileSize")
                .Must(at => at.All(a => a.FileSize > 0))
                .WithErrorCode(HttpStatusCode.OK.ToString())
                .When(x => x.Files != null)
                .WithMessage("File size cannot be null.");
        }
        Task<ValidationResult> IEnterpriseEventCacheDataRequestValidator.Validate(EnterpriseEventCacheDataRequest enterpriseEventCacheDataRequest)
        {
            return ValidateAsync(enterpriseEventCacheDataRequest);
        }
    }
}
