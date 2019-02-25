using System.Data.Entity.ModelConfiguration;

namespace Appointer.Models
{
    public class Job : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public WorkingDay WorkingDay { get; set; }
        public WorkingHour WorkingHour { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }

    public class JobConfig : EntityTypeConfiguration<Job>
    {
        public JobConfig()
        {
            Property(p => p.Title).IsRequired().HasMaxLength(128);
            Property(p => p.PhoneNumber).HasMaxLength(16);
            Property(p => p.Mobile).HasMaxLength(16);
            Property(p => p.Email).HasMaxLength(64);
            Property(p => p.WorkingDay).IsRequired();
            Property(p => p.WorkingHour).IsRequired();
        }
    }
}