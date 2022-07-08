using UKHO.FileShareClient.Models;

namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    public class BatchAttributesSearchModel
    {
        public bool IsSuccess { get; set; }

        public int StatusCode { get; set; }

        public List<Error> Errors { get; set; }

        public BatchAttributesSearchResponse Data { get; set; }

        public bool AttributeYearAndWeekIsCache { get; set; }
    }
}
