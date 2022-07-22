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
        private readonly IHttpContextAccessor _contextAccessor;

        public WebhookController(IHttpContextAccessor contextAccessor, ILogger<WebhookController> logger, IWebhookService webhookService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _webhookService = webhookService;
        }

        [HttpOptions]
        [Route("/webhook/newfilespublished")]
        public IActionResult NewFilesPublishedOptions()
        {
            string webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();

            Logger.LogInformation(EventIds.NewFilesPublishedWebhookOptionsCallStarted.ToEventId(), "Started processing the Options request for the New Files Published event webhook for WebHook-Request-Origin:{webhookRequestOrigin}", webhookRequestOrigin);

            HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
            HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);

            Logger.LogInformation(EventIds.NewFilesPublishedWebhookOptionsCallCompleted.ToEventId(), "Completed processing the Options request for the New Files Published event webhook for WebHook-Request-Origin:{webhookRequestOrigin}", webhookRequestOrigin);

            return GetCacheResponse();
        }

        [HttpPost]
        [Route("/webhook/newfilespublished")]
        public virtual async Task<IActionResult> NewFilesPublished([FromBody] JObject request)
        {
            Logger.LogInformation(EventIds.ClearFSSSearchCacheEventStarted.ToEventId(), "Clear FSS search cache event started for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());

            var eventGridEvent = new EventGridEvent();
            JsonConvert.PopulateObject(request.ToString(), eventGridEvent);
            EnterpriseEventCacheDataRequest data = (eventGridEvent.Data as JObject).ToObject<EnterpriseEventCacheDataRequest>();

            Logger.LogInformation(EventIds.ClearFSSSearchCacheEventStarted.ToEventId(), "Enterprise event data deserialized in ESS and Data:{data} and _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(data), GetCurrentCorrelationId());

            var validationResult = await _webhookService.ValidateEventGridCacheDataRequest(data);

            var productName = data.Attributes.Where(a => a.Key == "CellName").Select(a => a.Value).FirstOrDefault();

            if (!validationResult.IsValid)
            {
                Logger.LogInformation(EventIds.ClearFSSSearchCacheValidationEvent.ToEventId(), "Required attributes missing in event data from Enterprise event for clear FSS search cache from Azure table for _X-Correlation-ID:{correlationId}", GetCurrentCorrelationId());
                Logger.LogInformation(EventIds.ClearFSSSearchCacheEventCompleted.ToEventId(), "Clear FSS search cache event completed for ProductName:{productName} as required data was missing in payload with OK response and _X-Correlation-ID:{correlationId}", productName, GetCurrentCorrelationId());
                return GetCacheResponse();
            }

            await _webhookService.DeleteSearchAndDownloadCacheData(data, GetCurrentCorrelationId());

            Logger.LogInformation(EventIds.ClearFSSSearchCacheEventCompleted.ToEventId(), "Clear FSS search cache event completed for ProductName:{productName} with OK response and _X-Correlation-ID:{correlationId}", productName, GetCurrentCorrelationId());

            return GetCacheResponse();
        }
    }
}
