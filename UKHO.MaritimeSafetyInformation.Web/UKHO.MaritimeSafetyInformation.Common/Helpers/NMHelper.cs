using UKHO.FileShareClient.Models;
using UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners;

namespace UKHO.MaritimeSafetyInformation.Common.Helpers
{
    public static class NMHelper
    {
        public static List<ShowFilesResponseModel> ListFilesResponse(BatchSearchResponse SearchResult)
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
