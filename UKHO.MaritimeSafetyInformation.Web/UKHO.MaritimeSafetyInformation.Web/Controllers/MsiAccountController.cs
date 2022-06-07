using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    [AllowAnonymous]
    [Area("MicrosoftIdentity")]
    [Route("[area]/[controller]/[action]")]
    public class MsiAccountController : Controller
    {
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
            //do any post logout activities here

            return new RedirectResult(Url.Content("/"));
        }
    }
}
