using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Filters;

namespace UKHO.MaritimeSafetyInformation.Web.ViewComponents
{
    [ExcludeFromCodeCoverage]
    public class BaseViewComponent<T> : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger<T> Logger;

        protected BaseViewComponent(IHttpContextAccessor httpContextAccessor, ILogger<T> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            Logger = logger;
        }

        protected string GetCurrentCorrelationId()
        {
            return _httpContextAccessor.HttpContext.Request.Headers[UkhoHeaderNames.XCorrelationId].FirstOrDefault();
        }
    }
}
