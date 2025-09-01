using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    [ExcludeFromCodeCoverage]
    public class BaseController<T> : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger<T> Logger;

        protected BaseController(IHttpContextAccessor httpContextAccessor, ILogger<T> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            Logger = logger;
        }

        protected string GetCurrentCorrelationId()
        {
            return _httpContextAccessor.HttpContext.Request.Headers[UkhoHeaderNames.XCorrelationId].FirstOrDefault();
        }

        protected IActionResult GetCacheResponse()
        {
            return new OkObjectResult(StatusCodes.Status200OK);
        }
    }
}
