using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Configuration;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO
{
    [ExcludeFromCodeCoverage]
    public class RadioNavigationalWarning
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Warning Type")]
        [Required]
        [Range(WarningTypes.NAVAREA_1, WarningTypes.UK_Coastal)]
        public int WarningType { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "Reference cannot be longer than 32 characters.")]
        public string Reference { get; set; }

        [DisplayName("Date Time Group")]
        [Required]
        public DateTime DateTimeGroup { get; set; }

        [DisplayName("Description")]
        [Required]
        [StringLength(256, ErrorMessage = "Summary cannot be longer than 256 characters.")]
        public string Summary { get; set; }

        [DisplayName("Text")]
        [Required]
        public string Content { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}
