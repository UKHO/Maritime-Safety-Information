using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Configuration;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.IntegrationTests.MockServices;
using UKHO.MaritimeSafetyInformation.Web;
using UKHO.MaritimeSafetyInformation.Web.Controllers;

namespace UKHO.MaritimeSafetyInformation.IntegrationTests.NoticesToMariners
{
    /// <summary>
    /// These tests are similar to NoticesToMarinersControllersTest, but they use a mock of the File Share Service.
    /// </summary>
    [Category("FssMock")]
    internal class NoticesToMarinersControllersFssMockTest
    {
        private IServiceProvider services;
        private NoticesToMarinersController controller;
        private FssMock fss;
        private Configuration configuration;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            services = Program.CreateHostBuilder(Array.Empty<string>()).Build().Services;
            configuration = new Configuration();
            Assert.That(string.IsNullOrWhiteSpace(configuration.BusinessUnit), Is.False);
            Assert.That(string.IsNullOrWhiteSpace(configuration.ProductType), Is.False);
            fss = new FssMock(configuration);
            var fssConfig = services.GetService<IOptions<FileShareServiceConfiguration>>().Value;
            fssConfig.BaseUrl = fss.BaseUrl;
        }

        [SetUp]
        public void Setup()
        {
            _ = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
            controller = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(services);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            fss?.Stop();
        }

