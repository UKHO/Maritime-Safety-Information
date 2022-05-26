using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    class NoticeToMarinersControllersTest
    {
        readonly IServiceProvider _services = Program.CreateHostBuilder(Array.Empty<string>()).Build().Services;

        private NoticesToMarinersController _nMController;

        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context;
        private Configuration Config { get; set; }

        [SetUp]
        public void Setup()
        {
            Config = new Configuration();
            _context = new DefaultHttpContext();
            _contextAccessor = new HttpContextAccessor
            {
                HttpContext = _context
            };
            _nMController = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(_services);
        }

        [Test]
        public async Task WhenCallIndexOnLoad_ThenReturnList()
        {
            IActionResult result = await _nMController.Index();
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;

            Assert.IsTrue(showWeeklyFiles != null);
            Assert.AreEqual(4, showWeeklyFiles.YearAndWeekList.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(2020, showWeeklyFiles.YearAndWeekList[0].Year);
            Assert.AreEqual(14, showWeeklyFiles.YearAndWeekList[0].Week);
        }

        [Test]
        public async Task WhenCallIndex_ThenReturnYearWeek()
        {
            IActionResult result = await _nMController.Index(2021, 30);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsTrue(showWeeklyFiles != null);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(2021, showWeeklyFiles.YearAndWeekList[1].Year);
            Assert.AreEqual(30, showWeeklyFiles.YearAndWeekList[1].Week);
        }

        [Test]
        public async Task WhenCallIndexWithIncorrectData_ThenReturnNoData()
        {
            IActionResult result = await _nMController.Index(2021, 08);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.AreEqual(0, showWeeklyFiles.ShowFilesResponseList.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsync_ThenReturnWeeklyFiles()
        {
            IActionResult result = await _nMController.ShowWeeklyFilesAsync(2020, 14);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.IsTrue(listFiles != null);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
           Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual("a738d0d3-bc1e-47ca-892a-9514ccef6464", listFiles[1].BatchId);
            Assert.AreEqual("msi_img_W2020_14.jpg", listFiles[1].Filename);
            Assert.AreEqual(".jpg", listFiles[1].FileExtension);
            Assert.AreEqual(108480, listFiles[1].FileSize);
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncwithInvalidData_ThenReturnNull()
        {
            IActionResult result = await _nMController.ShowWeeklyFilesAsync(2022, 6);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(0, listFiles.Count);
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsync_ThenReturnWeeklyFiles()
        {
            IActionResult result = await _nMController.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.IsTrue(showFiles != null);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(4, showFiles[0].DailyFilesData.Count);
            Assert.AreEqual("22", showFiles[0].WeekNumber);
            Assert.AreEqual("2021", showFiles[0].Year);
            Assert.AreEqual("Daily .zip", showFiles[0].DailyFilesData[0].Filename);
            Assert.AreEqual("1 MB", showFiles[0].DailyFilesData[0].FileSizeInKB);
            Assert.AreEqual("155e7fe4-5866-4a31-9994-113beca0dce1", showFiles[0].DailyFilesData[0].BatchId);
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsyncWithInvalidData_ThenReturnFile()
        {
            string batchId = "a738d0d3-bc1e-47ca-892a-9514ccef6464";
            string filename = "21snii22_week_W2020_14.pdf";
            string mimeType = "application/pdf";

            FileResult result = await _nMController.DownloadWeeklyFile(batchId, filename, mimeType);
            Assert.IsTrue(result != null);
            Assert.AreEqual("application/pdf", result.ContentType);
           Assert.AreEqual("https://filesqa.admiralty.co.uk", Config.BaseUrl);           
        }
    }
}
