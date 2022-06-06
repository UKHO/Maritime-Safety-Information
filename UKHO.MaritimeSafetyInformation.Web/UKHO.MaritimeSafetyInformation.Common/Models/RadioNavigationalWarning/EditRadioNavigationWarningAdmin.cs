using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning
{
    [ExcludeFromCodeCoverage]
    public class EditRadioNavigationalWarningsAdmin
    {
        public int Id { get; set; }

        [DisplayName("Warning Type")]
        [Required]
        [Range(WarningTypes.NAVAREA_1, WarningTypes.UK_Coastal)]
        public int WarningType { get; set; }

        [DisplayName("Warning Type")]
        public string WarningTypeName { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "Reference cannot be longer than 32 characters.")]
        public string Reference { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        [DisplayName("Date/Time")]
        public DateTime DateTimeGroup { get; set; }

        
        [DisplayName("Description")]
        [Required]
        [StringLength(256, ErrorMessage = "Summary cannot be longer than 256 characters.")]
        public string Summary { get; set; }

        [DisplayName("Text")]
        [Required]
        public string Content { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        
        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; }
    }
}
