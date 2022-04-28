using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning
{
    public class RadioNavigationalWarningsAdminListPage
    {
        public List<RadioNavigationalWarningsAdminList> RadioNavigationalWarningsAdminList { get; set; }

        public List<WarningType> WarningTypes { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageCount { get; set; }

        public int WarningTypeId { get; set; }
    }
}
