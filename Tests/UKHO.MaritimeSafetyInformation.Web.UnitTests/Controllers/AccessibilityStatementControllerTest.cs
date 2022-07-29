using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class AccessibilityStatementControllerTest
    {
        private AccessibilityStatementController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new AccessibilityStatementController();
        }

        [Test]
        public void  WhenICallIndexView_ThenReturnView()
        {
            IActionResult result = _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
