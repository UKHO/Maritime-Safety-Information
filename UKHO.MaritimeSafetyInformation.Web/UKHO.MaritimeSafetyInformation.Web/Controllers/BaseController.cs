using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Web.Filters;

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
            return _httpContextAccessor.HttpContext.Request.Headers[CorrelationIdMiddleware.XCorrelationIdHeaderKey].FirstOrDefault();
        }
    }
}
