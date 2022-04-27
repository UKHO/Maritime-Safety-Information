using UKHO.MaritimeSafetyInformation.Common.Models.DTO;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RNW
{
    public class RadioNavigationalWarningsAdminList
    {
        public List<RadioNavigationalWarnings> RadioNavigationalWarnings { get; set; }

        public List<WarningType> WarningTypes { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageCount { get; set; }

        public int WarningTypeId { get; set; }
    }
}
