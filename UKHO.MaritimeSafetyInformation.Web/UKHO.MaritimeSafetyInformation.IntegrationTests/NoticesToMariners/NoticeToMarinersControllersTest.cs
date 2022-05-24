using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    class NoticeToMarinersControllersTest 
    {
        readonly IServiceProvider _services = Program.CreateHostBuilder(Array.Empty<string>()).Build().Services;

        private NoticesToMarinersController _nMController;
       
        [SetUp]
        public void Setup()
        {         
                     
            _nMController = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(_services);   
        }

        [Test]
        public async Task WhenCallIndexOnLoad_ThenReturnList()
        {
            _nMController.ControllerContext.HttpContext = new DefaultHttpContext();            
            IActionResult result = await _nMController.Index();
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsTrue(showWeeklyFiles != null);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnList()
        {
            _nMController.ControllerContext.HttpContext = new DefaultHttpContext();
            IActionResult result = await _nMController.Index(2021, 30);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsTrue(showWeeklyFiles != null);
        }

        [Test]
        public async Task WhenCallIndexWithIncorrectData_ThenReturnList()
        {
            _nMController.ControllerContext.HttpContext = new DefaultHttpContext();
            IActionResult result = await _nMController.Index(2021, 08);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsTrue(showWeeklyFiles.ShowFilesResponseList.Count == 0);
        }
    }
}
