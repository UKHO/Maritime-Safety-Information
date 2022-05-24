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
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Helpers;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    class NoticeToMarinersControllersTest : NMTestHelper
    {
       ////// private INMDataService _nMDataService;

        private NoticesToMarinersController _nMController;
        private Configuration TestConfig { get; set; }
        private FileShareServiceApiClient fileShareServiceApiClient;
        [SetUp]
        public void Setup()
        {
             ////// _nMDataService = new NMDataService(_fakeFileShareService, _fakeLoggerNMDataService, _fakeAuthFssTokenProvider);
            TestConfig = new Configuration();

            fileShareServiceApiClient = new FileShareServiceApiClient(TestConfig.BaseUrl);
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
            Task<IResult<BatchAttributesSearchResponse>> fssResponse = fileShareServiceApiClient.BatchAttributeSearch(TestConfig.BaseUrl, TestConfig.BusinessUnit, TestConfig.ProductType);
            Console.WriteLine(fssResponse.Result);
            IActionResult result = await _nMController.Index();
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsTrue(showWeeklyFiles != null);
        }
    }
}
