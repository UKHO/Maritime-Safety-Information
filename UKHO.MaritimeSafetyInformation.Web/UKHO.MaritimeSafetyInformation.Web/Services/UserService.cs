﻿using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string DistributorRoleName = "TBC";

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsDistributorUser
        {
            get {
                return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated
                    && _httpContextAccessor.HttpContext.User.IsInRole(DistributorRoleName);
            }
        }
    }
}