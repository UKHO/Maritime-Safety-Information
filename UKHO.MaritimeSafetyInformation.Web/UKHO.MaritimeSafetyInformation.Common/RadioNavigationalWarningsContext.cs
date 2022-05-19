using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Common
{
    public class RadioNavigationalWarningsContext : DbContext
    {
        public RadioNavigationalWarningsContext(DbContextOptions<RadioNavigationalWarningsContext> options)
            : base(options)
        {

        }

        public DbSet<RadioNavigationalWarning> RadioNavigationalWarnings { get; set; }
        public DbSet<WarningType> WarningType { get; set; }
    }
}
