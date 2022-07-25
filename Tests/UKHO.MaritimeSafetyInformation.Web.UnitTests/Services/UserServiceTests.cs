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
        private const string DistributorRoleName = "Distributor";
        
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
                    new Claim(ClaimTypes.Name, "Distributor"),
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

        [Test]
        public void WhenUserIsAuthenticated_ThenReturnsSignInNameAndUserIdentifier()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
               {
                     new Claim("SignInName", "TestUser"),
                     new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "f457520b-0f1c-44cc-9b5c-2113e2ba1234")
               }, "mock"));

            DefaultHttpContext httpContext = new()
            {
                User = user
            };

            HttpContextAccessor mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new(mockHttpContextAccessor);

            Assert.AreEqual("TestUser", userService.SignInName);
            Assert.AreEqual("f457520b-0f1c-44cc-9b5c-2113e2ba1234", userService.UserIdentifier);

        }
    }
}
