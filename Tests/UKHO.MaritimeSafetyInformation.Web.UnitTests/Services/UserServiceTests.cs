using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Services;
using FakeItEasy;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        [Test]
        public void WhenUserIsUnautheticated_ThenIsDistributorReturnsFalse()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();

            var mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new UserService(mockHttpContextAccessor);

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

            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = user
            };

            var mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new UserService(mockHttpContextAccessor);

            Assert.AreEqual(false, userService.IsDistributorUser);
        }

        [Test]
        public void WhenUserHasRole_ThenIsDistributorReturnsTrue()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.Role, UserService.DISTRIBUTOR_ROLE_NAME)
                }, "mock"));

            DefaultHttpContext httpContext = new DefaultHttpContext()
            {
                User = user
            };

            var mockHttpContextAccessor = A.Fake<HttpContextAccessor>();
            mockHttpContextAccessor.HttpContext = httpContext;

            UserService userService = new UserService(mockHttpContextAccessor);

            Assert.AreEqual(true, userService.IsDistributorUser);
        }
    }
}
