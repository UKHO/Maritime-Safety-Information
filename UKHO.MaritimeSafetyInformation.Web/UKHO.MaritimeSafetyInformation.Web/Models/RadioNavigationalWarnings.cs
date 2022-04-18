using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.MaritimeSafetyInformation.Web.Models
{
    public class RadioNavigationalWarnings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? WarningType { get; set; }
        public string? Reference { get; set; }
        public DateTime DateTimeGroup { get; set; }
        public string? Description { get; set; }
        public string? Text { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool ApprovalStatus { get; set; }
        public bool IsDeleted { get; set; }
    }
}
