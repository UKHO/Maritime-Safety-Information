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
    /// These tests require data to be set up in the File Share Service. Instructions can be found on the MSI project Wiki:
    /// https://dev.azure.com/ukhydro/Maritime%20Safety%20Information/_wiki/wikis/Maritime-Safety-Information.wiki/329/MSI-Notices-to-Mariners-Integration-Tests
    /// </summary>
    internal class NoticesToMarinersControllersTest
    {
        private IServiceProvider services;
        private NoticesToMarinersController controller;
        private Configuration configuration;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            services = Program.CreateHostBuilder(Array.Empty<string>()).Build().Services;
            configuration = new Configuration();
            Assert.That(configuration.BusinessUnit, Is.EqualTo("MaritimeSafetyInformationIntegrationTest"));
            Assert.That(configuration.ProductType, Is.EqualTo("Notices to Mariners"));
            Assert.That(configuration.BaseUrl, Is.EqualTo("https://filesqa.admiralty.co.uk"));
        }

        //weekly
        //2020	14
        //2021	30
        //2021	44
        //2022	5
        //2022	11
        //2022	18


        [SetUp]
        public void Setup()
        {
            _ = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
            controller = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(services);
        }

        // Test data - see 'Upload weekly dup Notices to Mariners 03' for first ShowFilesResponseList as it has the most recent year and week number
        [Test]
        public async Task WhenCallIndexOnLoad_ThenReturnList()
        {
            var result = await controller.Index() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showWeeklyFiles = result.Model as ShowWeeklyFilesResponseModel;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.YearAndWeekList.Count, Is.EqualTo(6));
            Assert.That(showWeeklyFiles.YearAndWeekList[0].Year, Is.EqualTo(2020));
            Assert.That(showWeeklyFiles.YearAndWeekList[0].Week, Is.EqualTo(14));
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(3));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[0].MimeType, Is.EqualTo("image/jpg"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].MimeType, Is.EqualTo("application/text"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[2].MimeType, Is.EqualTo("application/text"));
            Assert.That(showWeeklyFiles.YearAndWeekList.Count, Is.LessThanOrEqualTo(configuration.MaxAttributeValuesCount));
        }

        // Test data - see 'Upload weekly Notices to Mariners 03' for year 2021 week 30
        [Test]
        public async Task WhenCallIndexWithYearWeek_ThenReturnList()
        {
            var result = await controller.Index(2021, 30) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showWeeklyFiles = result.Model as ShowWeeklyFilesResponseModel;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(4));
            Assert.That(showWeeklyFiles.YearAndWeekList.Count, Is.EqualTo(6));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].Filename, Is.EqualTo("msi_img_W2021_30.jpg"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].FileExtension, Is.EqualTo(".jpg"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].FileDescription, Is.EqualTo("msi_img_W2021_30"));
            Assert.That(showWeeklyFiles.YearAndWeekList[1].Year, Is.EqualTo(2021));
            Assert.That(showWeeklyFiles.YearAndWeekList[1].Week, Is.EqualTo(30));
        }

        [Test]
        public async Task WhenCallIndexForWeekWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            IActionResult result = await controller.Index(2021, 08);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncForPublicUser_ThenReturnWeeklyFiles()
        {
            IActionResult result = await controller.ShowWeeklyFilesAsync(2020, 14);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(4));
            Assert.That("a738d0d3-bc1e-47ca-892a-9514ccef6464", Is.EqualTo(listFiles[0].BatchId));
            Assert.That("21snii22_week_W2020_14", Is.EqualTo(listFiles[0].FileDescription));
            Assert.That(".pdf", Is.EqualTo(listFiles[0].FileExtension));
            Assert.That(listFiles[0].FileSize, Is.EqualTo(1072212));
            Assert.That(listFiles[0].IsDistributorUser, Is.False);
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            IActionResult result = await controller.Index(2022, 06);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithDuplicateData_ThenReturnLatestWeeklyFiles()
        {
            IActionResult result = await controller.ShowWeeklyFilesAsync(2022, 18);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.That(listFiles != null);
            Assert.That(listFiles.Count, Is.EqualTo(3));
            Assert.That("e6231e8f-2dfa-4c1d-8b68-9913f4d70e55", Is.EqualTo(listFiles[0].BatchId));
            Assert.That("NM_MSI", Is.EqualTo(listFiles[0].FileDescription));
            Assert.That("image/jpg", Is.EqualTo(listFiles[0].MimeType));
            Assert.That(listFiles[0].FileSize, Is.EqualTo(108480));
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsync_ThenReturnDailyFiles()
        {
            IActionResult result = await controller.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((ViewResult)result).Model;
            Assert.That(showFiles, Is.Not.Null);
            Assert.That(showFiles[4].DailyFilesData.Count, Is.EqualTo(1));
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
            IActionResult result = await controller.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((ViewResult)result).Model;
            Assert.That(showFiles != null);
            Assert.That(showFiles[0].DailyFilesData.Count, Is.EqualTo(5));
            Assert.That("21", Is.EqualTo(showFiles[0].WeekNumber));
            Assert.That("2022", Is.EqualTo(showFiles[0].Year));
            Assert.That("2022-05-24", Is.EqualTo(showFiles[0].DailyFilesData[0].DataDate));
            Assert.That("Daily 24-05-22.zip", Is.EqualTo(showFiles[0].DailyFilesData[0].Filename));
            Assert.That("416 KB", Is.EqualTo(showFiles[0].DailyFilesData[0].FileSizeInKB));
            Assert.That("a8d14b93-42ab-455b-a4ed-39effecb8536", Is.EqualTo(showFiles[0].DailyFilesData[0].BatchId));
        }

        // Test data - see 'Upload weekly Notices to Mariners 04'
        [Test]
        public async Task WhenCallDownloadFile_ThenReturnFile()
        {
            const string batchId = "2bdec6dd-68be-4763-b805-d57a3a49d3b9";
            const string fileName = "21snii22_week_W2020_14.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            var result = await controller.DownloadFile(batchId, fileName, mimeType, frequency) as FileContentResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo(mimeType));
            Assert.That(result.FileContents.Length, Is.EqualTo(839));
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
               , async delegate { await controller.DownloadFile(batchId, filename, mimeType, frequency); });
        }

        // Test data - see 'Upload Daily Notices to Mariners 01'
        [Test]
        public async Task WhenCallDownloadDailyFile_ThenReturnFile()
        {
            const string batchId = "7ef50011-1820-4630-b540-7289526a7c89";
            const string fileName = "Daily 06-05-22.zip";
            const string mimeType = "application/x-zip";

            var result = await controller.DownloadDailyFile(batchId, fileName, mimeType) as FileContentResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo(mimeType));
            Assert.That(result.FileContents.Length, Is.EqualTo(1180));
        }

        // Test data - none, but BatchId doesn't exist
        [Test]
        public void WhenCallDownloadDailyFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "6ef2e377-d1e9-42ba-b37b-93231b2397bd";
            const string fileName = "Test.txt";
            const string mimeType = "application/txt";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>(),
                async delegate { await controller.DownloadDailyFile(batchId, fileName, mimeType); });
        }

        [Test]
        public async Task WhenCallCumulativeAsync_ThenReturnCumulativeFiles()
        {
            IActionResult result = await controller.Cumulative();
            ShowNMFilesResponseModel showNMFiles = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(showNMFiles.ShowFilesResponseModel.Count, Is.EqualTo(6));
            Assert.That("50044762-231d-41ec-a908-ba9eb59c61ab", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].BatchId));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That(".pdf", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileExtension));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileSize, Is.EqualTo(1386825));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That("NP234(B) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[2].FileDescription));
            Assert.That("NP234(A) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[3].FileDescription));
        }

        [Test]
        public async Task WhenCallCumulativeAsyncForDuplicateData_ThenReturnLatestCumulativeFiles()
        {
            IActionResult result = await controller.Cumulative();
            ShowNMFilesResponseModel showNMFiles = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(showNMFiles.ShowFilesResponseModel.Count, Is.EqualTo(6));
            Assert.That("f5569dc0-a0e4-40f5-b252-fef2e77861e1", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].BatchId));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That(".pdf", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileExtension));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileSize, Is.EqualTo(1265024));
            Assert.That("NP234(B) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[0].FileDescription));
            Assert.That("NP234(A) 2021", Is.EqualTo(showNMFiles.ShowFilesResponseModel[1].FileDescription));
            Assert.That("NP234(B) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[2].FileDescription));
            Assert.That("NP234(A) 2020", Is.EqualTo(showNMFiles.ShowFilesResponseModel[3].FileDescription));
        }

        [Test]
        public async Task WhenCallAnnual_ThenReturnAnnualFiles()
        {
            IActionResult result = await controller.Annual();
            ShowNMFilesResponseModel responseModel = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(responseModel.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(responseModel.ShowFilesResponseModel.Count, Is.EqualTo(15));
            Assert.That("10219d3c-15bb-43db-ab51-2f2f4f6038de", Is.EqualTo(responseModel.ShowFilesResponseModel[0].BatchId));
            Assert.That("An overview of the 26 sections", Is.EqualTo(responseModel.ShowFilesResponseModel[0].FileDescription));
            Assert.That(".pdf", Is.EqualTo(responseModel.ShowFilesResponseModel[0].FileExtension));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileSize, Is.EqualTo(205745));
            Assert.That("ADMIRALTY Tide Tables 2022 — General Information", Is.EqualTo(responseModel.ShowFilesResponseModel[1].FileDescription));
            Assert.That("Suppliers of ADMIRALTY Charts and Publications", Is.EqualTo(responseModel.ShowFilesResponseModel[2].FileDescription));
            Assert.That("Safety of British merchant ships in periods of peace, tension or conflict", Is.EqualTo(responseModel.ShowFilesResponseModel[3].FileDescription));
            Assert.That("---", Is.EqualTo(responseModel.ShowFilesResponseModel[0].Hash));
            Assert.That("1", Is.EqualTo(responseModel.ShowFilesResponseModel[1].Hash));
        }

        [Test]
        public async Task WhenCallAnnualWithDuplicateData_ThenReturnUniqueAnnualFiles()
        {
            IActionResult result = await controller.Annual();
            ShowNMFilesResponseModel responseModel = (ShowNMFilesResponseModel)((ViewResult)result).Model;
            Assert.That(responseModel.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(responseModel.ShowFilesResponseModel.Count, Is.EqualTo(15));
            Assert.That("10219d3c-15bb-43db-ab51-2f2f4f6038de", Is.EqualTo(responseModel.ShowFilesResponseModel[0].BatchId));
            Assert.That("Firing Practice and Exercise Areas", Is.EqualTo(responseModel.ShowFilesResponseModel[4].FileDescription));
            Assert.That(".pdf", Is.EqualTo(responseModel.ShowFilesResponseModel[3].FileExtension));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileSize, Is.EqualTo(133291));
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

            ActionResult result = await controller.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type);
            Assert.That((FileContentResult)result != null);
            Assert.That("application/gzip", Is.EqualTo(((FileContentResult)result).ContentType));
            Assert.That(((FileContentResult)result).FileContents.Length, Is.EqualTo(2278920));
            Assert.That("https://filesqa.admiralty.co.uk", Is.EqualTo(configuration.BaseUrl));
        }

        [Test]
        public void WhenCallDownloadAllWeeklyZipFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "3db9e8c4-0dea-43c8-8de3-e875be261234";
            const string filename = "WeeklyAll_NM.zip";
            const string mimeType = "application/gzip";
            const string type = "public";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>(),
                async delegate { await controller.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type); });
        }
    }
}
