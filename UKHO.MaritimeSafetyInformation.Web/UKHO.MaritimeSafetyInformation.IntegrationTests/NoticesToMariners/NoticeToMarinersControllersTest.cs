using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    class NoticeToMarinersControllersTest : NmTestsHelper
    {
        private INMDataService _nMDataService;

        private NoticesToMarinersController _nMController;

        [SetUp]
        public void Setup()
        {
            _nMDataService = new NMDataService(_fakeFileShareService, _fakeLoggerNMDataService, _fakeAuthFssTokenProvider);
            //////// _nMController = new NoticesToMarinersController(_nMDataService, _fakeHttpContextAccessor, _fakeLogger);
            IServiceCollection serviceCollection = BuildDefaultServiceCollection();

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            _nMController = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(serviceProvider);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnList()
        {
            _nMController.ControllerContext.HttpContext = new DefaultHttpContext();

            ////////A.CallTo(() => _fakeHttpContextAccessor.HttpContext).Returns(context);
            ////////_nMController.ControllerContext = new ControllerContext()
            ////////{
            ////////    HttpContext = context
            ////////};

            IActionResult result = await _nMController.Index();
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsTrue(showWeeklyFiles != null);
        }
    }
}
