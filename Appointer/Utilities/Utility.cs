using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public static class Utility
{
    public static DateTime ToPersianDateTime(this DateTime datetime)
    {
        var pc = new PersianCalendar();
        return new DateTime(pc.GetYear(datetime), pc.GetMonth(datetime), pc.GetDayOfMonth(datetime), 0, 0, 0);
    }

    public static DateTime ToMiladiDateTime(this DateTime datetime)
    {
        var pc = new PersianCalendar();
        //DateTime.Now.DayOfWeek
        return pc.ToDateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0, 0);
    }

    public static bool IsPersianDateTime(this string datetime)
    {
        //return Regex.IsMatch(datetime, @"^(13\d{2}|[1-9]\d)/(1[012]|0?[1-9])/([12]\d|3[01]|0?[1-9])  ([01][0-9]|2[0-3]):([0-5]?[0-9]):([0-5]?[0-9])$");
        return Regex.IsMatch(datetime, @"^(13\d{2}|[1-9]\d)/(1[012]|0?[1-9])/([12]\d|3[01]|0?[1-9])$");
    }
}