extern alias MSIAdminProjectAlias;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class RadioNavigationalWarningsAdminControllerTest
    {
        private RadioNavigationalWarningsAdminController controller;
        private IHttpContextAccessor fakeHttpContextAccessor;
        private ILogger<RadioNavigationalWarningsAdminController> fakeLogger;
        private IRNWService fakeRnwService;
        private TempDataDictionary tempData;
        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";
        private readonly ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Admin User"), }, "mock"));


        [SetUp]
        public void Setup()
        {
            fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();
            fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            fakeRnwService = A.Fake<IRNWService>();
            tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
            controller = new RadioNavigationalWarningsAdminController(fakeHttpContextAccessor, fakeLogger, fakeRnwService);
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        }


        [Test]
        public void WhenICallIndexView_ThenReturnView()
        {
            Task<IActionResult> result = controller.Index();
            Assert.That(result, Is.InstanceOf<Task<IActionResult>>());
        }

        [Test]
        public async Task WhenICallCreateView_ThenReturnView()
        {
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Create.cshtml";
            IActionResult result = await controller.Create();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
        }

        [Test]
        public void WhenAddRadioNavigationWarningsReturnTrueInRequest_ThenNewRecordIsCreated()
        {
            controller.TempData = tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => fakeRnwService.CreateNewRadioNavigationWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<string>.Ignored)).Returns(true);
            Task<IActionResult> result = controller.Create(new RadioNavigationalWarning());
            Assert.That(result, Is.InstanceOf<Task<IActionResult>>());
            Assert.That("Record created successfully!", Is.EqualTo(controller.TempData["message"].ToString()));
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithFlagSkipDuplicateReferenceCheckIsNo_ThenNewRecordIsCreated()
        {
            controller.TempData = tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => fakeRnwService.CreateNewRadioNavigationWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<string>.Ignored)).Returns(true);
            Task<IActionResult> result = controller.Create(new RadioNavigationalWarning());
            Assert.That(result, Is.InstanceOf<Task<IActionResult>>());
            Assert.That("Record created successfully!", Is.EqualTo(controller.TempData["message"].ToString()));
        }

        [Test]
        public void WhenAddRadioNavigationWarningsReturnFalseInRequest_ThenNewRecordIsNotCreated()
        {
            controller.TempData = tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => fakeRnwService.CreateNewRadioNavigationWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored, A<bool>.Ignored, A<string>.Ignored)).Returns(false);
            Task<IActionResult> result = controller.Create(new RadioNavigationalWarning());
            Assert.That(result, Is.InstanceOf<Task<IActionResult>>());
            Assert.That("A warning record with this reference number already exists. Would you like to add another record with the same reference?", Is.EqualTo(controller.TempData["message"].ToString()));
        }

        [Test]
        public async Task WhenICallGetRadioNavigationWarningsForAdmin_ThenReturnViewAndViewData()
        {
            A.CallTo(() => fakeRnwService.GetRadioNavigationWarningsForAdmin(A<int>.Ignored, A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(GetFakeRadioNavigationWarningsForAdmin());
            IActionResult result = await controller.Index(pageIndex: 1, warningType: 1, year: 2020);
            Assert.That(result, Is.InstanceOf<IActionResult>());
            Assert.That(((ViewResult)result).ViewData["WarningTypes"], Is.Not.Null);
            Assert.That(((ViewResult)result).ViewData["Years"], Is.Not.Null);
        }

        [Test]
        public void WhenICallGetRadioNavigationWarningsForAdmin_ThenCheckIfUserHasCorrectRole()
        {
            object[] actualAttribute = controller.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);
            object role = actualAttribute.GetValue(0);
            _ = controller.Index();
            Assert.That(typeof(AuthorizeAttribute), Is.EqualTo(actualAttribute[0].GetType()));
            Assert.That("rnw-admin", Is.EqualTo(((AuthorizeAttribute)role).Roles));
        }

        [Test]
        public async Task WhenICallEditView_ThenReturnView()
        {
            const int id = 5;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Edit.cshtml";
            IActionResult result = await controller.Edit(id);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
        }

        [Test]
        public void WhenEditRadioNavigationalWarningsRecordReturnTrueInRequest_ThenRecordIsUpdated()
        {
            controller.TempData = tempData;
            A.CallTo(() => fakeRnwService.EditRadioNavigationalWarningsRecord(A<RadioNavigationalWarning>.Ignored, A<string>.Ignored)).Returns(true);
            Task<IActionResult> result = controller.Edit(new RadioNavigationalWarning() { Id = 5 });
            Assert.That(result, Is.InstanceOf<Task<IActionResult>>());
            Assert.That("Record updated successfully!", Is.EqualTo(controller.TempData["message"].ToString()));
        }

        [Test]
        public async Task WhenICallRadioNavigationWarningListForAdmin_ThenReturnView()
        {
            const int id = 5;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Edit.cshtml";
            A.CallTo(() => fakeRnwService.GetRadioNavigationalWarningById(5, CorrelationId)).Returns(GetEditFakeRadioNavigationWarningForAdmin());
            IActionResult result = await controller.Edit(id);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
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
