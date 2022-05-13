using System.ComponentModel;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning
{
    public class RadioNavigationalWarningsData
    {
        public string Reference { get; set; }

        [DisplayName("Date Time Group")]
        public DateTime DateTimeGroup { get; set; }

        public string Description { get; set; }
    }
}
