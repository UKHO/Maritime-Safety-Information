using Microsoft.AspNetCore.Mvc;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class AccessibilityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
