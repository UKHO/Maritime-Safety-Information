using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Web.Configuration;
using UKHO.MaritimeSafetyInformation.Web.Models;
using UKHO.MaritimeSafetyInformation.Web.Services.Interfaces;

namespace UKHO.MaritimeSafetyInformation.Web.Services
{
    public class NMDataService : INMDataService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFileShareService fileShareService;
        private readonly IConfiguration configuration;
        private readonly FileShareServiceConfiguration fileShareServiceConfig;
        public NMDataService(IFileShareService fileShareService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.fileShareService = fileShareService;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.fileShareServiceConfig = configuration.GetSection("FileShareService").Get<FileShareServiceConfiguration>();
        }
        public async Task<List<ShowFilesResponseModel>> GetBatchDetailsFiles(int year, int week, string accessToken)
        {
            string searchText = $"BusinessUnit eq '{fileShareServiceConfig.BusinessUnit}' and $batch(Product Type) eq '{fileShareServiceConfig.ProductType}' and $batch(Frequency) eq 'Weekly' and $batch(Year) eq '{year}' and $batch(Week Number) eq '{week}'";
            var result = await fileShareService.FssWeeklySearchAsync(searchText, accessToken);

            BatchSearchResponse SearchResult = result.Data;
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new List<ShowFilesResponseModel>();
            if (SearchResult.Entries.Count > 0)
            {
                ListshowFilesResponseModels = GetShowFilesResponses(SearchResult);
                ListshowFilesResponseModels = ListshowFilesResponseModels.OrderBy(e => e.FileDescription).ToList();
            }
            return ListshowFilesResponseModels;

        }

        private List<ShowFilesResponseModel> GetShowFilesResponses(BatchSearchResponse SearchResult)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new List<ShowFilesResponseModel>();
            foreach (var item in SearchResult.Entries)
            {
                foreach (var file in item.Files)
                {
                    ListshowFilesResponseModels.Add(new ShowFilesResponseModel
                    {
                        BatchId = item.BatchId,
                        Filename = file.Filename,
                        FileDescription = Path.GetFileNameWithoutExtension(file.Filename),
                        FileExtension = Path.GetExtension(file.Filename),
                        FileSize = file.FileSize,
                        FileSizeinKB = FormatSize((long)file.FileSize),
                        MimeType = file.MimeType,
                        Links = file.Links,
                    });
                }
            }
            return ListshowFilesResponseModels;
        }

        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        public async Task<byte[]> DownloadFssFileAsync(string batchId, string filename, string accessToken)
        {
            Stream stream = await fileShareService.GetFssFileStreamAsync(batchId, filename, accessToken);
            byte[] fileBytes = new byte[stream.Length];
            stream.Read(fileBytes, 0, fileBytes.Length);
            return fileBytes;
        }
    }
}
