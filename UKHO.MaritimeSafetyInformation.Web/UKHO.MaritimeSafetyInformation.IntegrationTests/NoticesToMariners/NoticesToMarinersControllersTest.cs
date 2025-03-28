﻿using System;
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
    /// https://dev.azure.com/ukhydro/Maritime%20Safety%20Information/_wiki/wikis/Maritime-Safety-Information.wiki/329/Notices-to-Mariners-Test-Data-Creation
    /// The following years and week numbers are used by the weekly tests:
    /// 2020	14
    /// 2021	30
    /// 2021	44
    /// 2022	 5
    /// 2022	11
    /// 2022	18
    /// The following years and week numbers are used by the daily tests:
    /// 2020	40
    /// 2021	33
    /// 2021	44
    /// 2022	18
    /// 2022	21
    /// </summary>
    [Category("FssApi")]
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
            // Ensure that we're looking for test data in the right place.
            Assert.That(configuration.BusinessUnit, Is.EqualTo("MaritimeSafetyInformationIntegrationTest"));
            Assert.That(configuration.ProductType, Is.EqualTo("Notices to Mariners"));
            Assert.That(configuration.BaseUrl, Is.EqualTo("https://filesqa.admiralty.co.uk"));
        }

        [SetUp]
        public void Setup()
        {
            _ = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
            controller = ActivatorUtilities.CreateInstance<NoticesToMarinersController>(services);
        }

        // Test data - see 'Upload Weekly Dup Notices to Mariners 03' for first ShowFilesResponseList as it has the most recent year and week number
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

        // Test data - see 'Upload Weekly Notices to Mariners 03' for year 2021 week 30
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

        // Test data - there's no weekly data for 2021 week 8
        [Test]
        public async Task WhenCallIndexForWeekWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            var result = await controller.Index(2021, 8) as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showWeeklyFiles = result.Model as ShowWeeklyFilesResponseModel;
            Assert.That(showWeeklyFiles, Is.Not.Null);
            Assert.That(showWeeklyFiles.ShowFilesResponseList.Count, Is.EqualTo(0));
        }

        // Test data - see 'Upload Weekly Notices to Mariners 04' for year 2021 week 30
        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncForPublicUser_ThenReturnWeeklyFiles()
        {
            var result = await controller.ShowWeeklyFilesAsync(2020, 14) as PartialViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(4));
            Assert.That(listFiles[0].BatchId, Is.EqualTo("2467d7a8-4d08-4378-98b4-2898e5970d44"));
            Assert.That(listFiles[0].FileDescription, Is.EqualTo("21snii22_week_W2020_14"));
            Assert.That(listFiles[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(listFiles[0].FileSize, Is.EqualTo(839));
            Assert.That(listFiles[0].IsDistributorUser, Is.False);
        }

        // Test data - there's no weekly data for 2022 week 6
        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithNoData_ThenShouldReturnEmptyShowFilesResponseList()
        {
            var result = await controller.ShowWeeklyFilesAsync(2022, 6) as PartialViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(0));
        }

        // Test data - see 'Upload Weekly Dup Notices to Mariners 03'
        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithDuplicateData_ThenReturnLatestWeeklyFiles()
        {
            var result = await controller.ShowWeeklyFilesAsync(2022, 18) as PartialViewResult;
            Assert.That(result, Is.Not.Null);
            var listFiles = result.Model as List<ShowFilesResponseModel>;
            Assert.That(listFiles, Is.Not.Null);
            Assert.That(listFiles.Count, Is.EqualTo(3));
            Assert.That(listFiles[0].BatchId, Is.EqualTo("ba66b732-e67e-4219-bbd4-76b703ba18c0"));
            Assert.That(listFiles[0].FileDescription, Is.EqualTo("NM_MSI"));
            Assert.That(listFiles[0].MimeType, Is.EqualTo("image/jpg"));
            Assert.That(listFiles[0].FileSize, Is.EqualTo(2925));
        }

        // Test data - see 'Upload Daily Notices to Mariners 02'
        [Test]
        public async Task WhenCallShowDailyFilesAsync_ThenReturnDailyFiles()
        {
            var result = await controller.ShowDailyFilesAsync() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showFiles = result.Model as List<ShowDailyFilesResponseModel>;
            Assert.That(showFiles, Is.Not.Null);
            Assert.That(showFiles.Count, Is.EqualTo(5));
            Assert.That(showFiles[4].DailyFilesData.Count, Is.EqualTo(1));
            Assert.That(showFiles[4].WeekNumber, Is.EqualTo("40"));
            Assert.That(showFiles[4].Year, Is.EqualTo("2020"));
            Assert.That(showFiles[4].YearWeek, Is.EqualTo("2020/40"));
            Assert.That(showFiles[4].DailyFilesData[0].DataDate, Is.EqualTo("2020-10-02"));
            Assert.That(showFiles[4].DailyFilesData[0].Filename, Is.EqualTo("Daily 02-10-20.zip"));
            Assert.That(showFiles[4].DailyFilesData[0].FileSizeInKB, Is.EqualTo("1 KB"));
            Assert.That(showFiles[4].DailyFilesData[0].BatchId, Is.EqualTo("25dff927-796b-4fa2-af90-4f63b4885127"));
        }

        // Test data - see 'Upload Daily Dup Notices to Mariners 01'
        [Test]
        public async Task WhenCallShowDailyFilesAsyncWithDuplicateData_ThenReturnDailyLatestFiles()
        {
            var result = await controller.ShowDailyFilesAsync() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showFiles = result.Model as List<ShowDailyFilesResponseModel>;
            Assert.That(showFiles, Is.Not.Null);
            Assert.That(showFiles.Count, Is.EqualTo(5));
            Assert.That(showFiles[0].DailyFilesData.Count, Is.EqualTo(5));
            Assert.That(showFiles[0].WeekNumber, Is.EqualTo("21"));
            Assert.That(showFiles[0].Year, Is.EqualTo("2022"));
            Assert.That(showFiles[0].DailyFilesData[0].DataDate, Is.EqualTo("2022-05-24"));
            Assert.That(showFiles[0].DailyFilesData[0].Filename, Is.EqualTo("Daily 24-05-22.zip"));
            Assert.That(showFiles[0].DailyFilesData[0].FileSizeInKB, Is.EqualTo("1 KB"));
            Assert.That(showFiles[0].DailyFilesData[0].BatchId, Is.EqualTo("7af41646-22b5-4d68-bc86-22029c6d6f2d"));
        }

        // Test data - see 'Upload Weekly Notices to Mariners 04'
        [Test]
        public async Task WhenCallDownloadFile_ThenReturnFile()
        {
            const string batchId = "2467d7a8-4d08-4378-98b4-2898e5970d44";
            const string fileName = "21snii22_week_W2020_14.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            var result = await controller.DownloadFile(batchId, fileName, mimeType, frequency) as FileContentResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo(mimeType));
            Assert.That(result.FileContents.Length, Is.EqualTo(839));
        }

        // Test data - use same BatchId as WhenCallDownloadFile_ThenReturnFile()
        [Test]
        public void WhenCallDownloadFileWithInvalidData_ThenReturnException()
        {
            const string batchId = "2467d7a8-4d08-4378-98b4-2898e5970d44";
            const string filename = "Test.txt";
            const string mimeType = "application/txt";
            const string frequency = "Weekly";

            Assert.ThrowsAsync(Is.TypeOf<HttpRequestException>()
                .And.Message.EqualTo("Response status code does not indicate success: 404 (Not Found)."),
                async delegate { await controller.DownloadFile(batchId, filename, mimeType, frequency); });
        }

        // Test data - see 'Upload Daily Notices to Mariners 01'
        [Test]
        public async Task WhenCallDownloadDailyFile_ThenReturnFile()
        {
            const string batchId = "679c4ec7-1c25-4856-9f62-5202d97a7416";
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

        // Test data - see 'Upload Cumulative Notices to Mariners 05' for first entry in ShowFilesResponseModel
        [Test]
        public async Task WhenCallCumulativeAsync_ThenReturnCumulativeFiles()
        {
            var result = await controller.Cumulative() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showNMFiles = result.Model as ShowNMFilesResponseModel;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(showNMFiles.ShowFilesResponseModel?.Count, Is.EqualTo(6));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].BatchId, Is.EqualTo("a0925557-5119-44df-a51e-c73f104e6f71"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("NP234(B) 2023"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileSize, Is.EqualTo(839));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("NP234(A) 2023"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileDescription, Is.EqualTo("NP234(B) 2022"));
            Assert.That(showNMFiles.ShowFilesResponseModel[3].FileDescription, Is.EqualTo("NP234(A) 2022"));
            Assert.That(showNMFiles.ShowFilesResponseModel[4].FileDescription, Is.EqualTo("NP234(B) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[5].FileDescription, Is.EqualTo("NP234(A) 2021"));
        }

        // Test data - see 'Upload Cumulative Dup Notices to Mariners 03' for latest duplicate entry
        [Test]
        public async Task WhenCallCumulativeAsyncForDuplicateData_ThenReturnLatestCumulativeFiles()
        {
            var result = await controller.Cumulative() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var showNMFiles = result.Model as ShowNMFilesResponseModel;
            Assert.That(showNMFiles, Is.Not.Null);
            Assert.That(showNMFiles.ShowFilesResponseModel?.Count, Is.EqualTo(6));
            Assert.That(showNMFiles.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("NP234(B) 2023"));
            Assert.That(showNMFiles.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("NP234(A) 2023"));
            Assert.That(showNMFiles.ShowFilesResponseModel[2].FileDescription, Is.EqualTo("NP234(B) 2022"));
            Assert.That(showNMFiles.ShowFilesResponseModel[3].BatchId, Is.EqualTo("d576060c-5577-424d-b297-b6c20ac0d461"));
            Assert.That(showNMFiles.ShowFilesResponseModel[3].FileDescription, Is.EqualTo("NP234(A) 2022"));
            Assert.That(showNMFiles.ShowFilesResponseModel[3].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(showNMFiles.ShowFilesResponseModel[3].FileSize, Is.EqualTo(839));
            Assert.That(showNMFiles.ShowFilesResponseModel[4].FileDescription, Is.EqualTo("NP234(B) 2021"));
            Assert.That(showNMFiles.ShowFilesResponseModel[5].FileDescription, Is.EqualTo("NP234(A) 2021"));
        }

        // Test data - see 'Upload Annual Notices to Mariners 01'
        [Test]
        public async Task WhenCallAnnual_ThenReturnAnnualFiles()
        {
            var result = await controller.Annual() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var responseModel = result.Model as ShowNMFilesResponseModel;
            Assert.That(responseModel, Is.Not.Null);
            Assert.That(responseModel.ShowFilesResponseModel?.Count, Is.EqualTo(4));
            Assert.That(responseModel.ShowFilesResponseModel[0].BatchId, Is.EqualTo("f6ad3e65-c8b0-42ff-b7cc-3c8f6437fa8b"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("An overview of the 26 sections"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileSize, Is.EqualTo(839));
            Assert.That(responseModel.ShowFilesResponseModel[0].Hash, Is.EqualTo("---"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("ADMIRALTY Tide Tables 2024 - General Information"));
            Assert.That(responseModel.ShowFilesResponseModel[1].Hash, Is.EqualTo("1"));
            Assert.That(responseModel.ShowFilesResponseModel[2].FileDescription, Is.EqualTo("Suppliers of ADMIRALTY Charts and Publications"));
            Assert.That(responseModel.ShowFilesResponseModel[2].Hash, Is.EqualTo("2"));
            Assert.That(responseModel.ShowFilesResponseModel[3].FileDescription, Is.EqualTo("Safety of British merchant ships in periods of peace, tension or conflict"));
            Assert.That(responseModel.ShowFilesResponseModel[3].Hash, Is.EqualTo("3"));
        }

        // Test data - see 'Upload Annual Dup Notices to Mariners 01' for latest duplicate entry
        [Test]
        public async Task WhenCallAnnualWithDuplicateData_ThenReturnUniqueAnnualFiles()
        {
            var result = await controller.Annual() as ViewResult;
            Assert.That(result, Is.Not.Null);
            var responseModel = result.Model as ShowNMFilesResponseModel;
            Assert.That(responseModel, Is.Not.Null);
            Assert.That(responseModel.ShowFilesResponseModel?.Count, Is.EqualTo(4));
            Assert.That(responseModel.ShowFilesResponseModel[0].FileDescription, Is.EqualTo("An overview of the 26 sections"));
            Assert.That(responseModel.ShowFilesResponseModel[0].Hash, Is.EqualTo("---"));
            Assert.That(responseModel.ShowFilesResponseModel[1].FileDescription, Is.EqualTo("ADMIRALTY Tide Tables 2024 - General Information"));
            Assert.That(responseModel.ShowFilesResponseModel[1].Hash, Is.EqualTo("1"));
            Assert.That(responseModel.ShowFilesResponseModel[2].FileDescription, Is.EqualTo("Suppliers of ADMIRALTY Charts and Publications"));
            Assert.That(responseModel.ShowFilesResponseModel[2].Hash, Is.EqualTo("2"));
            Assert.That(responseModel.ShowFilesResponseModel[3].BatchId, Is.EqualTo("7422bd06-e2eb-4bb8-b682-647900e0143a"));
            Assert.That(responseModel.ShowFilesResponseModel[3].FileDescription, Is.EqualTo("Safety of British merchant ships in periods of peace, tension or conflict"));
            Assert.That(responseModel.ShowFilesResponseModel[3].FileExtension, Is.EqualTo(".pdf"));
            Assert.That(responseModel.ShowFilesResponseModel[3].FileSize, Is.EqualTo(839));
            Assert.That(responseModel.ShowFilesResponseModel[3].Hash, Is.EqualTo("3"));
        }

        // Test Data - see 'Upload Weekly Notices to Mariners 02'
        [Test]
        public async Task WhenCallDownloadAllWeeklyZipFile_ThenReturnZipFile()
        {
            const string batchId = "05eb6282-67c1-4403-88f5-83219552b1e9";
            const string filename = "WeeklyAll_NM.zip";
            const string mimeType = "application/gzip";
            const string type = "public";

            var result = await controller.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type) as FileContentResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo("application/gzip"));
            Assert.That(result.FileContents.Length, Is.EqualTo(3499));
        }

        // Test data - none, but BatchId doesn't exist
        [Test]
        public void WhenCallDownloadAllWeeklyZipFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "70ea968b-26f9-461e-8d16-4b488eb454bc";
            const string filename = "WeeklyAll_NM.zip";
            const string mimeType = "application/gzip";
            const string type = "public";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>(),
                async delegate { await controller.DownloadAllWeeklyZipFile(batchId, filename, mimeType, type); });
        }
    }
}