        [Test]
        public async Task WhenCallIndexOnLoad_ThenReturnList()
        {
            fss.SetupBatchAttributeSearchWeekly("WhenCallIndexOnLoad_ThenReturnList 1.json");
            fss.SetupSearchWeekly("WhenCallIndexOnLoad_ThenReturnList 2.json", 2024, 4);

            var result = await controller.Index() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showWeeklyFiles = result.Model as ShowWeeklyFilesResponseModel;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.YearAndWeekList.Count, Is.EqualTo(9));
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(3));

            Assert.That(showWeeklyFiles.YearAndWeekList[0].Year, Is.EqualTo(2024));
            Assert.That(showWeeklyFiles.YearAndWeekList[0].Week, Is.EqualTo(2));
            Assert.That(showWeeklyFiles.YearAndWeekList[1].Year, Is.EqualTo(2024));
            Assert.That(showWeeklyFiles.YearAndWeekList[1].Week, Is.EqualTo(3));
            Assert.That(showWeeklyFiles.YearAndWeekList[2].Year, Is.EqualTo(2024));
            Assert.That(showWeeklyFiles.YearAndWeekList[2].Week, Is.EqualTo(4));
            Assert.That(showWeeklyFiles.YearAndWeekList[3].Year, Is.EqualTo(2023));
            Assert.That(showWeeklyFiles.YearAndWeekList[3].Week, Is.EqualTo(5));
            Assert.That(showWeeklyFiles.YearAndWeekList[4].Year, Is.EqualTo(2023));
            Assert.That(showWeeklyFiles.YearAndWeekList[4].Week, Is.EqualTo(6));
            Assert.That(showWeeklyFiles.YearAndWeekList[5].Year, Is.EqualTo(2023));
            Assert.That(showWeeklyFiles.YearAndWeekList[5].Week, Is.EqualTo(7));
            Assert.That(showWeeklyFiles.YearAndWeekList[6].Year, Is.EqualTo(2022));
            Assert.That(showWeeklyFiles.YearAndWeekList[6].Week, Is.EqualTo(8));
            Assert.That(showWeeklyFiles.YearAndWeekList[7].Year, Is.EqualTo(2022));
            Assert.That(showWeeklyFiles.YearAndWeekList[7].Week, Is.EqualTo(9));
            Assert.That(showWeeklyFiles.YearAndWeekList[8].Year, Is.EqualTo(2022));
            Assert.That(showWeeklyFiles.YearAndWeekList[8].Week, Is.EqualTo(10));

            Assert.That(showWeeklyFiles.ShowFilesResponseList[0].MimeType, Is.EqualTo("application/pdf"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].MimeType, Is.EqualTo("text/plain"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[2].MimeType, Is.EqualTo("application/pdf"));

            Assert.That(configuration.MaxAttributeValuesCount >= showWeeklyFiles.YearAndWeekList.Count);
        }

        [Test]
        public async Task WhenCallIndexWithYearWeek_ThenReturnList()
        {
            const int year = 2021;
            const int weekNumber = 30;
            fss.SetupBatchAttributeSearchWeekly("WhenCallIndexWithYearWeek_ThenReturnList 1.json");
            fss.SetupSearchWeekly("WhenCallIndexWithYearWeek_ThenReturnList 2.json", year, weekNumber);

            var result = await controller.Index(year, weekNumber) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showWeeklyFiles = result.Model as ShowWeeklyFilesResponseModel;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(2));
            Assert.That(showWeeklyFiles.YearAndWeekList.Count, Is.EqualTo(10));

            Assert.That(showWeeklyFiles.ShowFilesResponseList[0].Filename, Is.EqualTo("file1.pdf"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[0].FileDescription, Is.EqualTo("file1"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[0].MimeType, Is.EqualTo("application/pdf"));

            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].Filename, Is.EqualTo("msi_img_W2021_30.jpg"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].FileExtension, Is.EqualTo(".jpg"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].FileDescription, Is.EqualTo("msi_img_W2021_30"));
            Assert.That(showWeeklyFiles.ShowFilesResponseList[1].MimeType, Is.EqualTo("image/jpeg"));

            Assert.That(showWeeklyFiles.YearAndWeekList[9].Year, Is.EqualTo(year));
            Assert.That(showWeeklyFiles.YearAndWeekList[9].Week, Is.EqualTo(weekNumber));
        }

        [Test]
        public async Task WhenCallIndexForWeekWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            const int year = 2021;
            const int weekNumber = 8;
            fss.SetupBatchAttributeSearchWeekly("WhenCallIndexForWeekWithNoData_ThenShouldReturnEmptyShowFilesResponseList 1.json");
            fss.SetupSearchWeekly("WhenCallIndexForWeekWithNoData_ThenShouldReturnEmptyShowFilesResponseList 2.json", year, weekNumber);

            var result = await controller.Index(year, weekNumber) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showWeeklyFiles = result.Model as ShowWeeklyFilesResponseModel;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncForPublicUser_ThenReturnWeeklyFiles()
        {
            const int year = 2020;
            const int weekNumber = 14;
            fss.SetupSearchWeekly("WhenCallShowWeeklyFilesAsyncForPublicUser_ThenReturnWeeklyFiles.json", year, weekNumber);

            var result = await controller.ShowWeeklyFilesAsync(year, weekNumber) as PartialViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(4));
            Assert.That(listFiles[0].BatchId, Is.EqualTo("a738d0d3-bc1e-47ca-892a-9514ccef6464"));
            Assert.That(listFiles[0].FileDescription, Is.EqualTo("21snii22_week_W2020_14"));
            Assert.That(listFiles[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(listFiles[0].FileSize, Is.EqualTo(357367356));
            Assert.That(listFiles[0].IsDistributorUser, Is.False);
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncForDistributerUser_ThenReturnWeeklyFiles()
        {
            const int year = 2020;
            const int weekNumber = 15;
            fss.SetupSearchWeekly("WhenCallShowWeeklyFilesAsyncForDistributerUser_ThenReturnWeeklyFiles.json", year, weekNumber);

            var result = await controller.ShowWeeklyFilesAsync(year, weekNumber) as PartialViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(1));
            Assert.That(listFiles[0].BatchId, Is.EqualTo("9b4f181b-6860-437e-9914-05632021339c"));
            Assert.That(listFiles[0].FileDescription, Is.EqualTo("21snii22_week_W2020_14"));
            Assert.That(listFiles[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(listFiles[0].FileSize, Is.EqualTo(357367356));
            Assert.That(listFiles[0].IsDistributorUser, Is.True);
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            const int year = 2022;
            const int weekNumber = 6;
            fss.SetupSearchWeekly(null, year, weekNumber);

            var result = await controller.ShowWeeklyFilesAsync(year, weekNumber) as PartialViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithDuplicateData_ThenReturnLatestWeeklyFiles()
        {
            const int year = 2022;
            const int weekNumber = 18;
            fss.SetupSearchWeekly("WhenCallShowWeeklyFilesAsyncWithDuplicateData_ThenReturnLatestWeeklyFiles.json", year, weekNumber);

            var result = await controller.ShowWeeklyFilesAsync(year, weekNumber) as PartialViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(3));

            for (var i = 0; i < listFiles.Count; i++)
            {
                Assert.That(listFiles[i].BatchId, Is.EqualTo("18f384bf-78cb-4c3b-863c-38f3ed9d2f86"));
                Assert.That(listFiles[i].FileDescription, Is.EqualTo($"file{i + 1}"));
                Assert.That(listFiles[i].MimeType, Is.EqualTo("text/plain"));
            }

            Assert.That(32452345, Is.EqualTo(listFiles[0].FileSize));
            Assert.That(456232, Is.EqualTo(listFiles[1].FileSize));
            Assert.That(98343, Is.EqualTo(listFiles[2].FileSize));
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsync_ThenReturnDailyFiles()
        {
            fss.SetupSearchDaily("WhenCallShowDailyFilesAsync_ThenReturnDailyFiles.json");

            var result = await controller.ShowDailyFilesAsync() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowDailyFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(2));

            Assert.That(listFiles[0].DailyFilesData.Count, Is.EqualTo(1));
            Assert.That(listFiles[0].WeekNumber, Is.EqualTo("34"));
            Assert.That(listFiles[0].Year, Is.EqualTo("2021"));
            Assert.That(listFiles[0].YearWeek, Is.EqualTo("2021/34"));
            Assert.That(listFiles[0].DailyFilesData[0].DataDate, Is.EqualTo("2021-08-15"));
            Assert.That(listFiles[0].DailyFilesData[0].Filename, Is.EqualTo("Daily 15-08-21.zip"));
            Assert.That(listFiles[0].DailyFilesData[0].FileSizeInKB, Is.EqualTo("418 KB"));
            Assert.That(listFiles[0].DailyFilesData[0].BatchId, Is.EqualTo("c595789d-443e-4b49-8b5e-0848ebc7c523"));

            Assert.That(listFiles[1].DailyFilesData.Count, Is.EqualTo(1));
            Assert.That(listFiles[1].WeekNumber, Is.EqualTo("33"));
            Assert.That(listFiles[1].Year, Is.EqualTo("2021"));
            Assert.That(listFiles[1].YearWeek, Is.EqualTo("2021/33"));
            Assert.That(listFiles[1].DailyFilesData[0].DataDate, Is.EqualTo("2021-08-14"));
            Assert.That(listFiles[1].DailyFilesData[0].Filename, Is.EqualTo("Daily 14-08-21.zip"));
            Assert.That(listFiles[1].DailyFilesData[0].FileSizeInKB, Is.EqualTo("416 KB"));
            Assert.That(listFiles[1].DailyFilesData[0].BatchId, Is.EqualTo("ecc0915d-640f-4fb3-8204-afc175e71174"));
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsyncWithDuplicateData_ThenReturnDailyLatestFiles()
        {
            fss.SetupSearchDaily("WhenCallShowDailyFilesAsyncWithDuplicateData_ThenReturnDailyLatestFiles.json");

            var result = await controller.ShowDailyFilesAsync() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowDailyFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(1));

            Assert.That(listFiles[0].DailyFilesData.Count, Is.EqualTo(2));

            Assert.That(listFiles[0].WeekNumber, Is.EqualTo("20"));
            Assert.That(listFiles[0].Year, Is.EqualTo("2022"));

            Assert.That(listFiles[0].DailyFilesData[0].DataDate, Is.EqualTo("2022-05-23"));
            Assert.That(listFiles[0].DailyFilesData[0].Filename, Is.EqualTo("Daily 23-05-22.zip"));
            Assert.That(listFiles[0].DailyFilesData[0].FileSizeInKB, Is.EqualTo("416 KB"));
            Assert.That(listFiles[0].DailyFilesData[0].BatchId, Is.EqualTo("00434ef5-97cc-460e-a0de-5e748372cce6"));

            Assert.That(listFiles[0].DailyFilesData[1].DataDate, Is.EqualTo("2022-05-24"));
            Assert.That(listFiles[0].DailyFilesData[1].Filename, Is.EqualTo("Daily 24-05-22.zip"));
            Assert.That(listFiles[0].DailyFilesData[1].FileSizeInKB, Is.EqualTo("507 KB"));
            Assert.That(listFiles[0].DailyFilesData[1].BatchId, Is.EqualTo("54c55160-bc13-4e2a-82f1-19294d05106e"));
        }

        [Test]
        public async Task WhenCallDownloadFile_ThenReturnFile()
        {
            const string batchId = "a36f6b68-b990-4e19-8182-6d116e9ca895";
            const string filename = "21snii22_week_W2020_14.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";
            fss.SetupDownloadFile("WhenCallDownloadFile_ThenReturnFile.pdf", batchId, filename);

            var result = await controller.DownloadFile(batchId, filename, mimeType, frequency);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo(mimeType));
            var fileContentResult = result as FileContentResult;
            Assert.That(fileContentResult, Is.Not.Null);
            Assert.That(fileContentResult.FileContents.Length, Is.EqualTo(27146));
        }

        [Test]
        public void WhenCallDownloadFileWithInvalidData_ThenReturnException()
        {
            const string batchId = "bb517feb-ff7b-4e19-826a-897a3e2f6efe";
            const string filename = "Test.txt";
            const string mimeType = "application/txt";
            const string frequency = "Weekly";
            fss.SetupDownloadFile(null, batchId, filename, 404);

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>()
                .And.Message.EqualTo("Response status code does not indicate success: 404 (Not Found)."),
                async delegate { await controller.DownloadFile(batchId, filename, mimeType, frequency); });
        }

        [Test]
        public async Task WhenCallDownloadDailyFile_ThenReturnFile()
        {
            const string batchId = "63d4ffa1-4353-40c3-9d23-b6786c198033";
            const string filename = "DNM_Text.zip";
            const string mimeType = "application/x-zip";
            fss.SetupDownloadZipFile("WhenCallDownloadDailyFile_ThenReturnFile.zip", batchId);

            var result = await controller.DownloadDailyFile(batchId, filename, mimeType);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo(mimeType));
            var fileContentResult = result as FileContentResult;
            Assert.That(fileContentResult, Is.Not.Null);
            Assert.That(fileContentResult.FileContents.Length, Is.EqualTo(370));
        }

        [Test]
        public void WhenCallDownloadDailyFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "b31000c3-c95f-4aec-ac2a-18a48f1a7f3b";
            const string filename = "Test.txt";
            const string mimeType = "application/txt";
            fss.SetupDownloadZipFile("WhenCallDownloadDailyFileWithInvalidData_ThenThrowArgumentException.txt", batchId, 400);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>()
                .And.Message.EqualTo($"Error text{Environment.NewLine}"),
                async delegate { await controller.DownloadDailyFile(batchId, filename, mimeType); });
        }

        [Test]
        public async Task WhenCallCumulativeAsync_ThenReturnCumulativeFiles()
        {
            fss.SetupSearchCumulative("WhenCallCumulativeAsync_ThenReturnCumulativeFiles.json");

            var result = await controller.Cumulative() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showNMFiles = result.Model as ShowNMFilesResponseModel;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(showNMFiles.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(3, Is.EqualTo(showNMFiles.ShowFilesResponseModel.Count));

            Assert.That(showNMFiles.ShowFilesResponseModel[0].BatchId, Is.EqualTo("bf6bad11-c926-4bec-83f1-c3a5d70a6df0"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("NP234(A) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileSize, Is.EqualTo(938465));

            Assert.That(showNMFiles.ShowFilesResponseModel[1].BatchId, Is.EqualTo("1a4511c4-d3ad-4d86-a8ae-57bca3ec3588"));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("NP234(B) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileSize, Is.EqualTo(94732));

            Assert.That(showNMFiles.ShowFilesResponseModel[2].BatchId, Is.EqualTo("b6736bbd-9931-441d-9181-eab20e755c94"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileDescription, Is.EqualTo("NP234(C) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileSize, Is.EqualTo(1386825));
        }

        [Test]
        public async Task WhenCallCumulativeAsyncForDuplicateData_ThenReturnLatestCumulativeFiles()
        {
            fss.SetupSearchCumulative("WhenCallCumulativeAsyncForDuplicateData_ThenReturnLatestCumulativeFiles.json");

            var result = await controller.Cumulative() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showNMFiles = result.Model as ShowNMFilesResponseModel;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(showNMFiles.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(3, Is.EqualTo(showNMFiles.ShowFilesResponseModel.Count));

            Assert.That(showNMFiles.ShowFilesResponseModel[0].BatchId, Is.EqualTo("93537c88-ca23-4931-b871-843ac202a2c4"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("NP234(A) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileSize, Is.EqualTo(938465));

            Assert.That(showNMFiles.ShowFilesResponseModel[1].BatchId, Is.EqualTo("65a917aa-75fc-48f1-b5b2-495dbab4577a"));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("NP234(B) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileSize, Is.EqualTo(94732));

            Assert.That(showNMFiles.ShowFilesResponseModel[2].BatchId, Is.EqualTo("5b8866be-9baa-4f44-9aaf-ffd1e6e4583d"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileDescription, Is.EqualTo("NP234(C) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileSize, Is.EqualTo(1386825));
        }

        [Test]
        public async Task WhenCallAnnual_ThenReturnAnnualFiles()
        {
            fss.SetupSearchAnnual("WhenCallAnnual_ThenReturnAnnualFiles.json");

            var result = await controller.Annual() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var responseModel = result.Model as ShowNMFilesResponseModel;
            Assert.That(responseModel, Is.Not.Null);
            Assert.That(responseModel.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(2, Is.EqualTo(responseModel.ShowFilesResponseModel.Count));

            Assert.That(responseModel.ShowFilesResponseModel[0].BatchId, Is.EqualTo("0a3593d5-bcb3-4f41-ba4d-a91d683d1e3b"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("An overview of the 26 sections"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileSize, Is.EqualTo(205745));
            Assert.That(responseModel.ShowFilesResponseModel[0].Hash, Is.EqualTo("123"));

            Assert.That(responseModel.ShowFilesResponseModel[1].BatchId, Is.EqualTo("cfea85a9-746c-410c-8f7b-9584b69b5da7"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("An overview of the 27 sections"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileSize, Is.EqualTo(835465));
            Assert.That(responseModel.ShowFilesResponseModel[1].Hash, Is.EqualTo("130"));
        }

        [Test]
        public async Task WhenCallAnnualWithDuplicateData_ThenReturnUniqueAnnualFiles()
        {
            fss.SetupSearchAnnual("WhenCallAnnualWithDuplicateData_ThenReturnUniqueAnnualFiles.json");

            var result = await controller.Annual() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var responseModel = result.Model as ShowNMFilesResponseModel;
            Assert.That(responseModel, Is.Not.Null);
            Assert.That(responseModel.ShowFilesResponseModel, Is.Not.Null);
            Assert.That(2, Is.EqualTo(responseModel.ShowFilesResponseModel.Count));

            Assert.That(responseModel.ShowFilesResponseModel[0].BatchId, Is.EqualTo("3391da0a-707e-4daa-a84e-1e548e1ad633"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("An overview of the 26 sections"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileSize, Is.EqualTo(205745));
            Assert.That(responseModel.ShowFilesResponseModel[0].Hash, Is.EqualTo("123"));

            Assert.That(responseModel.ShowFilesResponseModel[1].BatchId, Is.EqualTo("f4d7a5a3-c6a1-4cdf-9863-bac35d1f5de0"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("An overview of the 27 sections"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileSize, Is.EqualTo(835465));
            Assert.That(responseModel.ShowFilesResponseModel[1].Hash, Is.EqualTo("130"));
        }

        [Test]
        public async Task WhenCallDownloadAllWeeklyZipFile_ThenReturnZipFile()
        {
            const string batchId = "8d746e63-2217-4280-bbd0-5a135fc0b02e";
            const string filename = "WeeklyAll_NM.zip";
            const string mimeType = "application/gzip";
            const string type = "public";
            fss.SetupDownloadZipFile("WhenCallDownloadAllWeeklyZipFile_ThenReturnZipFile.zip", batchId);

            var result = await controller.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo(mimeType));
            var fileContentResult = result as FileContentResult;
            Assert.That(fileContentResult, Is.Not.Null);
            Assert.That(fileContentResult.FileContents.Length, Is.EqualTo(216));
        }

        [Test]
        public void WhenCallDownloadAllWeeklyZipFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "4bb16352-647f-4c83-a750-ca2947aa70dd";
            const string filename = "WeeklyAll_NM.zip";
            const string mimeType = "application/gzip";
            const string type = "public";
            fss.SetupDownloadZipFile("WhenCallDownloadAllWeeklyZipFileWithInvalidData_ThenThrowArgumentException.txt", batchId, 400);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>()
                .And.Message.EqualTo($"Error text{Environment.NewLine}"),
                async delegate { await controller.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type); });
        }
    }
}
