using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsAdminControllerTest
    {
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRnwRepository _fakeRnwRepository;

        private RadioNavigationalWarningsAdminController _controller;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _fakeRnwRepository = A.Fake<IRnwRepository>();

            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _fakeRnwRepository);
        }

        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, false, string.Empty)).Returns(GetFakeRadioNavigationWarningsForAdmin());
            Task<IActionResult> result = _controller.Index();
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        [Test]
        public void WhenICallIndexViewWithParameters_ThenReturnView()
        {
            A.CallTo(() => _fakeRnwRepository.GetRadioNavigationWarningsForAdmin(1, 0, string.Empty, false, string.Empty)).Returns(GetFakeRadioNavigationWarningsForAdmin());
            Task<IActionResult> result = _controller.Index(pageIndex: 1, warningType: 1, year: "2020");
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        private RadioNavigationalWarningsAdminListFilter GetFakeRadioNavigationWarningsForAdmin()
        {
            return new RadioNavigationalWarningsAdminListFilter
            {
                WarningTypes = new List<WarningType>() { new WarningType { Id = 1, Name = "Test" } },
                Years = new List<string>() { "2020", "2021" },
            };
        }
    }
}
