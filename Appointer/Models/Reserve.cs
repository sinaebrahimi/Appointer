using System;
using System.Data.Entity.ModelConfiguration;

namespace Appointer.Models
{
    public class Reserve : BaseEntity
    {
        public string Description { get; set; }
        public DateTime Meeting { get; set; }
        public TimeSpan Duration { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Job Job { get; set; }
    }

    public class ReserveConfig : EntityTypeConfiguration<Reserve>
    {
        public ReserveConfig()
        {
            Property(p => p.Description).HasMaxLength(1024);
        }
    }
}