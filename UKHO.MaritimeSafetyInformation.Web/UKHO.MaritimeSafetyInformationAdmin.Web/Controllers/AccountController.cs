using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UKHO.MaritimeSafetyInformationAdmin.Web.Controllers
{
    [AllowAnonymous]
    [Area("MsiIdentity")]
    [Route("[area]/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        /// <summary>
        /// Handles the user sign-out.
        /// </summary>
        public new IActionResult SignOut()
        {
            string callbackUrl = Url.Action("SignedOut");
            return SignOut(
                 new AuthenticationProperties
                 {
                     RedirectUri = callbackUrl,
                 },
                 CookieAuthenticationDefaults.AuthenticationScheme,
                 OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult SignedOut()
        {
            foreach (string cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            _contextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Index", "RadioNavigationalWarningsAdmin");
        }
    }
}
