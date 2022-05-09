using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UKHO.MaritimeSafetyInformation.Common.Models.RadioNavigationalWarning.DTO;

namespace UKHO.MaritimeSafetyInformation.Common
{
    [ExcludeFromCodeCoverage]
    public class RadioNavigationalWarningsContext : DbContext
    {
        public RadioNavigationalWarningsContext(DbContextOptions<RadioNavigationalWarningsContext> options)
            : base(options)
        {

        }

        public DbSet<RadioNavigationalWarnings> RadioNavigationalWarnings { get; set; }
        public DbSet<WarningType> WarningType { get; set; }

    }
}
