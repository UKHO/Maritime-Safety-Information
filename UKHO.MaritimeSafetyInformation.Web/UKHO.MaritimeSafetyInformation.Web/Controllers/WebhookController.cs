using System.Text;
using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Logging;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class WebhookController : BaseController<WebhookController>
    {
        public WebhookController(IHttpContextAccessor contextAccessor, ILogger<WebhookController> logger) : base(contextAccessor, logger)
        {
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
        public virtual async Task<IActionResult> NewFilesPublished()
        {
            ////Adding payload to log temporarily so that in can be used for unit testing etc. 
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string payload = await reader.ReadToEndAsync();
            Logger.LogInformation(EventIds.ClearFSSSearchCacheEventStart.ToEventId(), "Clear FSS Search Cache Event started for _X-Correlation-ID:{correlationId} Payload: {payload}", GetCurrentCorrelationId(), payload);

            return GetCacheResponse(); 
        }
    }
}
