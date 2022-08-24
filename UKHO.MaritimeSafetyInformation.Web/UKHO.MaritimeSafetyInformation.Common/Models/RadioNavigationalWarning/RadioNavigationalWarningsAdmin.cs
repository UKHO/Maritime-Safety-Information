using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning
{
    [ExcludeFromCodeCoverage]
    public class RadioNavigationalWarningsAdmin
    {
        public int Id { get; set; }

        public int WarningType { get; set; }

        [DisplayName("Warning Type")]
        public string WarningTypeName { get; set; }

        public string Reference { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        [DisplayName("Date/Time")]
        public DateTime DateTimeGroup { get; set; }

        [DisplayName("Date/Time")]
        public string DateTimeGroupRnwFormat { get; set; }

        [DisplayName("Description")]
        public string Summary { get; set; }

        [DisplayName("Text")]
        public string Content { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [DisplayName("Expiry Date")]
        public string ExpiryDateRnwFormat { get; set;}

        public string Status { get; set; }
    }
}
