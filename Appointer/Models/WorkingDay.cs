using System.ComponentModel;

namespace Appointer.Models
{
    public enum WorkingDay : byte
    {
        [Description("شنبه")]
        Saturday = 0,
        [Description("یک‌شنبه")]
        Sunday = 1,
        [Description("دوشنبه")]
        Friday = 2,
        [Description("سه‌شنبه")]
        Monday = 4,
        [Description("چهارشنبه")]
        Thursday = 8,
        [Description("پنج‌شنبه")]
        Tuesday = 16,
        [Description("جمعه")]
        Wednesday = 32
    }
}