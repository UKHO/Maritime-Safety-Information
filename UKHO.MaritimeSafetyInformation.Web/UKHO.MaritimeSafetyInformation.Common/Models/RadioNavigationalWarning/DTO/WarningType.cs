using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO
{
    [ExcludeFromCodeCoverage]
    public class WarningType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
