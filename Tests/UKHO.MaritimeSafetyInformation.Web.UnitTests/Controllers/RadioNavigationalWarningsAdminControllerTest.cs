extern alias MSIAdminProjectAlias;
using FakeItEasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsAdminControllerTest
    {
        private RadioNavigationalWarningsAdminController _controller;
        private IHttpContextAccessor _fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWService _fakeRnwService;
        private TempDataDictionary _tempData;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";
        private readonly ClaimsPrincipal _user = new(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Admin User"), }, "mock"));


        [SetUp]
        public void Setup()
        {
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _fakeRnwService = A.Fake<IRNWService>();
            _tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
            _controller = new RadioNavigationalWarningsAdminController(_fakeHttpContextAccessor, _fakeLogger, _fakeRnwService);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = _user };
        }


        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            Task<IActionResult> result = _controller.Index();
            Assert.IsInstanceOf<Task<IActionResult>>(result);
        }

        [Test]
        public async Task WhenICallCreateView_ThenReturnView()
        {
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Create.cshtml";
            IActionResult result = await _controller.Create();
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsReturnTrueInRequest_ThenNewRecordIsCreated()
        {
            _controller.TempData = _tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

             A.CallTo(() => _fakeRnwService.CreateNewRadioNavigationWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<string>.Ignored)).Returns(new ResponseNewRadioNavigationWarningsModel() { IsCreated = true});
            Task<IActionResult> result = _controller.Create(new RadioNavigationalWarning());
            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.AreEqual("Record created successfully!", _controller.TempData["message"].ToString());
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithFlagSkipCheckDuplicateIsNo_ThenNewRecordIsCreated()
        {
            _controller.TempData = _tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "No" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _fakeRnwService.CreateNewRadioNavigationWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<string>.Ignored)).Returns(new ResponseNewRadioNavigationWarningsModel() { IsCreated = true });
            Task<IActionResult> result = _controller.Create(new RadioNavigationalWarning());
            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.AreEqual("Record created successfully!", _controller.TempData["message"].ToString());
        }

        [Test]
        public void WhenAddRadioNavigationWarningsReturnFalseInRequest_ThenNewRecordIsNotCreated()
        {
            _controller.TempData = _tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "No" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _fakeRnwService.CreateNewRadioNavigationWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<string>.Ignored)).Returns(new ResponseNewRadioNavigationWarningsModel() { IsCreated = false });
            Task<IActionResult> result = _controller.Create(new RadioNavigationalWarning());
            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.AreEqual("A warning record with this reference number already exists. Would you like to add another record with the same reference?", _controller.TempData["message"].ToString());
        }

        [Test]
        public async Task WhenICallGetRadioNavigationWarningsForAdmin_ThenReturnViewAndViewData()
        {
            A.CallTo(() => _fakeRnwService.GetRadioNavigationWarningsForAdmin(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(GetFakeRadioNavigationWarningsForAdmin());
            IActionResult result = await _controller.Index(pageIndex: 1, warningType: 1, year: 2020);
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.IsNotNull(((ViewResult)result).ViewData["WarningTypes"]);
            Assert.IsNotNull(((ViewResult)result).ViewData["Years"]);
        }

        [Test]
        public void WhenICallGetRadioNavigationWarningsForAdmin_ThenCheckIfUserHasCorrectRole()
        {
            object[] actualAttribute = _controller.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);
            object role = actualAttribute.GetValue(0);
            _ = _controller.Index();
            Assert.AreEqual(typeof(AuthorizeAttribute), actualAttribute[0].GetType());
            Assert.AreEqual("rnw-admin", ((AuthorizeAttribute)role).Roles);
        }

        [Test]
        public async Task WhenICallEditView_ThenReturnView()
        {
            const int id = 5;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Edit.cshtml";
            IActionResult result = await _controller.Edit(id);
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        [Test]
        public void WhenEditRadioNavigationalWarningsRecordReturnTrueInRequest_ThenRecordIsUpdated()
        {
            _controller.TempData = _tempData;
            A.CallTo(() => _fakeRnwService.EditRadioNavigationalWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored)).Returns(true);
            Task<IActionResult> result = _controller.Edit(new RadioNavigationalWarning() { Id = 5 });
            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.AreEqual("Record updated successfully!", _controller.TempData["message"].ToString());
        }

        [Test]
        public async Task WhenICallRadioNavigationWarningListForAdmin_ThenReturnView()
        {
            const int id = 5;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Edit.cshtml";
            A.CallTo(() => _fakeRnwService.GetRadioNavigationalWarningById(5, CorrelationId)).Returns(GetEditFakeRadioNavigationWarningForAdmin());
            IActionResult result = await _controller.Edit(id);
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
        }

        private static RadioNavigationalWarningsAdminFilter GetFakeRadioNavigationWarningsForAdmin()
        {
            return new RadioNavigationalWarningsAdminFilter
            {
                WarningTypes = new List<WarningType>() { new WarningType { Id = 1, Name = "Test" } },
                Years = new List<string>() { "2020", "2021" },
            };
        }

        private static RadioNavigationalWarning GetEditFakeRadioNavigationWarningForAdmin()
        {
            return new RadioNavigationalWarning
            {
                WarningType = 1,
                Reference = "test",
                DateTimeGroup = DateTime.UtcNow,
                Summary = "Test1",
                Content = "test"
             };
        }

    }
}
