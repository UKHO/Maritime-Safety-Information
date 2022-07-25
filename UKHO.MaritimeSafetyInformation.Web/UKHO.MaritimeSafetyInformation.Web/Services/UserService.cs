using System.Security.Claims;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;       
        private const string DistributorRoleName = "Distributor";

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }       
       
        public bool IsDistributorUser
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated
                    && _httpContextAccessor.HttpContext.User.IsInRole(DistributorRoleName);
            }
        }

        public string UserIdentifier
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            }
        }

        public string SignInName
        {
            get
            {
               return _httpContextAccessor.HttpContext.User.FindFirstValue("signInName");
            }
        }
    }
}
