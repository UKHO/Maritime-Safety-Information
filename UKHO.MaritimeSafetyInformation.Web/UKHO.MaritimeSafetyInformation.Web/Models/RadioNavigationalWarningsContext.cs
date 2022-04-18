using Microsoft.EntityFrameworkCore;

namespace UKHO.MaritimeSafetyInformation.Web.Models
{
    public class RadioNavigationalWarningsContext : DbContext
    {
        public RadioNavigationalWarningsContext(DbContextOptions<RadioNavigationalWarningsContext> options)
            : base(options)
        {
           
        }

        public DbSet<RadioNavigationalWarnings>? RadioNavigationalWarnings { get; set; }

    }
}
