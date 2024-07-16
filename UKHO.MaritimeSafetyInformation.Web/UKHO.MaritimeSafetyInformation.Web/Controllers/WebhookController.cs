using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UKHO.MaritimeSafetyInformation.Common.Logging;
using UKHO.MaritimeSafetyInformation.Common.Models.WebhookRequest;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class WebhookController : BaseController<WebhookController>
    {
        private readonly IWebhookService _webhookService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IHttpContextAccessor contextAccessor,
                                 ILogger<WebhookController> logger,
                                 IWebhookService webhookService)
        : base(contextAccessor, logger)
        {
            _logger = logger;
            _webhookService = webhookService;
        }

        [HttpOptions]
        [Route("/webhook/newfilespublished")]
        public IActionResult NewFilesPublishedOptions()
        {
            string webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();

            _logger.LogInformation(EventIds.NewFilesPublishedWebhookOptionsCallStarted.ToEventId(), "Started processing the Options request for the New Files Published event webhook for WebHook-Request-Origin:{webhookRequestOrigin} and _X-Correlation-ID:{correlationId}", webhookRequestOrigin, GetCurrentCorrelationId());

            HttpContext.Response.Headers.Append("WebHook-Allowed-Rate", "*");
            HttpContext.Response.Headers.Append("WebHook-Allowed-Origin", webhookRequestOrigin);

            _logger.LogInformation(EventIds.NewFilesPublishedWebhookOptionsCallCompleted.ToEventId(), "Completed processing the Options request for the New Files Published event webhook for WebHook-Request-Origin and _X-Correlation-ID:{correlationId}:{webhookRequestOrigin}", webhookRequestOrigin, GetCurrentCorrelationId());

            return GetCacheResponse();
        }

        [HttpPost]
        [Route("/webhook/newfilespublished")]
        public virtual async Task<IActionResult> NewFilesPublished()
        {
            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            string payload = await reader.ReadToEndAsync();

            _logger.LogInformation(EventIds.ClearFSSSearchCacheEventStarted.ToEventId(), "Clear FSS search cache event started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            if (string.IsNullOrEmpty(payload))
            {
                _logger.LogError(EventIds.ClearFSSSearchCacheValidationEvent.ToEventId(), "Payload is null or empty for Enterprise event _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return GetCacheResponse();
            }

            var payloadExtract = new PayloadExtract();
            JsonConvert.PopulateObject(payload, payloadExtract);

            if (payloadExtract.Data == null || payloadExtract.Data.ToString() == "" || payloadExtract.Data.ToString() == "{}")
            {
                _logger.LogError(EventIds.ClearFSSSearchCacheValidationEvent.ToEventId(), "Payload data is null for Enterprise event _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return GetCacheResponse();
            }

            FSSNewFilesPublishedEventData data = (payloadExtract.Data as JObject).ToObject<FSSNewFilesPublishedEventData>();

            _logger.LogInformation(EventIds.ClearFSSSearchCacheEventStarted.ToEventId(), "Enterprise event data deserialized. Data:{data} and _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(data), GetCurrentCorrelationId());

            FluentValidation.Results.ValidationResult validationResult = await _webhookService.ValidateNewFilesPublishedEventData(data);

            string productType = validationResult.IsValid ? data.Attributes.Where(a => a.Key == "Product Type").Select(a => a.Value).FirstOrDefault() : "";

            if (!validationResult.IsValid)
            {
                _logger.LogInformation(EventIds.ClearFSSSearchCacheValidationEvent.ToEventId(), "Required attributes missing in event data from Enterprise event for clear FSS search cache from Azure table for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                _logger.LogInformation(EventIds.ClearFSSSearchCacheEventCompleted.ToEventId(), "Clear Cache Event completed for Product Type:{productType} as required data was missing in payload with OK response and _X-Correlation-ID:{correlationId}", productType, GetCurrentCorrelationId());
                return GetCacheResponse();
            }

            bool isCacheDeleted = await _webhookService.DeleteBatchSearchResponseCacheData(data, GetCurrentCorrelationId());

            if (isCacheDeleted)
            {
                _logger.LogInformation(EventIds.ClearFSSSearchCacheEventCompleted.ToEventId(), "Clear FSS search cache event completed for Product Type:{productType} with OK response and _X-Correlation-ID:{correlationId}", productType, GetCurrentCorrelationId());
            }
            else
            {
                _logger.LogInformation(EventIds.ClearFSSSearchCacheEventCompleted.ToEventId(), "Event triggered for different Product Type/Business Unit. Product Type:{productType} Business Unit: {businessUnit} and _X-Correlation-ID:{correlationId}", productType, data.BusinessUnit, GetCurrentCorrelationId());
            }

            return GetCacheResponse();
        }
    }
}
