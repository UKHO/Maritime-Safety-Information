using System.Security.Claims;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DefaultHttpContext _httpContext;
        private const string DistributorRoleName = "Distributor";

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserService(DefaultHttpContext httpContext) => _httpContext = httpContext;
       
        public bool IsDistributorUser
        {
            get {
                return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated
                    && _httpContextAccessor.HttpContext.User.IsInRole(DistributorRoleName);
            }
        }

        public string UserIdentifier
        {
            get
            {
                return Convert.ToString(_httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier"));
            }
        }

        public string SignInName
        {
            get
            {
               return Convert.ToString(_httpContextAccessor.HttpContext.User.FindFirstValue("signInName"));
            }
        }
    }
}
