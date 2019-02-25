using System.ComponentModel;

namespace Appointer.Models
{
    public enum WorkingHour : byte
    {
        [Description("صبح")]
        Morning = 0,
        [Description("بعدازظهر")]
        Afternoon = 1,
        [Description("شب")]
        Night = 2,
        [Description("نیمه‌شب")]
        Midnight = 4,
    }
}