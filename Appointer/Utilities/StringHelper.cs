using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Appointer.Utility
{
    public static class StringHelper
    {
        public static bool HasValue(this string value, bool ignoreWhiteSpace = false)
        {
            return !(ignoreWhiteSpace ? string.IsNullOrWhiteSpace(value) : string.IsNullOrEmpty(value));
        }

        public static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static string RemoveMaskChar(this string input)
        {
            var result = input.Replace("-", "");
            return result;
        }

        public static List<KeyValuePair<string, string>> GetParameters(string parameters)
        {
            return parameters.Split('/').Select(p =>
            {
                if (p.Contains("-"))
                {
                    var arr = p.Split('-');
                    return new KeyValuePair<string, string>(arr[0].ToLower(), arr[1]);
                }
                return new KeyValuePair<string, string>(null, p);
            }).ToList();
        }
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string GetFirstWord(this string str)
        {
            var arr = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return arr.Length >= 1 ? arr[0] : "";
        }

        public static string NullIfEmpty(this string str)
        {
            return (str == "" ? null : str);
        }

        public static string EmptyIfNull(this string str)
        {
            return (str == null ? "" : str);
        }

        /// <summary>
        /// Checks string object's value to array of string values
        /// </summary>       
        /// <param name="stringValues">Array of string values to compare</param>
        /// <returns>Return true if any string value matches</returns>
        public static bool In(this string str, params string[] stringValues)
        {
            foreach (var item in stringValues)
                if (string.Compare(str, item) == 0)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns characters from right of specified length
        /// </summary>
        /// <param name="str">String value</param>
        /// <param name="length">Max number of charaters to return</param>
        /// <returns>Returns string from right</returns>
        public static string Right(this string str, int length)
        {
            return str != null && str.Length > length ? str.Substring(str.Length - length) : str;
        }

        /// <summary>
        /// Returns characters from left of specified length
        /// </summary>
        /// <param name="str">String value</param>
        /// <param name="length">Max number of charaters to return</param>
        /// <returns>Returns string from left</returns>
        public static string Left(this string str, int length)
        {
            return str != null && str.Length > length ? str.Substring(0, length) : str;
        }

        public static string RemoveLeft(this string str, int length)
        {
            return str.Remove(0, length);
        }

        public static string RemoveRight(this string str, int length)
        {
            return str.Remove(str.Length - length);
        }

        /// <summary>
        ///  Replaces the format item in a specified System.String with the text equivalent
        ///  of the value of a specified System.Object instance.
        /// </summary>
        /// <param name="str">A composite format string</param>
        /// <param name="args">An System.Object array containing zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the System.String
        /// equivalent of the corresponding instances of System.Object in args.</returns>
        public static string Format(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static string Join(this IEnumerable<string> arr, string separator)
        {
            return string.Join(separator, arr);
        }

        public static string DeleteChars(this string str, params char[] chars)
        {
            return new string(str.Where(ch => !chars.Contains(ch)).ToArray());
        }

        public static string DeleteStrs(this string str, params string[] strs)
        {
            foreach (var item in strs)
                str = str.Replace(item, "");
            return str;
        }

        public static string ToLowerFirst(this string str)
        {
            char[] a = str.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
            //return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public static string ToUpperFirst(this string str)
        {
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
            //return str.Substring(0, 1).ToUpper() + (str.Length > 1 ? str.Substring(1) : "");
        }

        /// <summary>
        /// Returns the first string with a non-empty non-null value.
        /// </summary>
        public static string Or(this string str, string alternative)
        {
            return (!string.IsNullOrEmpty(str)) ? str : alternative;
        }

        public static string En2Fa(this string str)
        {
            return
                str.Replace("0", "۰")
                    .Replace("1", "۱")
                    .Replace("2", "۲")
                    .Replace("3", "۳")
                    .Replace("4", "۴")
                    .Replace("5", "۵")
                    .Replace("6", "۶")
                    .Replace("7", "۷")
                    .Replace("8", "۸")
                    .Replace("9", "۹");
        }

        public static string Fa2En(this string str)
        {
            return str
                    .Replace("۰", "0")
                    .Replace("۱", "1")
                    .Replace("۲", "2")
                    .Replace("۳", "3")
                    .Replace("۴", "4")
                    .Replace("۵", "5")
                    .Replace("۶", "6")
                    .Replace("۷", "7")
                    .Replace("۸", "8")
                    .Replace("۹", "9")

                    .Replace("٠", "0")
                    .Replace("١", "1")
                    .Replace("٢", "2")
                    .Replace("٣", "3")
                    .Replace("٤", "4")
                    .Replace("٥", "5")
                    .Replace("٦", "6")
                    .Replace("٧", "7")
                    .Replace("٨", "8")
                    .Replace("٩", "9");
        }

        public static string FixPersianChars(this string str)
        {
            var result = str.Replace('ﮎ', 'ک').Replace('ﮏ', 'ک').Replace('ﮐ', 'ک').Replace('ﮑ', 'ک').Replace('ك', 'ک'); // تصحیح ک
            result = result.Replace('ٱ', 'ا').Replace('ٵ', 'ا'); // تصحیح الف
            result = result.Replace('ي', 'ی').Replace('ئ', 'ی'); // تصحیح ی
            result = result.Replace('ة', 'ه'); // تصحیح ه
            result = result.Replace(' ', ' ');
            result = result.Replace(" می ", " می‌").Replace(" ی ", "‌ی ").Replace(" ای ", "‌ای ").Replace(" ها ", "‌ها ").Replace(" های ", "‌های ").Replace(" تر ", "‌تر ").Replace(" ترین ", "‌ترین "); // تصحیح نیم‌فاصله
            result = result.Replace(" . ", ". ").Replace(" .", ". "); // تصحیح نقطه
            result = result.Replace(" ، ", "، ").Replace(" ،", "، "); // تصحیح کاما

            return result;
        }

        public static string CleanString(this string str)
        {
            return str.Trim().FixPersianChars().RemoveBlankSpaces().Fa2En().NullIfEmpty();
        }

        public static string RemoveBlankSpaces(this string text)
        {
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }
            return text;
        }

        public static string RemoveLetters(this string text)
        {
            if (text.IsNotNull())
            {
                return Regex.Replace(text, "[^0-9.]", "");

            }
            return null;
        }

        public static string RemoveStr(this string text, params string[] strs)
        {
            strs.ForEach(p => text = text.Replace(p, ""));
            return text;
        }

        public static string ToTitleCase(string str)
        {
            return CultureHelper.GetCultureInfo("en-US").TextInfo.ToTitleCase(str.ToLower());
        }

        private static List<KeyValuePair<char, char>> _characters = new List<KeyValuePair<char, char>>
        {
            new KeyValuePair<char, char>('q', 'ض'),
            new KeyValuePair<char, char>('w', 'ص'), //«»:ةيژؤBإأء
            new KeyValuePair<char, char>('e', 'ث'),
            new KeyValuePair<char, char>('r', 'ق'),
            new KeyValuePair<char, char>('t', 'ف'),
            new KeyValuePair<char, char>('T', '،'), //***
            new KeyValuePair<char, char>('y', 'غ'),
            new KeyValuePair<char, char>('Y', '؛'), //***
            new KeyValuePair<char, char>('u', 'ع'),
            new KeyValuePair<char, char>('i', 'ه'),
            new KeyValuePair<char, char>('o', 'خ'),
            new KeyValuePair<char, char>('p', 'ح'),
            new KeyValuePair<char, char>('[', 'ج'),
            new KeyValuePair<char, char>(']', 'چ'),
            new KeyValuePair<char, char>('\\', 'پ'),
            new KeyValuePair<char, char>('a', 'ش'),
            new KeyValuePair<char, char>('s', 'س'),
            new KeyValuePair<char, char>('d', 'ی'),
            new KeyValuePair<char, char>('f', 'ب'),
            new KeyValuePair<char, char>('g', 'ل'),
            new KeyValuePair<char, char>('G', 'ۀ'), //***
            new KeyValuePair<char, char>('h', 'ا'),
            new KeyValuePair<char, char>('H', 'آ'), //***
            new KeyValuePair<char, char>('j', 'ت'),
            new KeyValuePair<char, char>('J', 'ـ'), //***
            new KeyValuePair<char, char>('k', 'ن'),
            new KeyValuePair<char, char>('K', '«'), //***
            new KeyValuePair<char, char>('l', 'م'),
            new KeyValuePair<char, char>('L', '»'), //***
            new KeyValuePair<char, char>(';', 'ک'),
            new KeyValuePair<char, char>('\'', 'گ'),
            new KeyValuePair<char, char>('z', 'ظ'),
            new KeyValuePair<char, char>('x', 'ط'),
            new KeyValuePair<char, char>('c', 'ز'),
            new KeyValuePair<char, char>('C', 'ژ'), //***
            new KeyValuePair<char, char>('v', 'ر'),
            new KeyValuePair<char, char>('V', 'ؤ'), //***
            new KeyValuePair<char, char>('b', 'ذ'),
            new KeyValuePair<char, char>('B', 'إ'), //***
            new KeyValuePair<char, char>('n', 'د'),
            new KeyValuePair<char, char>('N', 'أ'), //***
            new KeyValuePair<char, char>('m', 'ئ'),
            new KeyValuePair<char, char>('M', 'ء'), //***
            new KeyValuePair<char, char>(',', 'و')
        };

        public static string IncorrectEnglishToFarsi(string text)
        {
            if (text == null)
                return null;

            var result = "";
            foreach (var item in text)
            {
                var aaa = _characters.Where(p => p.Key == item).Select(p => new { p.Value }).SingleOrDefault();
                if (aaa == null)
                {
                    var lower = item.ToString().ToLower()[0];
                    aaa = _characters.Where(p => p.Key == lower).Select(p => new { p.Value }).SingleOrDefault();
                }
                var ch = aaa?.Value ?? item;
                result += ch;
            }
            return result;
        }

        public static string FarsiToIncorrectEnglish(string text)
        {
            if (text == null)
                return null;

            var result = "";
            foreach (var item in text)
            {
                var aaa = _characters.Where(p => p.Value == item).Select(p => new { p.Key }).SingleOrDefault();
                var ch = aaa?.Key ?? item;
                result += ch;
            }
            return result;
        }
    }
}