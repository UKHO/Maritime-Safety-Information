extern alias MSIAdminProjectAlias;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;
using MSIAdminProjectAlias::UKHO.MaritimeSafetyInformationAdmin.Web.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningCreateAdminTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> _fakeLogger;
        private IRNWRepository _rnwRepository;
        private IRNWService _rnwService;
        private TempDataDictionary _tempData;
        private RadioNavigationalWarning _fakeRadioNavigationalWarning;
        private ILogger<RNWService> _fakeLoggerRnwService;

        private RadioNavigationalWarningsAdminController _controller;
        private readonly ClaimsPrincipal _user = new(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Admin User"), }, "mock"));

        [SetUp]
        public void Setup()
        {
            _fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            _fakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            _rnwRepository = new RNWRepository(FakeContext);
            _rnwService = new RNWService(_rnwRepository, FakeRadioNavigationalWarningConfiguration, _fakeLoggerRnwService);
            _tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
            _controller = new RadioNavigationalWarningsAdminController(FakeHttpContextAccessor, _fakeLogger, _rnwService);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = _user };
        }

        [Test]
        public async Task WhenCallCreate_ThenReturnViewAsync()
        {
            IActionResult result = await _controller.Create();
            Assert.IsInstanceOf<IActionResult>(result);
            Assert.IsNotNull(((ViewResult)result).ViewData["WarningType"]);
        }

        [Test]
        public async Task WhenCallAddRadioNavigationalWarnings_ThenNewRecordIsCreated()
        {
            _controller.TempData = _tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            IActionResult result = await _controller.Create(GetFakeRadioNavigationalWarning());

            Assert.IsInstanceOf<IActionResult>(result);
            Assert.AreEqual("Record created successfully!", _controller.TempData["message"].ToString());
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual(1, FakeContext.RadioNavigationalWarnings.ToListAsync().Result.Count);
            Assert.IsTrue(FakeContext.RadioNavigationalWarnings.ToListAsync().Result[0].LastModified < DateTime.Now);
        }

        [Test]
        public void WhenAddRadioNavigationalWarningsWithInValidValue_ThenNewRecordIsNotCreated()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Reference = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            Task<IActionResult> result = _controller.Create(_fakeRadioNavigationalWarning);
            Assert.IsInstanceOf<Task<IActionResult>>(result);
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public async Task WhenAddRadioNavigationWarningsWithInValidModel_ThenNewRecordIsNotCreated()
        {
            _controller.TempData = _tempData;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Create.cshtml";
            _controller.ModelState.AddModelError("WarningType", "In Valid WarningType Selected");
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            IActionResult result = await _controller.Create(new RadioNavigationalWarning());
            Assert.IsInstanceOf<ViewResult>(result);
            string actualView = ((ViewResult)result).ViewName;
            Assert.AreEqual(expectedView, actualView);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidWarningType_ThenReturnInValidDataException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.WarningType = 3;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidReference_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Reference = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidSummary_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Summary = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidContent_ThenReturnArgumentNullException()
        {
            _controller.TempData = _tempData;
            _fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            _fakeRadioNavigationalWarning.Content = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipCheckDuplicate", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            _controller.ControllerContext.HttpContext = httpContext;


            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await _controller.Create(_fakeRadioNavigationalWarning); });
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}
