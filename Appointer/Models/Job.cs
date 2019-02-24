namespace Appointer.Models
{
    public class Job : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}