using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsAdminControllerTest
    {
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRnwService _fakeRnwService;

        private RadioNavigationalWarningsAdminController _controller;

        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _fakeRnwService = A.Fake<IRnwService>();

            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _fakeRnwService);
        }

        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            A.CallTo(() => _fakeRnwService.GetRadioNavigationWarningsForAdmin(1, 0, null, string.Empty)).Returns(GetFakeRadioNavigationWarningsForAdmin());
            Task<IActionResult> result = _controller.Index();
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        [Test]
        public void WhenICallIndexViewWithParameters_ThenReturnView()
        {
            A.CallTo(() => _fakeRnwService.GetRadioNavigationWarningsForAdmin(1, 0, null, string.Empty)).Returns(GetFakeRadioNavigationWarningsForAdmin());
            Task<IActionResult> result = _controller.Index(pageIndex: 1, warningType: 1, year: 2020);
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        private static RadioNavigationalWarningsAdminListFilter GetFakeRadioNavigationWarningsForAdmin()
        {
            return new RadioNavigationalWarningsAdminListFilter
            {
                WarningTypes = new List<WarningType>() { new WarningType { Id = 1, Name = "Test" } },
                Years = new List<string>() { "2020", "2021" },
            };
        }

        #region Edit RadioNavigation
        [Test]
        public void WhenICallEditView_ThenReturnView()
        {
            int id = 5;
            Task<IActionResult> result = _controller.Edit(id);
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        [Test]
        public void WhenEditRadioNavigationWarningsReturnTrueInRequest_ThenRecordIsUpdated()
        {
            int id = 5;
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, A.Fake<ITempDataProvider>());
            _controller.TempData = tempData;

            A.CallTo(() => _fakeRnwService.EditRadioNavigationWarningsRecord(A<RadioNavigationalWarningsAdminList>.Ignored, A<string>.Ignored)).Returns(true);
            Task<IActionResult> result = _controller.Edit(id, new RadioNavigationalWarningsAdminList( ) { Id = 5});

            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.AreEqual("Record updated successfully!", _controller.TempData["message"].ToString());
        }
        #endregion
    }
}
