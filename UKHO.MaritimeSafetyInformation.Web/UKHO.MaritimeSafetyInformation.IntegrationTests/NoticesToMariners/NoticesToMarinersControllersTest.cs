using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            Assert.AreEqual(8, showWeeklyFiles.YearAndWeekList.Count);
            Assert.AreEqual(5, showWeeklyFiles.ShowFilesResponseList.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(2020, showWeeklyFiles.YearAndWeekList[0].Year);
            Assert.AreEqual(14, showWeeklyFiles.YearAndWeekList[0].Week);
            Assert.AreEqual("application/pdf", showWeeklyFiles.ShowFilesResponseList[0].MimeType);
        }

        [Test]
        public async Task WhenCallIndexWithYearWeek_ThenReturnList()
        {
            IActionResult result = await _nMController.Index(2021, 30);
            ShowWeeklyFilesResponseModel showWeeklyFiles = (ShowWeeklyFilesResponseModel)((ViewResult)result).Model;
            Assert.IsNotNull(showWeeklyFiles);
            Assert.AreEqual(4, showWeeklyFiles.ShowFilesResponseList.Count);
            Assert.AreEqual(8, showWeeklyFiles.YearAndWeekList.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual("msi_img_W2021_30.jpg", showWeeklyFiles.ShowFilesResponseList[1].Filename);
            Assert.AreEqual(".jpg", showWeeklyFiles.ShowFilesResponseList[1].FileExtension);
            Assert.AreEqual("msi_img_W2021_30", showWeeklyFiles.ShowFilesResponseList[1].FileDescription);
            Assert.AreEqual(2021, showWeeklyFiles.YearAndWeekList[1].Year);
            Assert.AreEqual(30, showWeeklyFiles.YearAndWeekList[1].Week);
        }

        [Test]
        public void WhenCallIndexForWeekWithNoData_ThenThrowInvalidDataException()
        {
            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for weekly NM files"),
                async delegate { await _nMController.Index(2021, 08); });
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncForPublicUser_ThenReturnWeeklyFiles()
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
            Assert.IsFalse(listFiles[0].IsDistributorUser);
        }

        [Test]
        public void WhenCallShowWeeklyFilesAsyncWithNoData_ThenThrowInvalidDataException()
        {
            Assert.ThrowsAsync(Is.TypeOf<InvalidDataException>().And.Message.EqualTo("Invalid data received for weekly NM files"),
                async delegate { await _nMController.ShowWeeklyFilesAsync(2022, 6); });
        }

        [Test]
        public async Task WhenCallShowWeeklyFilesAsyncWithDuplicateData_ThenReturnLatestWeeklyFiles()
        {
            IActionResult result = await _nMController.ShowWeeklyFilesAsync(2022, 18);
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((PartialViewResult)result).Model;
            Assert.IsTrue(listFiles != null);
            Assert.AreEqual(3, listFiles.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual("e6231e8f-2dfa-4c1d-8b68-9913f4d70e55", listFiles[0].BatchId);
            Assert.AreEqual("NM_MSI", listFiles[0].FileDescription);
            Assert.AreEqual("image/jpg", listFiles[0].MimeType);
            Assert.AreEqual(108480, listFiles[0].FileSize);
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsync_ThenReturnDailyFiles()
        {
            IActionResult result = await _nMController.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((ViewResult)result).Model;
            Assert.IsNotNull(showFiles);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(1, showFiles[3].DailyFilesData.Count);
            Assert.AreEqual("33", showFiles[3].WeekNumber);
            Assert.AreEqual("2021", showFiles[3].Year);
            Assert.AreEqual("2021/33", showFiles[3].YearWeek);
            Assert.AreEqual("2021-08-14", showFiles[3].DailyFilesData[0].DataDate);
            Assert.AreEqual("Daily 14-08-21.zip", showFiles[3].DailyFilesData[0].Filename);
            Assert.AreEqual("416 KB", showFiles[3].DailyFilesData[0].FileSizeInKB);
            Assert.AreEqual("977e771c-1ed6-4345-8d01-fff728952f1b", showFiles[3].DailyFilesData[0].BatchId);
        }

        [Test]
        public async Task WhenCallShowDailyFilesAsyncWithDuplicateData_ThenReturnDailyLatestFiles()
        {
            IActionResult result = await _nMController.ShowDailyFilesAsync();
            List<ShowDailyFilesResponseModel> showFiles = (List<ShowDailyFilesResponseModel>)((ViewResult)result).Model;
            Assert.IsTrue(showFiles != null);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(5, showFiles[0].DailyFilesData.Count);
            Assert.AreEqual("21", showFiles[0].WeekNumber);
            Assert.AreEqual("2022", showFiles[0].Year);
            Assert.AreEqual("2022-05-24", showFiles[0].DailyFilesData[0].DataDate);
            Assert.AreEqual("Daily 24-05-22.zip", showFiles[0].DailyFilesData[0].Filename);
            Assert.AreEqual("416 KB", showFiles[0].DailyFilesData[0].FileSizeInKB);
            Assert.AreEqual("a8d14b93-42ab-455b-a4ed-39effecb8536", showFiles[0].DailyFilesData[0].BatchId);
        }

        [Test]
        public async Task WhenCallDownloadFile_ThenReturnFile()
        {
            const string batchId = "a738d0d3-bc1e-47ca-892a-9514ccef6464";
            const string filename = "21snii22_week_W2020_14.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            FileResult result = await _nMController.DownloadFile(batchId, filename, mimeType, frequency);
            Assert.IsNotNull(result);
            Assert.AreEqual("application/pdf", result.ContentType);
            Assert.AreEqual("https://filesqa.admiralty.co.uk", Config.BaseUrl);
            Assert.AreEqual(1072212, ((FileContentResult)result).FileContents.Length);
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
               , async delegate { await _nMController.DownloadFile(batchId, filename, mimeType, frequency); });
        }

        [Test]
        public async Task WhenCallDownloadDailyFile_ThenReturnFile()
        {
            const string batchId = "1882c04c-bc05-41b7-bf9b-11aeb5c5bd4a";
            const string filename = "DNM_Text.pdf";
            const string mimeType = "application/pdf";

            ActionResult result = await _nMController.DownloadDailyFile(batchId, filename, mimeType);
            Assert.IsTrue(((FileContentResult)result) != null);
            Assert.AreEqual("application/pdf", ((FileContentResult)result).ContentType);
            Assert.AreEqual(425602, ((FileContentResult)result).FileContents.Length);
            Assert.AreEqual("https://filesqa.admiralty.co.uk", Config.BaseUrl);
        }

        [Test]
        public void WhenCallDownloadDailyFileWithInvalidData_ThenThrowArgumentException()
        {
            const string batchId = "08e8cce6-e69d-46bd-832d-6fd3d4ef8740";
            const string filename = "Test.txt";
            const string mimeType = "application/txt";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>(),
                async delegate { await _nMController.DownloadDailyFile(batchId, filename, mimeType); });
        }

        [Test]
        public async Task WhenCallCumulativeAsync_ThenReturnCumulativeFiles()
        {
            IActionResult result = await _nMController.Cumulative();
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((ViewResult)result).Model;
            Assert.IsNotNull(listFiles);
            Assert.AreEqual(7, listFiles.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual("0cdb2271-b5a3-43b0-b923-733ada1760af", listFiles[0].BatchId);
            Assert.AreEqual("NP234(A) 2022", listFiles[0].FileDescription);
            Assert.AreEqual(".pdf", listFiles[0].FileExtension);
            Assert.AreEqual(1125181, listFiles[0].FileSize);
            Assert.AreEqual("NP234(A) 2022", listFiles[0].FileDescription);
            Assert.AreEqual("NP234(B) 2021", listFiles[1].FileDescription);
            Assert.AreEqual("NP234(A) 2021", listFiles[2].FileDescription);
            Assert.AreEqual("NP234(B) 2020", listFiles[3].FileDescription);
        }

        [Test]
        public async Task WhenCallCumulativeAsyncForDuplicateData_ThenReturnLatestCumulativeFiles()
        {
            IActionResult result = await _nMController.Cumulative();
            List<ShowFilesResponseModel> listFiles = (List<ShowFilesResponseModel>)((ViewResult)result).Model;
            Assert.IsNotNull(listFiles);
            Assert.AreEqual(7, listFiles.Count);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual("50044762-231d-41ec-a908-ba9eb59c61ab", listFiles[1].BatchId);
            Assert.AreEqual("NP234(B) 2021", listFiles[1].FileDescription);
            Assert.AreEqual(".pdf", listFiles[1].FileExtension);
            Assert.AreEqual(1386825, listFiles[1].FileSize);
            Assert.AreEqual("NP234(A) 2022", listFiles[0].FileDescription);
            Assert.AreEqual("NP234(B) 2021", listFiles[1].FileDescription);
            Assert.AreEqual("NP234(A) 2021", listFiles[2].FileDescription);
            Assert.AreEqual("NP234(B) 2020", listFiles[3].FileDescription);
        }

        [Test]
        public async Task WhenLeisureCalled_ThenReturnFiles()
        {
            IActionResult result = await _nMController.Leisure();
            List<ShowFilesResponseModel> showFiles = (List<ShowFilesResponseModel>)((ViewResult)result).Model;
            Assert.IsTrue(showFiles != null);
            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(3, showFiles.Count);
            Assert.AreEqual("application/pdf", showFiles[0].MimeType);
            Assert.AreEqual("5603SC5603_Falmouth_to_Hartland_Pt_incl_Isles_of_Scilly", showFiles[0].FileDescription);
            Assert.AreEqual(".pdf", showFiles[0].FileExtension);
            Assert.AreEqual(539264, showFiles[0].FileSize);
            Assert.AreEqual("527 KB", showFiles[0].FileSizeinKB);
            Assert.AreEqual("Leisure", showFiles[0].Attributes.First(x=>x.Key=="Frequency").Value);
            Assert.AreEqual("SC5603", showFiles[0].Attributes.First(x=>x.Key=="Chart").Value);
            Assert.AreEqual("dd36d1d4-3421-4402-b678-b52d19f5d325", showFiles[0].BatchId);
        }

      [Test]
        public async Task WhenLeisureCalledWithDuplicateData_ThenShouldReturnUniqueFiles()
        {
            IActionResult result = await _nMController.Leisure();
            List<ShowFilesResponseModel> showFiles = (List<ShowFilesResponseModel>)((ViewResult)result).Model;
            Assert.IsTrue(showFiles != null);
            
            List<string> lstChart = new();
            foreach (var file in showFiles) 
            {
                lstChart.Add(file.Attributes.FirstOrDefault(x => x.Key == "Chart").Value);            
            }
            Assert.AreEqual(lstChart.Distinct().Count(),lstChart.Count);

            Assert.AreEqual("MaritimeSafetyInformationIntegrationTest", Config.BusinessUnit);
            Assert.AreEqual("Notices to Mariners", Config.ProductType);
            Assert.AreEqual(3, showFiles.Count);
            Assert.AreEqual("application/pdf", showFiles[1].MimeType);
            Assert.AreEqual("5608SC5608_Bristol_Channel", showFiles[1].FileDescription);
            Assert.AreEqual(".pdf", showFiles[1].FileExtension);
            Assert.AreEqual(781199, showFiles[1].FileSize);
            Assert.AreEqual("763 KB", showFiles[1].FileSizeinKB);
            Assert.AreEqual("Leisure", showFiles[1].Attributes.First(x => x.Key == "Frequency").Value);
            Assert.AreEqual("SC5608", showFiles[1].Attributes.First(x => x.Key == "Chart").Value);
            Assert.AreEqual("f017aead-89d3-484d-9acc-e12842724e9e", showFiles[1].BatchId);
        }
    }
}
