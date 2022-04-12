using Microsoft.AspNetCore.Mvc;
using UKHO.FileShareClient;
using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Web.Controllers
{
    public class NoticestoMarinersController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        protected readonly IHttpClientFactory httpClientFactory;
        public NoticestoMarinersController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShowWeeklyFilesAsync()
        {
            string accessToken = "";
            FileShareApiClient fileShareApi = new FileShareApiClient(httpClientFactory, "https://filesqa.admiralty.co.uk", accessToken);
            var result = await fileShareApi.Search("$batch(Year) eq '2022' and $batch(Week Number) eq '14'", 25, 0, CancellationToken.None);

            string SearchResultAsJson = result.Data.ToJson();
            BatchSearchResponse SearchResult = result.Data;
            IEnumerable<BatchDetailsFiles> listFiles = SearchResult.Entries.SelectMany(e => e.Files).ToList();
            return View(listFiles);
        }
    }
}
