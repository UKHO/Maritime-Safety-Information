using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;
using UKHO.MaritimeSafetyInformation.Web.Controllers;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.UnitTests.Controllers
{
    [TestFixture]
    public class NoticesToMarinersControllerTest
    {
        private NoticesToMarinersController controller;
        private ILogger<NoticesToMarinersController> fakeLogger;
        private IHttpContextAccessor fakeContextAccessor;
        private INMDataService fakeNMDataService;
        private IUserService fakeUserService;

        private const string CorrelationId = "7b838400-7d73-4a64-982b-f426bddc1296";

        [SetUp]
        public void Setup()
        {
            fakeLogger = A.Fake<ILogger<NoticesToMarinersController>>();
            fakeContextAccessor = A.Fake<IHttpContextAccessor>();
            fakeNMDataService = A.Fake<INMDataService>();
            fakeUserService = A.Fake<IUserService>();
            A.CallTo(() => fakeContextAccessor.HttpContext).Returns(new DefaultHttpContext());
            controller = new NoticesToMarinersController(fakeNMDataService, fakeContextAccessor, fakeLogger, fakeUserService);
        }

        [Test]
        public async Task WhenIndexIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022;
            const int week = 20;
            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(SetResultForShowWeeklyFilesResponseModel());

            IActionResult result = await controller.Index();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(year, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Year"])));
            Assert.That(week, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Week"])));
            Assert.That(false, Is.EqualTo(controller.ViewBag.IsDistributor));
        }

        [Test]
        public async Task WhenIndexIsCalledForNoBatchData_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022;
            const int week = 20;

            ShowWeeklyFilesResponseModel showWeeklyFilesResponseModel = SetResultForShowWeeklyFilesResponseModel();

            showWeeklyFilesResponseModel.ShowFilesResponseList = new System.Collections.Generic.List<ShowFilesResponseModel>();

            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(showWeeklyFilesResponseModel);

            IActionResult result = await controller.Index();
            Assert.That(result, Is.InstanceOf<ViewResult>());

            string actualView = ((ViewResult)result).ViewName;

            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(year, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Year"])));
            Assert.That(week, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Week"])));
            Assert.That(true, Is.EqualTo(controller.ViewBag.HasError));
        }

        [Test]
        public void WhenIndexIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = controller.Index();

            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenIndexPostIsCalled_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022;
            const int week = 20;

            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(SetResultForShowWeeklyFilesResponseModel());

            IActionResult result = await controller.Index(year, week);
            Assert.That(result, Is.InstanceOf<ViewResult>());

            string actualView = ((ViewResult)result).ViewName;

            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(year, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Year"])));
            Assert.That(week, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Week"])));
            Assert.That(false, Is.EqualTo(controller.ViewBag.IsDistributor));
        }

        [Test]
        public async Task WhenIndexPostIsCalledForNoBatchData_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022;
            const int week = 20;

            ShowWeeklyFilesResponseModel showWeeklyFilesResponseModel = SetResultForShowWeeklyFilesResponseModel();

            showWeeklyFilesResponseModel.ShowFilesResponseList = new System.Collections.Generic.List<ShowFilesResponseModel>();

            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(showWeeklyFilesResponseModel);

            IActionResult result = await controller.Index(year, week);
            Assert.That(result, Is.InstanceOf<ViewResult>());

            string actualView = ((ViewResult)result).ViewName;

            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(year, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Year"])));
            Assert.That(week, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Week"])));
            Assert.That(true, Is.EqualTo(controller.ViewBag.HasError));

        }

        [Test]
        public async Task WhenIndexPostIsCalledWithYearAndWeekZero_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 0;
            const int week = 0;
            const int expectedViewCount = 4;

            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(SetResultForShowWeeklyFilesResponseModel());

            IActionResult result = await controller.Index(year, week);
            Assert.That(result, Is.InstanceOf<ViewResult>());

            string actualView = ((ViewResult)result).ViewName;

            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(expectedViewCount, Is.EqualTo(((ViewResult)result).ViewData.Count));
        }

        [Test]
        public async Task WhenIndexPostIsCalledWithWeekOrYearZero_ThenShouldReturnsExpectedViewAndViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Index.cshtml";
            const int year = 2022;
            const int week = 0;

            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Returns(SetResultForShowWeeklyFilesResponseModel());

            IActionResult result = await controller.Index(year, week);
            Assert.That(result, Is.InstanceOf<ViewResult>());

            string actualView = ((ViewResult)result).ViewName;

            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(year, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Year"])));
            Assert.That(week, Is.EqualTo(Convert.ToInt32(((ViewResult)result).ViewData["Week"])));
        }

        [Test]
        public void WhenIndexPostIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            const int year = -1;
            const int week = -1;

            A.CallTo(() => fakeNMDataService.GetWeeklyFilesResponseModelsAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = controller.Index(year, week);
            Assert.That(result.IsFaulted);
        }

        [Test]
        public void WhenShowWeeklyFilesAsyncIsCalled_ThenShouldThrowException()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => fakeNMDataService.GetWeeklyBatchFiles(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = controller.ShowWeeklyFilesAsync(year, week);
            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenShowWeeklyFilesAsyncIsCalled_ThenShouldReturnsExpectedPartialView()
        {
            const int year = 2022;
            const int week = 15;

            const string expectedView = "~/Views/NoticesToMariners/ShowWeeklyFilesList.cshtml";

            A.CallTo(() => fakeNMDataService.GetWeeklyBatchFiles(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored));

            IActionResult result = await controller.ShowWeeklyFilesAsync(year, week);
            Assert.That(result, Is.InstanceOf<PartialViewResult>());
            string actualView = ((PartialViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
        }

        [Test]
        public async Task WhenShowDailyFilesAsyncIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/ShowDailyFiles.cshtml";

            A.CallTo(() => fakeNMDataService.GetDailyBatchDetailsFiles(CorrelationId));

            IActionResult result = await controller.ShowDailyFilesAsync();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
        }

        [Test]
        public async Task WhenShowDailyFilesAsyncIsCalledAndExceptionThrownByService_ThenShouldReturnExpectedViewWithViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/ShowDailyFiles.cshtml";

            A.CallTo(() => fakeNMDataService.GetDailyBatchDetailsFiles(A<string>.Ignored)).Throws(new Exception());

            IActionResult result = await controller.ShowDailyFilesAsync();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
        }

        [Test]
        public void WhenDownloadFileIsCalledWithNullBatchID_ThenShouldReturnArgumentNullException()
        {
            const string batchId = null;
            const string fileName = "testfile.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter BatchId"),
                async delegate { await controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithEmptyBatchID_ThenShouldReturnArgumentNullException()
        {
            const string batchId = "";
            const string fileName = "testfile.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter BatchId"),
                async delegate { await controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithNullFileName_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = null;
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter FileName"),
                async delegate { await controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithEmptyFileName_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter FileName"),
               async delegate { await controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithNullMimeType_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = null;
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter MimeType"),
               async delegate { await controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public void WhenDownloadFileIsCalledWithEmptyMimeType_ThenShouldReturnArgumentNullException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = "";
            const string frequency = "Weekly";
            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Invalid value received for parameter MimeType"),
               async delegate { await controller.DownloadFile(batchId, fileName, mimeType, frequency); });
        }

        [Test]
        public async Task WhenDownloadFileIsCalled_ThenShouldReturnFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = "application/pdf";
            const string frequency = "Weekly";

            A.CallTo(() => fakeNMDataService.DownloadFssFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));
            IActionResult result = await controller.DownloadFile(batchId, fileName, mimeType, frequency);

            Assert.That(result, Is.InstanceOf<FileResult>());
        }

        [Test]
        public void WhenDownloadFileIsCalled_ThenShouldExecuteCatch()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "testfile.pdf";
            const string mimeType = "wrongmime";
            const string frequency = "Weekly";

            A.CallTo(() => fakeNMDataService.DownloadFssFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());
            fakeContextAccessor.HttpContext.Response.Headers.Append("Content-Disposition", "Test");

            Task<FileResult> result = controller.DownloadFile(batchId, fileName, mimeType, frequency);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenDownloadDailyFileIsCalled_ThenShouldReturnFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            const string mimeType = "application/gzip";

            A.CallTo(() => fakeNMDataService.DownloadFSSZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));
            IActionResult result = await controller.DownloadDailyFile(batchId, fileName, mimeType);

            Assert.That(result, Is.InstanceOf<FileResult>());
        }

        [Test]
        public void WhenDownloadDailyFileIsCalled_ThenShouldReturnShowDailyFilesAction()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "Daily 16-05-22.zip";
            const string mimeType = "application/gzip";

            A.CallTo(() => fakeNMDataService.DownloadFSSZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());

            fakeContextAccessor.HttpContext.Response.Headers.Append("Content-Disposition", "Test");

            Task<FileResult> result = controller.DownloadDailyFile(batchId, fileName, mimeType);

            Assert.That(result.IsFaulted);

        }

        [TestCase(null, "Daily 16-05-22.zip", "application/gzip", Description = "When Download Daily File Is Called With Null BatchID Then Should Throw Exception")]
        [TestCase("", "Daily 16-05-22.zip", "application/gzip", Description = "When Download Daily File Is Called With Empty BatchID Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d1", "Daily 16-05-22.zip", null, Description = "When Download Daily File Is Called With Null Mime Type Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d2", "Daily 16-05-22.zip", "", Description = "When Download Daily File Is Called With Empty Mime Type Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d3", null, "application/gzip", Description = "When Download Daily File Is Called With Null File Name Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d4", "", "application/gzip", Description = "When Download Daily File Is Called With Empty File Name Then Should Then Should Throw Exception")]
        public void WhenDownloadDailyFileIsCalledWithEmptyValue_ThenShouldThrowException(string batchId, string fileName, string mimeType)
        {
            Task<FileResult> result = controller.DownloadDailyFile(batchId, fileName, mimeType);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public void WhenAboutIsCalled_ThenReturnView()
        {
            const string expectedView = "~/Views/NoticesToMariners/About.cshtml";

            IActionResult result = controller.About();
            Assert.That(result, Is.InstanceOf<IActionResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
        }

        [Test]
        public async Task WhenCumulativeFilesAsyncIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Cumulative.cshtml";

            A.CallTo(() => fakeNMDataService.GetCumulativeBatchFiles(CorrelationId));

            IActionResult result = await controller.Cumulative();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
        }

        [Test]
        public async Task WhenCumulativeIsCalledAndExceptionThrownByService_ThenShouldReturnExpectedViewWithViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Cumulative.cshtml";

            A.CallTo(() => fakeNMDataService.GetCumulativeBatchFiles(A<string>.Ignored)).Throws(new Exception());

            IActionResult result = await controller.Cumulative();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
        }

        [Test]
        public void WhenShowWeeklyFilesAsyncIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            const int year = 2022;
            const int week = 15;

            A.CallTo(() => fakeNMDataService.GetWeeklyBatchFiles(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored)).Throws(new Exception());

            Task<IActionResult> result = controller.ShowWeeklyFilesAsync(year, week);

            Assert.That(result.IsFaulted);
        }

        [Test]
        public async Task WhenAnnualFilesAsyncIsCalled_ThenShouldReturnsExpectedView()
        {
            const string expectedView = "~/Views/NoticesToMariners/Annual.cshtml";

            A.CallTo(() => fakeNMDataService.GetAnnualBatchFiles(CorrelationId));

            IActionResult result = await controller.Annual();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
        }

        [Test]
        public async Task WhenAnnualIsCalledAndExceptionThrownByService_ThenShouldReturnExpectedViewWithViewData()
        {
            const string expectedView = "~/Views/NoticesToMariners/Annual.cshtml";

            A.CallTo(() => fakeNMDataService.GetAnnualBatchFiles(A<string>.Ignored)).Throws(new Exception());

            IActionResult result = await controller.Annual();
            Assert.That(result, Is.InstanceOf<ViewResult>());
            string actualView = ((ViewResult)result).ViewName;
            Assert.That(expectedView, Is.EqualTo(actualView));
            Assert.That(((ViewResult)result).ViewData.ContainsKey("CurrentCorrelationId"));
        }

        [Test]
        public async Task WhenDownloadAllWeeklyZipFileIsCalled_ThenShouldReturnFileResult()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "2022 Wk 29 Weekly NMs.zip";
            const string mimeType = "application/gzip";
            const string type = "public";

            A.CallTo(() => fakeNMDataService.DownloadFSSZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored));
            IActionResult result = await controller.DownloadAllWeeklyZipFile(batchId, fileName, mimeType, type);

            Assert.That(result, Is.InstanceOf<FileResult>());
        }

        [Test]
        public void WhenDownloadAllWeeklyZipFileIsCalledAndExceptionThrownByService_ThenShouldThrowException()
        {
            string batchId = Guid.NewGuid().ToString();
            const string fileName = "2022 Wk 29 Weekly NMs.zip";
            const string mimeType = "application/gzip";
            const string type = "public";

            A.CallTo(() => fakeNMDataService.DownloadFSSZipFileAsync(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception());

            fakeContextAccessor.HttpContext.Response.Headers.Append("Content-Disposition", "Test");

            Task<FileResult> result = controller.DownloadAllWeeklyZipFile(batchId, fileName, mimeType, type);

            Assert.That(result.IsFaulted);

        }

        [TestCase(null, "2022 Wk 26 Weekly NMs.zip", "application/gzip", "public", Description = "When Download All Weekly File Is Called With Null BatchID Then Should Throw Exception")]
        [TestCase("", "2022 Wk 26 Weekly NMs.zip", "application/gzip", "partner", Description = "When Download All Weekly File Is Called With Empty BatchID Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d1", "2022 Wk 26 Weekly NMs.zip", null, "public", Description = "When Download All Weekly File Is Called With Null Mime Type Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d2", "2022 Wk 26 Weekly NMs.zip", "", "partner", Description = "When Download All Weekly File Is Called With Empty Mime Type Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d3", null, "application/gzip", "public", Description = "When Download All Weekly File Is Called With Null File Name Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d4", "", "application/gzip", "partner", Description = "When Download All Weekly File Is Called With Empty File Name Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d4", "2022 Wk 26 Weekly NMs.zip", "application/gzip", null, Description = "When Download All Weekly File Is Called With Null type Then Should Then Should Throw Exception")]
        [TestCase("03f8ee96-62c4-461a-9fe4-f03e46abc2d4", "2022 Wk 26 Weekly NMs.zip", "application/gzip", "", Description = "When Download All Weekly File Is Called With Empty type Then Should Then Should Throw Exception")]
        public void WhenDownloadAllWeeklyZipFileIsCalledWithEmptyValue_ThenShouldThrowException(string batchId, string fileName, string mimeType, string type)
        {
            Task<FileResult> result = controller.DownloadAllWeeklyZipFile(batchId, fileName, mimeType, type);

            Assert.That(result.IsFaulted);
        }

        private static ShowWeeklyFilesResponseModel SetResultForShowWeeklyFilesResponseModel()
        {
            return new()
            {
                ShowFilesResponseList = new()
                {
                    new ShowFilesResponseModel()
                    {
                        BatchId = "1",
                        Filename = "aaa.pdf",
                        FileDescription = "aaa",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    },
                    new ShowFilesResponseModel()
                    {
                        BatchId = "1",
                        Filename = "bbb.pdf",
                        FileDescription = "bbb",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    },
                    new ShowFilesResponseModel()
                    {
                        BatchId = "2",
                        Filename = "ccc.pdf",
                        FileDescription = "ccc",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    },
                    new ShowFilesResponseModel()
                    {
                        BatchId = "2",
                        Filename = "ddd.pdf",
                        FileDescription = "ddd",
                        FileExtension = ".pdf",
                        FileSize = 1232,
                        FileSizeinKB = "1 KB",
                        MimeType = "PDF",
                        Links = null
                    }
                },
                YearAndWeekList = new()
                {
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 17
                    },
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 18
                    },
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 19
                    },
                    new YearWeekModel()
                    {
                        Year = 2022,
                        Week = 20
                    }
                }
            };
        }
    }
}
