extern alias MSIAdminProjectAlias;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
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
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings.Admin
{
    [TestFixture]
    internal class RadioNavigationalWarningCreateAdminTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsAdminController> fakeLogger;
        private IRNWRepository rnwRepository;
        private IRNWService rnwService;
        private TempDataDictionary tempData;
        private RadioNavigationalWarning fakeRadioNavigationalWarning;
        private ILogger<RNWService> fakeLoggerRnwService;

        private RadioNavigationalWarningsAdminController controller;
        private readonly ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "Admin User"), }, "mock"));

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsAdminController>>();
            fakeLoggerRnwService = A.Fake<ILogger<RNWService>>();
            rnwRepository = new RNWRepository(FakeContext);
            rnwService = new RNWService(rnwRepository, FakeRadioNavigationalWarningConfiguration, fakeLoggerRnwService);
            tempData = new(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
            controller = new RadioNavigationalWarningsAdminController(FakeHttpContextAccessor, fakeLogger, rnwService);
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        }

        [Test]
        public async Task WhenCallCreate_ThenReturnViewAsync()
        {
            IActionResult result = await controller.Create();
            Assert.That(result, Is.InstanceOf<IActionResult>());
            Assert.That(((ViewResult)result).ViewData["WarningType"], Is.Not.Null);
        }

        [Test]
        public async Task WhenCallAddRadioNavigationalWarnings_ThenNewRecordIsCreated()
        {
            controller.TempData = tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            IActionResult result = await controller.Create(GetFakeRadioNavigationalWarning());

            Assert.That(result, Is.InstanceOf<IActionResult>());
            Assert.That("Record created successfully!", Is.EqualTo(controller.TempData["message"].ToString()));
            Assert.That("Index", Is.EqualTo(((RedirectToActionResult)result).ActionName));
            Assert.That(1, Is.EqualTo(FakeContext.RadioNavigationalWarnings.ToListAsync().Result.Count));
            Assert.That(FakeContext.RadioNavigationalWarnings.ToListAsync().Result[0].LastModified < DateTime.Now);
        }

        [Test]
        public async Task WhenCallCreateWithExistingReferenceNumberAndSkipDuplicateReferenceCheckFlagAsNo_ThenReturnAlertMessage()
        {
            controller.TempData = tempData;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Create.cshtml";
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            RadioNavigationalWarning radioNavigationalWarning = GetFakeRadioNavigationalWarning();
            radioNavigationalWarning.Id++;
            radioNavigationalWarning.IsDeleted = false;

            await SeedRadioNavigationalWarnings(new List<RadioNavigationalWarning>() { radioNavigationalWarning });

            IActionResult result = await controller.Create(radioNavigationalWarning);

            Assert.That(result, Is.InstanceOf<IActionResult>());
            Assert.That("A warning record with this reference number already exists. Would you like to add another record with the same reference?", Is.EqualTo(controller.TempData["message"].ToString()));
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        [Test]
        public async Task WhenCallCreateWithExistingReferenceNumberAndSkipDuplicateReferenceCheckFlagAsYes_ThenNewRecordIsCreated()
        {
            controller.TempData = tempData;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "Yes" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            await DeSeedRadioNavigationalWarnings();

            RadioNavigationalWarning radioNavigationalWarning = GetFakeRadioNavigationalWarning();
            radioNavigationalWarning.Id++;
            radioNavigationalWarning.IsDeleted = false;

            await SeedRadioNavigationalWarnings(new List<RadioNavigationalWarning>() { radioNavigationalWarning });

            RadioNavigationalWarning radioNavigationalWarningNewEntry = GetFakeRadioNavigationalWarning();
            radioNavigationalWarningNewEntry.Id += 2;
            radioNavigationalWarningNewEntry.IsDeleted = false;

            IActionResult result = await controller.Create(radioNavigationalWarningNewEntry);

            Assert.That(result, Is.InstanceOf<IActionResult>());
            Assert.That("Record created successfully!", Is.EqualTo(controller.TempData["message"].ToString()));
            Assert.That("Index", Is.EqualTo(((RedirectToActionResult)result).ActionName));
            Assert.That(2, Is.EqualTo(FakeContext.RadioNavigationalWarnings.ToListAsync().Result.Count));
            Assert.That(FakeContext.RadioNavigationalWarnings.ToListAsync().Result[0].LastModified < DateTime.Now);
        }

        [Test]
        public void WhenAddRadioNavigationalWarningsWithInValidValue_ThenNewRecordIsNotCreated()
        {
            controller.TempData = tempData;
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.Reference = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            Task<IActionResult> result = controller.Create(fakeRadioNavigationalWarning);
            Assert.That(result, Is.InstanceOf<Task<IActionResult>>());
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                async delegate { await controller.Create(fakeRadioNavigationalWarning); });
        }

        [Test]
        public async Task WhenAddRadioNavigationWarningsWithInValidModel_ThenNewRecordIsNotCreated()
        {
            controller.TempData = tempData;
            const string expectedView = "~/Views/RadioNavigationalWarningsAdmin/Create.cshtml";
            controller.ModelState.AddModelError("WarningType", "In Valid WarningType Selected");
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            IActionResult result = await controller.Create(new RadioNavigationalWarning());
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(((ViewResult)result).ViewData.ModelState.IsValid, Is.False);
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidWarningType_ThenReturnInValidDataException()
        {
            controller.TempData = tempData;
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.WarningType = 3;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid value received for parameter warningType"),
                                async delegate { await controller.Create(fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidReference_ThenReturnArgumentNullException()
        {
            controller.TempData = tempData;
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.Reference = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter reference"),
                async delegate { await controller.Create(fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidSummary_ThenReturnArgumentNullException()
        {
            controller.TempData = tempData;
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.Summary = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await controller.Create(fakeRadioNavigationalWarning); });
        }

        [Test]
        public void WhenAddRadioNavigationWarningsWithInValidContent_ThenReturnArgumentNullException()
        {
            controller.TempData = tempData;
            fakeRadioNavigationalWarning = GetFakeRadioNavigationalWarning();
            fakeRadioNavigationalWarning.Content = string.Empty;
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"SkipDuplicateReferenceCheck", "No" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;


            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>(),
                                async delegate { await controller.Create(fakeRadioNavigationalWarning); });
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}
