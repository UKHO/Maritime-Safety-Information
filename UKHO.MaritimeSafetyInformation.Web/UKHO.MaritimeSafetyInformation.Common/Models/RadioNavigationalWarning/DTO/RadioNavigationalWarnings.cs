using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO
{
    public class RadioNavigationalWarnings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Warning Type")]
        [Required]
        public int WarningType { get; set; }

        [Required]
        public string Reference { get; set; }

        [DisplayName("Date Time Group")]
        [Required]
        public DateTime DateTimeGroup { get; set; }

        [DisplayName("Description")]
        [Required]
        public string Summary { get; set; }

        [DisplayName("Text")]
        public string Content { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
