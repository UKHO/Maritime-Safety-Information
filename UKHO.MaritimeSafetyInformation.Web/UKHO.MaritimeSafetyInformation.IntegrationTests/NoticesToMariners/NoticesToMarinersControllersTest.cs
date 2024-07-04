using System;
using System.Collections.Generic;
using System.Net.Http;
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
    /// <summary>
    /// These tests require data to be set up in the File Share Service.Instructions can be found on the MSI project Wiki :
    /// https://dev.azure.com/ukhydro/Maritime%20Safety%20Information/_wiki/wikis/Maritime-Safety-Information.wiki/329/MSI-Notices-to-Mariners-Integration-Tests
    /// </summary>
    internal class NoticesToMarinersControllersTest
    {
        private readonly IServiceProvider services = Program.CreateHostBuilder(Array.Empty<string>()).Build().Services;
        private NoticesToMarinersController nMController;

        private Configuration Config { get; set; }

        [SetUp]
        public void Setup()
        {
            Config = new Configuration();
            _ = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };

            nMController = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(services);
        }

        [Test]
        public async Task WhenCallIndexOnLoad_ThenReturnList()
        {
            IActionResult result = await nMController.Index();
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(10, Is.EqualTo(showWeeklyFiles.YearAndWeekList.Count));
            Assert.That(5, Is.EqualTo(showWeeklyFiles.ShowFilesResponseList.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That(2020, Is.EqualTo(showWeeklyFiles.YearAndWeekList[0].Year));
            Assert.That(14, Is.EqualTo(showWeeklyFiles.YearAndWeekList[0].Week));
            Assert.That("application/pdf", Is.EqualTo(showWeeklyFiles.ShowFilesResponseList[0].MimeType));
            Assert.That(Config.MaxAttributeValuesCount >= showWeeklyFiles.YearAndWeekList.Count);
        }

        [Test]
        public async Task WhenCallIndexWithYearWeek_ThenReturnList()
        {
            IActionResult result = await nMController.Index(2021, 30);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(4, Is.EqualTo(showWeeklyFiles.ShowFilesResponseList.Count));
            Assert.That(10, Is.EqualTo(showWeeklyFiles.YearAndWeekList.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That("msi_img_W2021_30.jpg", Is.EqualTo(showWeeklyFiles.ShowFilesResponseList[1].Filename));
            Assert.That(".jpg", Is.EqualTo(showWeeklyFiles.ShowFilesResponseList[1].FileExtension));
            Assert.That("msi_img_W2021_30", Is.EqualTo(showWeeklyFiles.ShowFilesResponseList[1].FileDescription));
            Assert.That(2021, Is.EqualTo(showWeeklyFiles.YearAndWeekList[2].Year));
            Assert.That(30, Is.EqualTo(showWeeklyFiles.YearAndWeekList[2].Week));
        }

        [Test]
        public async Task WhenCallIndexForWeekWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            IActionResult result = await nMController.Index(2021, 08);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(0, Is.EqualTo(showWeeklyFiles.ShowFilesResponseList.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncForPublicUser_ThenReturnWeeklyFiles()
        {
            IActionResult result = await nMController.ShowWeeklyFilesAsync(2020, 14);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(4, Is.EqualTo(listFiles.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That("a738d0d3-bc1e-47ca-892a-9514ccef6464", Is.EqualTo(listFiles[0].BatchId));
            Assert.That("21snii22_week_W2020_14", Is.EqualTo(listFiles[0].FileDescription));
            Assert.That(".pdf", Is.EqualTo(listFiles[0].FileExtension));
            Assert.That(1072212, Is.EqualTo(listFiles[0].FileSize));
            Assert.That(listFiles[0].IsDistributorUser, Is.False);
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            IActionResult result = await nMController.Index(2022, 06);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(0, Is.EqualTo(showWeeklyFiles.ShowFilesResponseList.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithDuplicateData_ThenReturnLatestWeeklyFiles()
        {
            IActionResult result = await nMController.ShowWeeklyFilesAsync(2022, 18);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.That(listFiles != null);
            Assert.That(3, Is.EqualTo(listFiles.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That("e6231e8f-2dfa-4c1d-8b68-9913f4d70e55", Is.EqualTo(listFiles[0].BatchId));
            Assert.That("NM_MSI", Is.EqualTo(listFiles[0].FileDescription));
            Assert.That("image/jpg", Is.EqualTo(listFiles[0].MimeType));
            Assert.That(108480, Is.EqualTo(listFiles[0].FileSize));
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsync_ThenReturnDailyFiles()
        {
            IActionResult result = await nMController.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((ViewResult)result).Model;
            Assert.That(showFiles, Is.Not.Null);
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That(1, Is.EqualTo(showFiles[4].DailyFilesData.Count));
            Assert.That("33", Is.EqualTo(showFiles[4].WeekNumber));
            Assert.That("2021", Is.EqualTo(showFiles[4].Year));
            Assert.That("2021/33", Is.EqualTo(showFiles[4].YearWeek));
            Assert.That("2021-08-14", Is.EqualTo(showFiles[4].DailyFilesData[0].DataDate));
            Assert.That("Daily 14-08-21.zip", Is.EqualTo(showFiles[4].DailyFilesData[0].Filename));
            Assert.That("416 KB", Is.EqualTo(showFiles[4].DailyFilesData[0].FileSizeInKB));
            Assert.That("977e771c-1ed6-4345-8d01-fff728952f1b", Is.EqualTo(showFiles[4].DailyFilesData[0].BatchId));
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsyncWithDuplicateData_ThenReturnDailyLatestFiles()
        {
            IActionResult result = await nMController.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((ViewResult)result).Model;
            Assert.That(showFiles != null);
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That(5, Is.EqualTo(showFiles[0].DailyFilesData.Count));
            Assert.That("21", Is.EqualTo(showFiles[0].WeekNumber));
            Assert.That("2022", Is.EqualTo(showFiles[0].Year));
            Assert.That("2022-05-24", Is.EqualTo(showFiles[0].DailyFilesData[0].DataDate));
            Assert.That("Daily 24-05-22.zip", Is.EqualTo(showFiles[0].DailyFilesData[0].Filename));
            Assert.That("416 KB", Is.EqualTo(showFiles[0].DailyFilesData[0].FileSizeInKB));
            Assert.That("a8d14b93-42ab-455b-a4ed-39effecb8536", Is.EqualTo(showFiles[0].DailyFilesData[0].BatchId));
        }

        //hb done
        [Test]
        public async Task WhenCallDownloadFile_ThenReturnFile()
        {
            const string batchId = "2bdec6dd-68be-4763-b805-d57a3a49d3b9";
            const string filename = "21snii22_week_W2020_14.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            FileResult result = await nMController.DownloadFile(batchId, filename, mimeType, frequency);
            Assert.That(result, Is.Not.Null);
            Assert.That("application/pdf", Is.EqualTo(result.ContentType));
            Assert.That("https://filesqa.admiralty.co.uk", Is.EqualTo(Config.BaseUrl));
            Assert.That(839, Is.EqualTo(((FileContentResult)result).FileContents.Length));
        }

        [Test]
        public void WhenCallDownloadFileWithInvalidData_ThenReturnException()
        {
            const string batchId = "a738d0d3-bc1e-47ca-892a-9514ccef6464";
            const string filename = "Test.txt";
            const string mimeType = "application/txt";
            const string frequency = "Weekly";

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>()
               .And.Message.EqualTo("Response status code does not indicate success: 404 (Not Found).")
               , async delegate { await nMController.DownloadFile(batchId, filename, mimeType, frequency); });
        }

        //hb done
        [Test]
        public async Task WhenCallDownloadDailyFile_ThenReturnFile()
        {
            const string batchId = "7ef50011-1820-4630-b540-7289526a7c89"; //hb done
            const string filename = "DNM_Text.pdf";
            const string mimeType = "application/pdf";

            ActionResult result = await nMController.DownloadDailyFile(batchId, filename, mimeType);
            Assert.That((FileContentResult)result != null);
            Assert.That("application/pdf", Is.EqualTo(((FileContentResult)result).ContentType));
            Assert.That(1180, Is.EqualTo(((FileContentResult)result).FileContents.Length));
            Assert.That("https://filesqa.admiralty.co.uk", Is.EqualTo(Config.BaseUrl));
        }

        //hb done
        [Test]
        public void WhenCallDownloadDailyFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "6ef2e377-d1e9-42ba-b37b-93231b2397bd"; // BatchId doesn't exist.
            const string filename = "Test.txt";
            const string mimeType = "application/txt";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>(),
                async delegate { await nMController.DownloadDailyFile(batchId, filename, mimeType); });
        }

        [Test]
        public async Task WhenCallCumulativeAsync_ThenReturnCumulativeFiles()
        {
            IActionResult result = await nMController.Cumulative();
            ShowNMFilesResponseModel showNMFiles = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(6, Is.EqualTo(showNMFiles.ShowFilesResponseModel.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That("50044762-231d-41ec-a908-ba9eb59c61ab", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].BatchId));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That(".pdf", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileExtension));
            Assert.That(1386825, Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileSize));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That("NP234(B) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[2].FileDescription));
            Assert.That("NP234(A) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[3].FileDescription));
        }

        [Test]
        public async Task WhenCallCumulativeAsyncForDuplicateData_ThenReturnLatestCumulativeFiles()
        {
            IActionResult result = await nMController.Cumulative();
            ShowNMFilesResponseModel showNMFiles = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(6, Is.EqualTo(showNMFiles.ShowFilesResponseModel.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That("f5569dc0-a0e4-40f5-b252-fef2e77861e1", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].BatchId));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That(".pdf", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileExtension));
            Assert.That(1265024, Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileSize));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That("NP234(B) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[2].FileDescription));
            Assert.That("NP234(A) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[3].FileDescription));
        }

        [Test]
        public async Task WhenCallAnnual_ThenReturnAnnualFiles()
        {
            IActionResult result = await nMController.Annual();
            ShowNMFilesResponseModel responseModel = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(responseModel.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(15, Is.EqualTo(responseModel.ShowFilesResponseModel.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That("10219d3c-15bb-43db-ab51-2f2f4f6038de", Is.EqualTo(responseModel.ShowFilesResponseModel[0].BatchId));
            Assert.That("An overview of the 26 sections", Is.EqualTo(responseModel.ShowFilesResponseModel[0].FileDescription));
            Assert.That(".pdf", Is.EqualTo(responseModel.ShowFilesResponseModel[0].FileExtension));
            Assert.That(205745, Is.EqualTo(responseModel.ShowFilesResponseModel[0].FileSize));
            Assert.That("ADMIRALTY Tide Tables 2022 — General Information", Is.EqualTo(responseModel.ShowFilesResponseModel[1].FileDescription));
            Assert.That("Suppliers of ADMIRALTY Charts and Publications", Is.EqualTo(responseModel.ShowFilesResponseModel[2].FileDescription));
            Assert.That("Safety of British merchant ships in periods of peace, tension or conflict", Is.EqualTo(responseModel.ShowFilesResponseModel[3].FileDescription));
            Assert.That("---", Is.EqualTo(responseModel.ShowFilesResponseModel[0].Hash));
            Assert.That("1", Is.EqualTo(responseModel.ShowFilesResponseModel[1].Hash));
        }

        [Test]
        public async Task WhenCallAnnualWithDuplicateData_ThenReturnUniqueAnnualFiles()
        {
            IActionResult result = await nMController.Annual();
            ShowNMFilesResponseModel responseModel = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(responseModel.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(15, Is.EqualTo(responseModel.ShowFilesResponseModel.Count));
            Assert.That("MaritimeSafetyInformationIntegrationTest", Is.EqualTo(Config.BusinessUnit));
            Assert.That("Notices to Mariners", Is.EqualTo(Config.ProductType));
            Assert.That("10219d3c-15bb-43db-ab51-2f2f4f6038de", Is.EqualTo(responseModel.ShowFilesResponseModel[0].BatchId));
            Assert.That("Firing Practice and Exercise Areas", Is.EqualTo(responseModel.ShowFilesResponseModel[4].FileDescription));
            Assert.That(".pdf", Is.EqualTo(responseModel.ShowFilesResponseModel[3].FileExtension));
            Assert.That(133291, Is.EqualTo(responseModel.ShowFilesResponseModel[1].FileSize));
            Assert.That("Mine-Laying and Mine Countermeasures Exercises - Waters around the British Isles", Is.EqualTo(responseModel.ShowFilesResponseModel[5].FileDescription));
            Assert.That("National Claims to Maritime Jurisdiction", Is.EqualTo(responseModel.ShowFilesResponseModel[6].FileDescription));
            Assert.That("19 Global Navigational Satellite System Positions, Horizontal Datums and Position Shifts.pdf", Is.EqualTo(responseModel.ShowFilesResponseModel[7].Filename));
            Assert.That("---", Is.EqualTo(responseModel.ShowFilesResponseModel[14].Hash));
            Assert.That("1", Is.EqualTo(responseModel.ShowFilesResponseModel[1].Hash));
        }

        [Test]
        public async Task WhenCallDownloadAllWeeklyZipFile_ThenReturnZipFile()
        {
            const string batchId = "3db9e8c4-0dea-43c8-8de3-e875be26c418";
            const string filename = "WeeklyAll_NM.zip";
            const string mimeType = "application/gzip";
            const string type = "public";

            ActionResult result = await nMController.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type);
            Assert.That((FileContentResult)result != null);
            Assert.That("application/gzip", Is.EqualTo(((FileContentResult)result).ContentType));
            Assert.That(2278920, Is.EqualTo(((FileContentResult)result).FileContents.Length));
            Assert.That("https://filesqa.admiralty.co.uk", Is.EqualTo(Config.BaseUrl));
        }

        [Test]
        public void WhenCallDownloadAllWeeklyZipFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "3db9e8c4-0dea-43c8-8de3-e875be261234";
            const string filename = "WeeklyAll_NM.zip";
            const string mimeType = "application/gzip";
            const string type = "public";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>(),
                async delegate { await nMController.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type); });
        }
    }
}
