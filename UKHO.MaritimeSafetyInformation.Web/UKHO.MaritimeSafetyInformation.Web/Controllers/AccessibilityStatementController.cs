using Microsoft.AspNetCore.Mvc;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class AccessibilityStatementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
