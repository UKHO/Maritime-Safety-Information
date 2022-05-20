using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning
{
    [ExcludeFromCodeCoverage]
    public class RadioNavigationalWarningsAdminFilter
    {
        public int SrNo { get; set; }

        public List<RadioNavigationalWarningsAdmin> RadioNavigationalWarningsAdminList { get; set; }

        public List<WarningType> WarningTypes { get; set; }

        public List<string> Years { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageCount { get; set; }

        public int? WarningType { get; set; }

        public int? Year { get; set; }
    }
}
