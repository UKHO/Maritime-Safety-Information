using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Security.Claims;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private const string DistributorRoleName = "TBC";

        [Test]
        public void WhenUserIsUnauthenticated_ThenIsDistributorReturnsFalse()
        {
            DefaultHttpContext httpContext = new();

            HttpContextAccessor mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new(mockHttpContextAccessor);

            Assert.AreEqual(false, userService.IsDistributorUser);
        }

        [Test]
        public void WhenUserNotInRole_ThenIsDistributorReturnsFalse()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.Role, "Test Role 1")
                }, "mock"));

            DefaultHttpContext httpContext = new()
            {
                User = user
            };

            HttpContextAccessor mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new(mockHttpContextAccessor);

            Assert.AreEqual(false, userService.IsDistributorUser);
        }

        [Test]
        public void WhenUserHasRole_ThenIsDistributorReturnsTrue()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "Test User"),
                   new Claim(ClaimTypes.Role, DistributorRoleName)
                }, "mock"));

            DefaultHttpContext httpContext = new()
            {
                User = user
            };

            HttpContextAccessor mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new(mockHttpContextAccessor);

            Assert.AreEqual(true, userService.IsDistributorUser);
        }
    }
}
