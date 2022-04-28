using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO
{
    public class WarningType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Warning Type")]
        [Required]
        public string Name { get; set; }
    }
}
