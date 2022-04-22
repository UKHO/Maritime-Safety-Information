using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.MaritimeSafetyInformation.Web.Models

{
    public class RadioNavigationalWarnings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(32)")]
        [DisplayName("Warning Type")]
        [Required]
        public string? WarningType { get; set; }

        [Column(TypeName = "nvarchar(32)")]
        [Required]
        public string? Reference { get; set; }

        [DisplayName("DateTime Group")]
        [Required]
        public DateTime DateTimeGroup { get; set; }

        [Column(TypeName = "nvarchar(32)")]
        [Required]
        public string? Description { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Text { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool ApprovalStatus { get; set; }
        public bool IsDeleted { get; set; }
    }
}
