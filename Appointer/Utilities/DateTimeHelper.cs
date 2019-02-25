using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Appointer.Utility
{
    public static class DateTimeHelper
    {
        public static string PersianDateTimeRegex = @"^(((13|14)|20)\d{2})-(1[012]|0?[1-9])-([12]\d|3[01]|0?[1-9])$";
        public static string GeorgianDateTimeRegex = "(0[1-9]|1[0-2])-([0][1-9]|[1-2][0-9]|3[0-1])-(20[1-9][0-9])";
        public static string FixDatePersian(this string date)
        {
            // convert to Standard 13950101 and 1395-01-01 1395/01/01
            if (Regex.IsMatch(date, @"^(13\d{2}|[1-9]\d)(1[012]|0?[1-9])([12]\d|3[01]|0?[1-9])$"))
                return date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
            else if (Regex.IsMatch(date, @"^(13\d{2}|[1-9]\d)-(1[012]|0?[1-9])-([12]\d|3[01]|0?[1-9])$"))
                return date.Replace('-', '/');
            else if (Regex.IsMatch(date, @"^(13\d{2}|[1-9]\d)/(1[012]|0?[1-9])/([12]\d|3[01]|0?[1-9])$"))
                return date;
            else
                return null;
        }
        public static string ToShortDateStringStandard(this DateTime datetime, string format = "yyyy-MM-dd")
        {
            return datetime.ToString(format);
        }
        public static string ToEnDateTime(this DateTime datetime, string format = "yyyy-MM-dd")
        {

            return datetime.ToString(format, CultureHelper.GetCultureInfo("en-US"));
        }

        public static string ToFaDateTime(this DateTime datetime, string format = "yyyyMMdd")
        {
            return datetime.ToString(format, CultureHelper.GetCultureInfo("fa-IR"));
        }

        public static DateTime FaStringToDateTime(this string datetime)
        {
            return Convert.ToDateTime(datetime, CultureHelper.GetCultureInfo("fa-IR"));
        }

        public static DateTime FaStringToDateTime(this string datetime, string format = "yyyy/MM/dd hh:mm")
        {
            return DateTime.ParseExact(datetime, format, CultureHelper.GetCultureInfo("en-US"));
        }

        public static DateTime EnStringToDateTime(this string datetime)
        {
            return Convert.ToDateTime(datetime, CultureHelper.GetCultureInfo("fa-IR"));
        }

        public static DateTime EnStringToDateTime(this string datetime, string format = "yyyy/MM/dd hh:mm")
        {
            return DateTime.ParseExact(datetime, format, CultureHelper.GetCultureInfo("en-US"));
        }

        public static DateTime ToPersianDateTime(this DateTime dt)
        {
            return dt;
        }

        public static TimeSpan ToTimeSpan(this string time)
        {
            return TimeSpan.Parse(time.StartsWith("24:00") ? "00:00" : time);
        }

        //public static DateTime? ToNullableDateTime(this object obj)
        //{
        //    if (obj == null)
        //        return null;
        //    var arr = obj.ToString().FixDateTime().Split(' ');

        //    if (arr.Length == 1) //date
        //        return PersianDateTime.Parse(arr[0]).ToDateTime();
        //    return PersianDateTime.Parse(arr[0], arr[1]).ToDateTime();
        //}
        //public static PersianDateTime ToPersianDateTime(this DateTime DT)
        //{
        //    return new PersianDateTime(DT);
        //}

        //public static PersianDateTime ToPersianDateTime(this string text)
        //{
        //    return new PersianDateTime(Convert.ToDateTime(text));
        //}

        //public static DateTime ToDateTime(this PersianDateTime DT)
        //{
        //    return DT.ToDateTime();
        //}

        public static string FixDateTime(this string text)
        {
            var result = "";

            var year = "0000";

            var month = "00";

            var day = "00";

            var hour = "00";

            var min = "00";

            var sec = "00";

            var arr = text.Split(' ');

            if (arr.Length == 1)
            {
                if (arr[0].Contains('/'))
                {
                    //only date
                    var arr2 = arr[0].Split('/');

                    if (arr2.Length == 2)
                    {
                        //has year
                        if (arr2[0].Length == 2) //92
                            year = "13" + arr2[0];
                        else if (arr2[0].Length == 4) //1392
                            year = arr2[0];
                        else
                            throw new FormatException();

                        //has month
                        if (arr2[1].Length == 1) //9
                            month = "0" + arr2[1];
                        else if (arr2[1].Length == 2) //09
                            month = arr2[1];
                        else
                            throw new FormatException();
                    }
                    else if (arr2.Length == 3)
                    {
                        //has year
                        if (arr2[0].Length == 2) //92
                            year = "13" + arr2[0];
                        else if (arr2[0].Length == 4) //1392
                            year = arr2[0];
                        else
                            throw new FormatException();

                        //has month
                        if (arr2[1].Length == 1) //9
                            month = "0" + arr2[1];
                        else if (arr2[1].Length == 2) //09
                            month = arr2[1];
                        else
                            throw new FormatException();

                        //has day
                        day = arr2[2];
                        if (arr2[2].Length == 1) //5
                            day = "0" + arr2[2];
                        else if (arr2[2].Length == 2) //05
                            day = arr2[2];
                        else
                            throw new FormatException();
                    }
                    else
                    {
                        throw new FormatException();
                    }
                    result = year + "/" + month + "/" + day;
                }
                else if (arr[0].Contains(':'))
                {
                    //only time
                    var arr3 = arr[0].Split(':');

                    if (arr3.Length == 2)
                    {
                        //has hour
                        if (arr3[0].Length == 1) //2
                            hour = "0" + arr3[0];
                        else if (arr3[0].Length == 2) //02
                            hour = arr3[0];
                        else
                            throw new FormatException();

                        //has min
                        if (arr3[1].Length == 1) //8
                            min = "0" + arr3[1];
                        else if (arr3[1].Length == 2) //08
                            min = arr3[1];
                        else
                            throw new FormatException();
                    }
                    else if (arr3.Length == 3)
                    {
                        //has hour
                        if (arr3[0].Length == 1) //2
                            hour = "0" + arr3[0];
                        else if (arr3[0].Length == 2) //02
                            hour = arr3[0];
                        else
                            throw new FormatException();

                        //has min
                        if (arr3[1].Length == 1) //8
                            min = "0" + arr3[1];
                        else if (arr3[1].Length == 2) //08
                            min = arr3[1];
                        else
                            throw new FormatException();

                        //has sec
                        if (arr3[2].Length == 1) //1
                            sec = "0" + arr3[2];
                        else if (arr3[2].Length == 2) //01
                            sec = arr3[2];
                        else
                            throw new FormatException();
                    }
                    else
                    {
                        throw new FormatException();
                    }
                    result = hour + ":" + min + ":" + sec;
                }
                else
                {
                    throw new FormatException();
                }
            }
            else if (arr.Length == 2)
            {
                //has date
                var arr2 = arr[0].Split('/');

                if (arr2.Length == 2)
                {
                    //has year
                    if (arr2[0].Length == 2) //92
                        year = "13" + arr2[0];
                    else if (arr2[0].Length == 4) //1392
                        year = arr2[0];
                    else
                        throw new FormatException();

                    //has month
                    if (arr2[1].Length == 1) //9
                        month = "0" + arr2[1];
                    else if (arr2[1].Length == 2) //09
                        month = arr2[1];
                    else
                        throw new FormatException();
                }
                else if (arr2.Length == 3)
                {
                    //has year
                    if (arr2[0].Length == 2) //92
                        year = "13" + arr2[0];
                    else if (arr2[0].Length == 4) //1392
                        year = arr2[0];
                    else
                        throw new FormatException();

                    //has month
                    if (arr2[1].Length == 1) //9
                        month = "0" + arr2[1];
                    else if (arr2[1].Length == 2) //09
                        month = arr2[1];
                    else
                        throw new FormatException();

                    //has day
                    if (arr2[2].Length == 1) //5
                        day = "0" + arr2[2];
                    else if (arr2[2].Length == 2) //05
                        day = arr2[2];
                    else
                        throw new FormatException();
                }
                else
                {
                    throw new FormatException();
                }

                //has time
                var arr3 = arr[1].Split(':');

                if (arr3.Length == 2)
                {
                    //has hour
                    if (arr3[0].Length == 1) //2
                        hour = "0" + arr3[0];
                    else if (arr3[0].Length == 2) //02
                        hour = arr3[0];
                    else
                        throw new FormatException();

                    //has min
                    if (arr3[1].Length == 1) //8
                        min = "0" + arr3[1];
                    else if (arr3[1].Length == 2) //08
                        min = arr3[1];
                    else
                        throw new FormatException();
                }
                else if (arr3.Length == 3)
                {
                    //has hour
                    if (arr3[0].Length == 1) //2
                        hour = "0" + arr3[0];
                    else if (arr3[0].Length == 2) //02
                        hour = arr3[0];
                    else
                        throw new FormatException();

                    //has min
                    if (arr3[1].Length == 1) //8
                        min = "0" + arr3[1];
                    else if (arr3[1].Length == 2) //08
                        min = arr3[1];
                    else
                        throw new FormatException();

                    //has sec
                    if (arr3[2].Length == 1) //1
                        sec = "0" + arr3[2];
                    else if (arr3[2].Length == 2) //01
                        sec = arr3[2];
                    else
                        throw new FormatException();
                }
                else
                {
                    throw new FormatException();
                }
                result = year + "/" + month + "/" + day + " " + hour + ":" + min + ":" + sec;
            }
            else
            {
                throw new FormatException();
            }
            return result;
        }

        //public static DateTime ToDateTime(this string text)
        //{
        //    string[] arr = text.FixDateTime().Split(' ');

        //    if (arr.Length == 1) //date
        //        return PersianDateTime.Parse(arr[0]).ToDateTime();
        //    return PersianDateTime.Parse(arr[0], arr[1]).ToDateTime();
        //}

        public static string ToStringDateTime12(this DateTime DT)
        {
            return DT.ToString("yyyy/MM/dd hh:mm tt");
        }

        public static string ToStringDateTime24(this DateTime DT)
        {
            return DT.ToString("yyyy/MM/dd HH:mm");
        }

        public static string ToStringDate(this DateTime DT)
        {
            return DT.ToString("yyyy/MM/dd");
        }

        public static string ToStringShamsiDateTime(this DateTime DT)
        {
            return DT.ToString("d MMMM yyyy ساعت HH:mm");
        }

        public static string ToStringDateTime12P(this DateTime DT)
        {
            //return DT.ToString("yyyy/MM/dd hh:mm tt").Replace("AM", "ق.ظ").Replace("PM", "ب.ظ");
            var hh = DT.ToString("HH");

            var tt = hh.ToInt() < 12 ? "ق.ظ" : "ب.ظ";
            return DT.ToString("yyyy/M/d h:m " + tt);
        }

        //public static string ToStringDateTime12(this PersianDateTime DT)
        //{
        //    return DT.ToString("yyyy/MM/dd hh:mm tt");
        //}

        //public static string ToStringDateTime24(this PersianDateTime DT)
        //{
        //    return DT.ToString("yyyy/MM/dd HH:mm");
        //}

        //public static string ToStringDate(this PersianDateTime DT)
        //{
        //    return DT.ToString("yyyy/MM/dd");
        //}

        //public static string ToStringShamsiDate(this PersianDateTime DT)
        //{
        //    var tt = DT.Hour < 12 ? "قبل از ظهر" : "بعد از ظهر";
        //    return DT.ToString("d MMMM yyyy در h:mm " + tt);
        //}

        //public static string ToStringDateTime12P(this PersianDateTime DT)
        //{
        //    //return DT.ToString("yyyy/MM/dd hh:mm tt").Replace("AM", "ق.ظ").Replace("PM", "ب.ظ");
        //    var hh = DT.ToString("HH");

        //    var tt = hh.ToInt() < 12 ? "ق.ظ" : "ب.ظ";
        //    return DT.ToString("yyyy/M/d h:m " + tt);
        //}

        public static string GetDiffDate(this DateTime LastDate)
        {
            TimeSpan ts = DateTime.Now - LastDate;

            PersianCalendar pc = new PersianCalendar();

            int DifferenceYear = DateTime.Now.Year - LastDate.Year;

            int DiffernceMounth = DateTime.Now.Month - LastDate.Month;

            if (DateTime.Now.Month > LastDate.Month)
                DiffernceMounth = DateTime.Now.Month - LastDate.Month;
            else
                DiffernceMounth = LastDate.Month - DateTime.Now.Month;

            int DifferenceDays = ts.Days;

            double DifferenceMinute = ts.TotalMinutes;

            StringBuilder Result = new StringBuilder("");

            if (DifferenceYear > 0)
            {
                Result.Append(DifferenceYear.ToString() + " سال پیش" + " ، " + GetDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) + GetHour(LastDate));
            }
            else if (DiffernceMounth > 0)
            {
                Result.Append(DiffernceMounth.ToString() + " ماه پیش" + " ، " + GetDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) + GetHour(LastDate));
            }
            else if (DifferenceDays > 0)
                Result.Append(DifferenceDays.ToString() + " روز پیش" + " ، " + GetDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) + GetHour(LastDate));
            else if (DateTime.Now.Date == LastDate.Date && (DifferenceMinute < 60 && (DateTime.Now.Hour - LastDate.Hour <= 1)))
            {
                var diff = (int)DifferenceMinute;

                var hh = ""; //" ، " + GetHour(LastDate);

                var mm = diff == 0 ? "هم اکنون" : ConvertMinuteToString(diff) + " دقیقه قبل ";
                Result.Append(mm + hh);
            }
            else if (DifferenceDays == 0)
                Result.Append(" امروز" + " ، " + GetDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) + GetHour(LastDate));

            return Result.ToString();
        }

        public static string ConvertMinuteToString(int minute)
        {
            string Result = "";

            string[] minLessTen = { "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه", "ده" };

            string[] minLesstwenty = { "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };

            Dictionary<int, string> minmore = new Dictionary<int, string>
               {
                   {2,"بیست"},
                   {3,"سی"},
                   {4,"چهل"},
                   {5,"پنجاه"}
               };

            if (minute <= 10)
                Result = minLessTen[minute - 1];
            else if (minute < 20)
                Result = minLesstwenty[(minute % 10) - 1];
            else if ((minute / 10) >= 2)
            {
                Result = minmore[minute / 10];
                if (minute % 10 > 0)
                    Result += " و " + minLessTen[(minute % 10) - 1];
            }
            return Result;
        }

        public static string GetMounth(int month)
        {
            string[] monthInYear = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            return monthInYear[month - 1];
        }

        public static string GetHour(DateTime lastdate)
        {
            PersianCalendar pc = new PersianCalendar();

            string result = " ساعت " + (((pc.GetHour(lastdate)) < 10) ? ("0" + pc.GetHour(lastdate).ToString()) : (pc.GetHour(lastdate)).ToString()) + ":" + (((pc.GetMinute(lastdate)) < 10) ? ("0" + pc.GetMinute(lastdate).ToString()) : (pc.GetMinute(lastdate)).ToString());
            return result;
        }

        public static string GetDay(DayOfWeek day)
        {
            string Result = "";

            switch (day)
            {
                case DayOfWeek.Friday:
                    Result = "جمعه";
                    break;
                case DayOfWeek.Monday:
                    Result = "دوشنبه";
                    break;
                case DayOfWeek.Saturday:
                    Result = "شنبه";
                    break;
                case DayOfWeek.Sunday:
                    Result = "یکشنبه";
                    break;
                case DayOfWeek.Thursday:
                    Result = "پنج شنبه";
                    break;
                case DayOfWeek.Tuesday:
                    Result = "سه شنبه";

                    break;
                case DayOfWeek.Wednesday:
                    Result = "چهارشنبه";
                    break;
                default:
                    break;
            }
            return Result;
        }

        public static string ToStringShamsiDate(this DateTime dt)
        {
            var PC = new PersianCalendar();

            var intYear = PC.GetYear(dt);

            var intMonth = PC.GetMonth(dt);

            var intDayOfMonth = PC.GetDayOfMonth(dt);

            var enDayOfWeek = PC.GetDayOfWeek(dt);

            var strMonthName = "";

            switch (intMonth)
            {
                case 1:
                    strMonthName = "فروردین";
                    break;
                case 2:
                    strMonthName = "اردیبهشت";
                    break;
                case 3:
                    strMonthName = "خرداد";
                    break;
                case 4:
                    strMonthName = "تیر";
                    break;
                case 5:
                    strMonthName = "مرداد";
                    break;
                case 6:
                    strMonthName = "شهریور";
                    break;
                case 7:
                    strMonthName = "مهر";
                    break;
                case 8:
                    strMonthName = "آبان";
                    break;
                case 9:
                    strMonthName = "آذر";
                    break;
                case 10:
                    strMonthName = "دی";
                    break;
                case 11:
                    strMonthName = "بهمن";
                    break;
                case 12:
                    strMonthName = "اسفند";
                    break;
                default:
                    strMonthName = "";
                    break;
            }

            return $"{intDayOfMonth} {strMonthName} {intYear}";
        }

        public static DateTime FirstTimeOfHour(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
        }

        public static DateTime LastTimeOfHour(this DateTime dt)
        {
            return dt.FirstTimeOfHour().AddHours(1).AddTicks(-1);
        }

        public static DateTime FirstTimeOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

        public static DateTime LastTimeOfDay(this DateTime dt)
        {
            return dt.FirstTimeOfDay().AddDays(1).AddTicks(-1);
        }

        public static DateTime FirstTimeOfWeek(this DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return dt.AddDays(-6).LastTimeOfDay();
                case DayOfWeek.Monday:
                    return dt.AddDays(-2).LastTimeOfDay();
                case DayOfWeek.Saturday:
                    return LastTimeOfDay(dt);
                case DayOfWeek.Sunday:
                    return dt.AddDays(-1).LastTimeOfDay();
                case DayOfWeek.Thursday:
                    return dt.AddDays(-5).LastTimeOfDay();
                case DayOfWeek.Tuesday:
                    return dt.AddDays(-3).LastTimeOfDay();
                case DayOfWeek.Wednesday:
                    return dt.AddDays(-4).LastTimeOfDay();
                default:
                    return dt;
            }
        }

        public static DateTime LastTimeOfWeek(this DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return dt.LastTimeOfDay();
                case DayOfWeek.Monday:
                    return dt.AddDays(4).LastTimeOfDay();
                case DayOfWeek.Saturday:
                    return dt.AddDays(6).LastTimeOfDay();
                case DayOfWeek.Sunday:
                    return dt.AddDays(5).LastTimeOfDay();
                case DayOfWeek.Thursday:
                    return dt.AddDays(1).LastTimeOfDay();
                case DayOfWeek.Tuesday:
                    return dt.AddDays(3).LastTimeOfDay();
                case DayOfWeek.Wednesday:
                    return dt.AddDays(2).LastTimeOfDay();
                default:
                    return dt;
            }
        }

        public static DateTime FirstTimeOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
        }

        public static DateTime LastTimeOfMonth(this DateTime dt)
        {
            return dt.FirstTimeOfMonth().AddMonths(1).AddTicks(-1);
        }

        public static DateTime FirstTimeOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1, 0, 0, 0);
        }

        public static DateTime LastTimeOfYear(this DateTime dt)
        {
            return dt.FirstTimeOfYear().AddYears(1).AddTicks(-1);
        }

        //public static PersianDateTime FirstTimeOfHour(this PersianDateTime dt)
        //{
        //    return dt.ToDateTime().FirstTimeOfHour().ToPersianDateTime();
        //}

        //public static PersianDateTime LastTimeOfHour(this PersianDateTime dt)
        //{
        //    return dt.ToDateTime().LastTimeOfHour().ToPersianDateTime();
        //}

        //public static PersianDateTime FirstTimeOfDay(this PersianDateTime dt)
        //{
        //    return dt.ToDateTime().FirstTimeOfDay().ToPersianDateTime();
        //}

        //public static PersianDateTime LastTimeOfDay(this PersianDateTime dt)
        //{
        //    return dt.ToDateTime().LastTimeOfDay().ToPersianDateTime();
        //}

        //public static PersianDateTime FirstTimeOfWeek(this PersianDateTime dt)
        //{
        //    return dt.FirstDayOfWeek.FirstTimeOfDay();
        //}

        //public static PersianDateTime LastTimeOfWeek(this PersianDateTime dt)
        //{
        //    return dt.LastDayOfWeek.LastTimeOfDay();
        //}

        //public static PersianDateTime FirstTimeOfMonth(this PersianDateTime dt)
        //{
        //    return dt.FirstDayOfMonth.FirstTimeOfDay();
        //}

        //public static PersianDateTime LastTimeOfMonth(this PersianDateTime dt)
        //{
        //    return dt.LastDayOfMonth.LastTimeOfDay();
        //}

        //public static PersianDateTime FirstTimeOfYear(this PersianDateTime dt)
        //{
        //    return dt.FirstDayOfYear.FirstTimeOfDay();
        //}

        //public static PersianDateTime LastTimeOfYear(this PersianDateTime dt)
        //{
        //    return dt.LastDayOfYear.LastTimeOfDay();
        //}

        /// <summary>
        /// Get difference in years
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
            //this assumes you are looking for the western idea of age and not using East Asian reckoning.
            int age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;
            return age;
        }
    }
}
