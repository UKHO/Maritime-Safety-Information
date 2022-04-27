using Microsoft.EntityFrameworkCore;
using UKHO.MaritimeSafetyInformation.Common.Models.DTO;

namespace UKHO.MaritimeSafetyInformation.Common
{
    public class RadioNavigationalWarningsContext : DbContext
    {
        public RadioNavigationalWarningsContext(DbContextOptions<RadioNavigationalWarningsContext> options)
            : base(options)
        {

        }

        public DbSet<RadioNavigationalWarnings> RadioNavigationalWarnings { get; set; }

    }
}
