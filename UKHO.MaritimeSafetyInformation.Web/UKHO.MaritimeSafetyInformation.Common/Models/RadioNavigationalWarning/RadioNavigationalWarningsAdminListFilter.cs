using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning
{
    public class RadioNavigationalWarningsAdminListFilter
    {
        public List<RadioNavigationalWarningsAdminList> RadioNavigationalWarningsAdminList { get; set; }

        public List<WarningType> WarningTypes { get; set; }

        public List<string> Years { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageCount { get; set; }

        public int WarningType { get; set; }

        public string Year { get; set; }
    }
}
