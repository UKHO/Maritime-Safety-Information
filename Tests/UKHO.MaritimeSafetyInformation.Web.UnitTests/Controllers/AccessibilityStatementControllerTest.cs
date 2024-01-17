using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class AccessibilityStatementControllerTest
    {
        private AccessibilityStatementController controller;

        [SetUp]
        public void Setup()
        {
            controller = new AccessibilityStatementController();
        }

        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            IActionResult result = controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }
    }
}
