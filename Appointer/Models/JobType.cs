using System.ComponentModel;

namespace Appointer.Models
{
    public enum JobType : byte
    {
        [Description("آرایشگاه")]
        BarberShop = 0,
        [Description("مطب پزشک")]
        Clinic = 1,
    }
}