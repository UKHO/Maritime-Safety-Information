using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models;

namespace UKHO.MaritimeSafetyInformation.Common.Helper
{
    public static class NMHelper
    {
        public static List<ShowFilesResponseModel> GetShowFilesResponses(BatchSearchResponse SearchResult)
        {
            List<ShowFilesResponseModel> ListshowFilesResponseModels = new();
            foreach (BatchDetails item in SearchResult.Entries)
            {
                foreach (BatchDetailsFiles file in item.Files)
                {
                    ListshowFilesResponseModels.Add(new ShowFilesResponseModel
                    {
                        BatchId = item.BatchId,
                        Filename = file.Filename,
                        FileDescription = Path.GetFileNameWithoutExtension(file.Filename),
                        FileExtension = Path.GetExtension(file.Filename),
                        FileSize = file.FileSize,
                        FileSizeinKB = FileHelper.FormatSize((long)file.FileSize),
                        MimeType = file.MimeType,
                        Links = file.Links,
                    });
                }
            }
            return ListshowFilesResponseModels;
        }
    }
}
