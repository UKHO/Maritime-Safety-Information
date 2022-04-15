using Microsoft.AspNetCore.Mvc;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class NoticestoMarinersController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFileShareService fileShareService;
        private readonly INMDataService nMDataService;
        public NoticestoMarinersController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IFileShareService fileShareService, INMDataService nMDataService)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.fileShareService = fileShareService;
            this.nMDataService = nMDataService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync()
        {
            string accessToken = "";
            IEnumerable<BatchDetailsFiles> listFiles = await nMDataService.GetBatchDetailsFiles(2022, 14, accessToken);
            return View(listFiles);
        }

        [HttpGet]
        [Route("/batch/{batchId}/files/{filename}")]
        public async Task<IActionResult> DownloadFssFilesAsync(string batchId, string filename)
        {
            string accessToken = "";
            byte[] fileBytes = await nMDataService.DownloadFssFileAsync(batchId, filename, accessToken);
            return File(fileBytes, "application/octet-stream", filename);
        }
    }
}
