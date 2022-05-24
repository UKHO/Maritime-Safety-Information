using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

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
            //////////A.CallTo(() => _fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            //////////_controller = new NoticesToMarinersController(_fakeNMService, _fakeContextAccessor, _fakeLogger);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnList()
        {
            _nMController.ControllerContext.HttpContext = new DefaultHttpContext();
            
         //////   Task<IResult<BatchAttributesSearchResponse>> fssResponse = fileShareServiceApiClient.BatchAttributeSearch(TestConfig.BusinessUnit, TestConfig.ProductType);
            
            IActionResult result = await _nMController.Index();
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsTrue(showWeeklyFiles != null);
        }
    }
}
