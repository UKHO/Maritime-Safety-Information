using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsAdminControllerTest
    {
        private RadioNavigationalWarningsAdminController _controller;
        private IRnwRepository _fakeRnwRepository;

        [SetUp]
        public void Setup()
        {
            _fakeRnwRepository = A.Fake<IRnwRepository>();

            _controller = new RadioNavigationalWarningsAdminController(_fakeRnwRepository);
        }

        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            Task<IActionResult> result = _controller.Index();
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        [Test]
        public void WhenICallIndexViewWithParameters_ThenReturnView()
        {
            Task<IActionResult> result = _controller.Index(pageIndex:1,warningType:1,year:"2020");
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }
    }
}
