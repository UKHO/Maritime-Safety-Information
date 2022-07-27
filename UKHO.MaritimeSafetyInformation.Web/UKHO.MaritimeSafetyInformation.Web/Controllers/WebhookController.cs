using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
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

        public WebhookController(IHttpContextAccessor contextAccessor, ILogger<WebhookController> logger, IWebhookService webhookService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _webhookService = webhookService;
        }

        [HttpOptions]
        [Route("/webhook/newfilespublished")]
        public IActionResult NewFilesPublishedOptions()
        {
            string webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();

            _logger.LogInformation(EventIds.NewFilesPublishedWebhookOptionsCallStarted.ToEventId(), "Started processing the Options request for the New Files Published event webhook for WebHook-Request-Origin:{webhookRequestOrigin}", webhookRequestOrigin);

            HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
            HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);

            _logger.LogInformation(EventIds.NewFilesPublishedWebhookOptionsCallCompleted.ToEventId(), "Completed processing the Options request for the New Files Published event webhook for WebHook-Request-Origin:{webhookRequestOrigin}", webhookRequestOrigin);

            return GetCacheResponse();
        }

        [HttpPost]
        [Route("/webhook/newfilespublished")]
        public virtual async Task<IActionResult> NewFilesPublished()
        {
            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            string payload = await reader.ReadToEndAsync();

            _logger.LogInformation(EventIds.ClearFSSSearchCacheEventStarted.ToEventId(), "Clear FSS search cache event started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            EventGridEvent eventGridEvent = new();
            JsonConvert.PopulateObject(payload, eventGridEvent);
            EnterpriseEventCacheDataRequest data = (eventGridEvent.Data as JObject).ToObject<EnterpriseEventCacheDataRequest>();

            _logger.LogInformation(EventIds.ClearFSSSearchCacheEventStarted.ToEventId(), "Enterprise event data deserialized. Data:{data} and _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(data), GetCurrentCorrelationId());

            FluentValidation.Results.ValidationResult validationResult = await _webhookService.ValidateEventGridCacheDataRequest(data);

            string productName = data.Attributes.Where(a => a.Key == "Product Type").Select(a => a.Value).FirstOrDefault();

            if (!validationResult.IsValid)
            {
                _logger.LogInformation(EventIds.ClearFSSSearchCacheValidationEvent.ToEventId(), "Required attributes missing in event data from Enterprise event for clear FSS search cache from Azure table for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                return GetCacheResponse();
            }

            bool isCacheDeleted = await _webhookService.DeleteSearchAndDownloadCacheData(data, GetCurrentCorrelationId());
            if (!isCacheDeleted)
            {
                _logger.LogInformation(EventIds.ClearFSSSearchCacheEventCompleted.ToEventId(), "Event triggered for different ProductName/Business Unit. ProductName:{productName} Business Unit: {businessUnit} and _X-Correlation-ID:{correlationId}", productName, data.BusinessUnit, GetCurrentCorrelationId());
            }
            else { 
                _logger.LogInformation(EventIds.ClearFSSSearchCacheEventCompleted.ToEventId(), "Clear FSS search cache event completed for ProductName:{productName} with OK response and _X-Correlation-ID:{correlationId}", productName, GetCurrentCorrelationId());
            }
            return GetCacheResponse();
        }
    }
}
