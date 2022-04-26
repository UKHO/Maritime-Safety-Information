using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Contollers
{
    [TestFixture]
    public class NoticestoMarinersControllerTest
    {
        private NoticestoMarinersController controller;
        private INMDataService nMService;
        private ILogger<NoticestoMarinersController> logger;
        private IHttpClientFactory httpClientFactory;
        private IHttpContextAccessor contextAccessor;

        [SetUp]
        public void Setup()
        {
            nMService = A.Fake<INMDataService>();
            httpClientFactory = A.Fake<IHttpClientFactory>();
            logger = A.Fake<ILogger<NoticestoMarinersController>>();
            contextAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => contextAccessor.HttpContext).Returns(new DefaultHttpContext());
            controller = new NoticestoMarinersController(httpClientFactory, nMService, contextAccessor, logger);
        }

        [Test]
        public void DoesIndexReturnsView()
        {
            var result = controller.Index();
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public void DoesLoadYearsReturnsJson()
        {
            var result = controller.LoadYears();
            Assert.IsInstanceOf<JsonResult>(result);
        }
        [Test]
        public void DoesLoadWeeksReturnsJson()
        {
            int year = 2022;
            var result = controller.LoadWeeks(year);
            Assert.IsInstanceOf<JsonResult>(result);
        }
        [Test]
        public async Task DoesShowWeeklyFilesReturnPartialViewAsync()
        {
            int year = 2022;
            int week = 16;
            var result = await controller.ShowWeeklyFilesAsync(year,week);
            Assert.IsInstanceOf<PartialViewResult>(result);
        }

    }
}
