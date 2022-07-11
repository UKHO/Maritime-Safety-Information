
using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Common.Models.NoticesToMariners
{
    [ExcludeFromCodeCoverage]
    public class ShowWeeklyFilesResponseModel
    {
        public List<ShowFilesResponseModel> ShowFilesResponseList { get; set; }
        public List<YearWeekModel> YearAndWeekList { get; set; }
        public bool AttributeYearAndWeekIsCached { get; set; }
        public bool WeeklyNMFilesIsCached { get; set; }
    }
}
