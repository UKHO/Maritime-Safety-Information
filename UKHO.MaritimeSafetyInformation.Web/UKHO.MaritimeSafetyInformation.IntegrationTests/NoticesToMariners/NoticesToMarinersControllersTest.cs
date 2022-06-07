using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
   internal class NoticesToMarinersControllersTest
    {
        private readonly IServiceProvider _services = Program.CreateHostBuilder(Array.Empty<string>()).Build().Services;
        private NoticesToMarinersController _nMController;

        private Configuration Config { get; set; }

        [SetUp]
        public void Setup()
        {
            Config = new Configuration();
            _ = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
            _nMController = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(_services);
        }

        [Test]
        public async Task WhenCallIndexOnLoad_ThenReturnList()
        {
            IActionResult result = await _nMController.Index();
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsNotNull(showWeeklyFiles);
            Assert.AreEqual(6, showWeeklyFiles.YearAndWeekList.Count);
            Assert.AreEqual(9, showWeeklyFiles.ShowFilesResponseList.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(2022, showWeeklyFiles.YearAndWeekList[3].Year);
            Assert.AreEqual(05, showWeeklyFiles.YearAndWeekList[3].Week);
            Assert.AreEqual("image/jpg", showWeeklyFiles.ShowFilesResponseList[3].MimeType);
        }

        [Test]
        public async Task WhenCallIndexWithYearWeek_ThenReturnList()
        {
            IActionResult result = await _nMController.Index(2021, 30);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsNotNull(showWeeklyFiles);
            Assert.AreEqual(4, showWeeklyFiles.ShowFilesResponseList.Count);
            Assert.AreEqual(6, showWeeklyFiles.YearAndWeekList.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual("msi_img_W2021_30.jpg", showWeeklyFiles.ShowFilesResponseList[1].Filename);
            Assert.AreEqual(".jpg", showWeeklyFiles.ShowFilesResponseList[1].FileExtension);
            Assert.AreEqual("msi_img_W2021_30", showWeeklyFiles.ShowFilesResponseList[1].FileDescription);
            Assert.AreEqual(2021, showWeeklyFiles.YearAndWeekList[1].Year);
            Assert.AreEqual(30, showWeeklyFiles.YearAndWeekList[1].Week);
        }

        [Test]
        public async Task WhenCallIndexForWeekWithNoData_ThenReturnNoData()
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
            Assert.IsNotNull(listFiles);
            Assert.AreEqual(4, listFiles.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual("a738d0d3-bc1e-47ca-892a-9514ccef6464", listFiles[0].BatchId);
            Assert.AreEqual("21snii22_week_W2020_14", listFiles[0].FileDescription);
            Assert.AreEqual(".pdf", listFiles[0].FileExtension);
            Assert.AreEqual(1072212, listFiles[0].FileSize);
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithNoData_ThenReturnEmptyList()
        {
            IActionResult result = await _nMController.ShowWeeklyFilesAsync(2022, 6);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(0, listFiles.Count);
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsync_ThenReturnDailyFiles()
        {
            IActionResult result = await _nMController.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((ViewResult)result).Model;
            Assert.IsNotNull(showFiles);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(18, showFiles[0].DailyFilesData.Count);
            Assert.AreEqual("44", showFiles[0].WeekNumber);
            Assert.AreEqual("2021", showFiles[0].Year);
            Assert.AreEqual("Daily .zip", showFiles[0].DailyFilesData[0].Filename);
            Assert.AreEqual("1 MB", showFiles[0].DailyFilesData[0].FileSizeInKB);
            Assert.AreEqual("155e7fe4-5866-4a31-9994-113beca0dce1", showFiles[0].DailyFilesData[0].BatchId);
        }

        [Test]
        public async Task WhenCallDownloadWeeklyFile_ThenReturnFile()
        {
            const string batchId = "a738d0d3-bc1e-47ca-892a-9514ccef6464";
            const string filename = "21snii22_week_W2020_14.pdf";
            const string mimeType = "application/pdf";

            FileResult result = await _nMController.DownloadWeeklyFile(batchId, filename, mimeType);
            Assert.IsNotNull(result);
            Assert.AreEqual("application/pdf", result.ContentType);
            Assert.AreEqual("https://filesqa.admiralty.co.uk", Config.BaseUrl);
            Assert.AreEqual(1072222, ((FileContentResult)result).FileContents.Length);
        }

        [Test]
        public void WhenCallDownloadWeeklyFileWithInvalidData_ThenReturnException()
        {
            const string batchId = "a738d0d3-bc1e-47ca-892a-9514ccef6464";
            const string filename = "Test.txt";
            const string mimeType = "application/txt";

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>()
               .And.Message.EqualTo("Response status code does not indicate success: 404 (Not Found).")
               , async delegate { await _nMController.DownloadWeeklyFile(batchId, filename, mimeType); });
        }

        [Test]
        public async Task WhenCallDownloadDailyFile_ThenReturnFile()
        {
            const string batchId = "44e8cce6-e69d-46bd-832d-6fd3d4ef8740";
            const string filename = "SERIAL_D2022_18.txt";
            const string mimeType = "application/text";

            ActionResult result = await _nMController.DownloadDailyFile(batchId, filename, mimeType);
            Assert.IsTrue(((FileContentResult)result) != null);
            Assert.AreEqual("application/text",((FileContentResult)result).ContentType);            
            Assert.AreEqual(1229033, ((FileContentResult)result).FileContents.Length);
            Assert.AreEqual("https://filesqa.admiralty.co.uk", Config.BaseUrl);
        }

        [Test]      
        public async Task WhenCallDownloadDailyFileWithInvalidData_ThenReturnNoData()
        {
            const string batchId = "08e8cce6-e69d-46bd-832d-6fd3d4ef8740";
            const string filename = "Test.txt";
            const string mimeType = "application/txt";

            ActionResult result = await _nMController.DownloadDailyFile(batchId, filename, mimeType);
            Assert.AreEqual(false,((RedirectToActionResult)result).PreserveMethod);
            Assert.AreEqual("ShowDailyFiles", ((RedirectToActionResult)result).ActionName);
        }
    }
}
