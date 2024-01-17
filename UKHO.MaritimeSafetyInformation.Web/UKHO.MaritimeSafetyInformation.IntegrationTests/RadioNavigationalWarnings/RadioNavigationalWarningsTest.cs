using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.RadioNavigationalWarnings
{
    [TestFixture]
    internal class RadioNavigationalWarningsTest : BaseRNWTest
    {
        private ILogger<RadioNavigationalWarningsController> fakeLogger;
        private IRNWRepository rnwRepository;
        private RNWService rnwService;
        private RadioNavigationalWarningsController controller;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await SeedRadioNavigationalWarnings(GetFakeRadioNavigationalWarnings());
            await SeedWarningType(GetFakeWarningTypes());
        }

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<RadioNavigationalWarningsController>>();
            rnwRepository = new RNWRepository(FakeContext);
            rnwService = new RNWService(rnwRepository, FakeRadioNavigationalWarningConfiguration, FakeLoggerRnwService);

            controller = new RadioNavigationalWarningsController(FakeHttpContextAccessor, fakeLogger, rnwService);
        }

        [Test]
        public async Task WhenCallGetRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            IActionResult result = await controller.Index();
            List<RadioNavigationalWarningsData> warningsData = (List<RadioNavigationalWarningsData>)((ViewResult)result).Model;
            string lastModifiedDateTime = ((ViewResult)result).ViewData["LastModifiedDateTime"].ToString();
            Assert.That(7, Is.EqualTo(warningsData.Count));
            Assert.That("RnwAdminListReference", Is.EqualTo(warningsData[2].Reference));
            Assert.That("RnwAdminListSummary", Is.EqualTo(warningsData[2].Description));
            Assert.That(new DateTime(2022, 1, 1), Is.EqualTo(warningsData[2].DateTimeGroup));
            Assert.That("010000 UTC Jan 22", Is.EqualTo(warningsData[2].DateTimeGroupRnwFormat));
            Assert.That("151415 UTC Aug 19", Is.EqualTo(lastModifiedDateTime));
        }

        [Test]
        public async Task WhenCallAbout_ThenReturnLastModifiedDateTime()
        {
            IActionResult result = await controller.About();
            string lastModifiedDateTime = ((ViewResult)result).ViewData["LastModifiedDateTime"].ToString();
            Assert.That("151415 UTC Aug 19", Is.EqualTo(lastModifiedDateTime));
        }

        [Test]
        public async Task WhenCallGetSelectedRadioNavigationalWarningsDataList_ThenReturnOnlyNonDeletedAndNonExpiredWarnings()
        {
            DefaultHttpContext httpContext = new();
            FormCollection formCol = new(new Dictionary<string, StringValues>
                                        {
                                            {"showSelectionId", "11" }
                                        });
            httpContext.Request.Form = formCol;
            controller.ControllerContext.HttpContext = httpContext;

            IActionResult result = await controller.ShowSelection();
            List<RadioNavigationalWarningsData> warningsData = (List<RadioNavigationalWarningsData>)((ViewResult)result).Model;
            string lastModifiedDateTime = ((ViewResult)result).ViewData["LastModifiedDateTime"].ToString();
            Assert.That(1, Is.EqualTo(warningsData.Count));
            Assert.That("RnwAdminListReference", Is.EqualTo(warningsData[0].Reference));
            Assert.That("RnwAdminListSummary", Is.EqualTo(warningsData[0].Description));
            Assert.That(new DateTime(2021, 1, 1), Is.EqualTo(warningsData[0].DateTimeGroup));
            Assert.That("010000 UTC Jan 21", Is.EqualTo(warningsData[0].DateTimeGroupRnwFormat));
            Assert.That("151415 UTC Aug 19", Is.EqualTo(lastModifiedDateTime));
        }

        [OneTimeTearDown]
        public async Task GlobalTearDown()
        {
            await DeSeedRadioNavigationalWarnings();
            await DeSeedWarningType();
        }
    }
}
