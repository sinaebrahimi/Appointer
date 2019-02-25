using Appointer.Models;
using Appointer.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Image = System.Drawing.Image;

namespace Appointer
{
    public static class CommonUtility
    {
        public static void CheckArgumentNull(this object obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
        }
        public static IEnumerable<Type> GetTypesAssignableFrom<TType>() where TType : Type
        {
            var type = typeof(TType);
            return type.Assembly.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();
        }
        public static bool IsEnumerable(this Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
            //type.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }
        public static bool IsEnumerable<T>(this Type type)
        {
            return type != typeof(string) && typeof(IEnumerable<T>).IsAssignableFrom(type) && type.IsGenericType;
        }
        public static bool IsUserDefinedClass(this Type type)
        {
            return type.Assembly.GetName().Name != "mscorlib";
        }


        /// <summary>
        /// Get unproxied entity type
        /// </summary>
        /// <remarks> If your Entity Framework context is proxy-enabled, 
        /// the runtime will create a proxy instance of your entities, 
        /// i.e. a dynamically generated class which inherits from your entity class 
        /// and overrides its virtual properties by inserting specific code useful for example 
        /// for tracking changes and lazy loading.
        /// </remarks>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Type GetUnproxiedEntityType(this BaseEntity entity)
        {
            var userType = ObjectContext.GetObjectType(entity.GetType());
            return userType;
        }

        public static PropertyInfo GetPropInfo<T, TPropType>(this Expression<Func<T, TPropType>> keySelector)
        {
            //var propInfo = (keySelector.Body as MemberExpression)?.Member as PropertyInfo;

            var member = keySelector.Body as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;

            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");
            }
            return propInfo;
        }

        public static T GetObject<T>(this IEnumerable<T> list)
        {
            return Activator.CreateInstance<T>();
        }

        public static void LogError(this Exception ex, HttpContext context = null)
        {
            var c = context ?? HttpContext.Current;
            //ErrorSignal.FromCurrentContext().Raise(ex);
            //try
            //{
            //    if (ex is DbEntityValidationException)
            //    {
            //        var text = string.Join(" - ", ex.GetValidationErrors());
            //        ErrorSignal.FromCurrentContext().Raise(new Exception(text, ex));
            //    }
            //    else
            //    {
            //        ErrorSignal.FromCurrentContext().Raise(ex);
            //    }
            //}
            //catch { }
            //return "برنامه با خطا مواجه شده است!";
        }

        public class FileNameResult
        {
            public string AbsolutePathForSave { get; set; }
            public string RelativePathForWeb { get; set; }
        }

        public static FileNameResult RenameFile(string fileName, string newName)
        {
            fileName = fileName.Trim().Replace('/', '\\').Trim('\\');
            var isPathRooted = Path.IsPathRooted(fileName);
            if (isPathRooted == false)
                fileName = HttpContext.Current.Server.MapPath("\\" + fileName);

            var dir = Path.GetDirectoryName(fileName);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(newName).CleanString();
            var trimedFileName = TrimFileForSave(fileNameWithoutExt);
            var ext = Path.GetExtension(fileName);
            var index = 2;
            if (File.Exists(dir + "\\" + trimedFileName + ext))
            {
                while (File.Exists(dir + "\\" + trimedFileName + " ({0})".Format(index) + ext))
                {
                    index++;
                }
                trimedFileName += " ({0})".Format(index);
            }
            var absolutePath = dir + "\\" + trimedFileName + ext;
            var root = HttpContext.Current.Server.MapPath("~").TrimEnd('\\');

            File.Move(fileName, absolutePath);

            return new FileNameResult
            {
                AbsolutePathForSave = absolutePath,
                RelativePathForWeb = absolutePath.Replace(root, "").Replace("\\", "/")
            };
        }

        public static FileNameResult GetFileName(this string fileName, string path, string format = "yyyy/MM/dd", string name = null)
        {
            var root = HttpContext.Current.Server.MapPath("~").TrimEnd('\\');
            var path2 = path.Replace("/", "\\").Trim('\\');

            string dir;
            if (string.IsNullOrEmpty(format) == false)
            {
                var time = DateTime.Now.ToPersianDateTime().ToString(format).Replace("/", "\\").TrimEnd('\\');
                FolderPath(path2 + "\\" + time);
                dir = root + "\\" + path2 + "\\" + time;
                //Directory.CreateDirectory(dir);
            }
            else
            {
                FolderPath(path2);
                dir = root + "\\" + path2;
            }

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(name ?? fileName).CleanString();
            var trimedFileName = TrimFileForSave(fileNameWithoutExt);
            var ext = Path.GetExtension(fileName);

            var index = 2;

            if (File.Exists(dir + "\\" + trimedFileName + ext))
            {
                while (File.Exists(dir + "\\" + trimedFileName + string.Format(" ({0})", index) + ext))
                {
                    index++;
                }
                trimedFileName += string.Format(" ({0})", index);
            }

            var absolutePath = dir + "\\" + trimedFileName + ext;
            return new FileNameResult
            {
                AbsolutePathForSave = absolutePath,
                RelativePathForWeb = absolutePath.Replace(root, "").Replace("\\", "/")
            };
        }

        public static FileNameResult GetFileName(this HttpPostedFileBase file, string path, string format = "yyyy/MM/dd")
        {
            var root = HttpContext.Current.Server.MapPath("~").TrimEnd('\\');
            var path2 = path.Replace("/", "\\").Trim('\\');

            string dir;
            if (string.IsNullOrEmpty(format) == false)
            {
                var time = DateTime.Now.ToPersianDateTime().ToString(format).Replace("/", "\\").TrimEnd('\\');
                FolderPath(path2 + "\\" + time);
                dir = root + "\\" + path2 + "\\" + time;
                Directory.CreateDirectory(dir);
            }
            else
            {
                FolderPath(path2);
                dir = root + "\\" + path2;
            }

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName).CleanString();
            var trimedFileName = TrimFileForSave(fileNameWithoutExt);
            var ext = Path.GetExtension(file.FileName);

            var index = 2;
            if (File.Exists(dir + "\\" + trimedFileName + ext))
            {
                while (File.Exists(dir + "\\" + trimedFileName + " ({0})".Format(index) + ext))
                {
                    index++;
                }
                trimedFileName += " ({0})".Format(index);
            }

            var absolutePath = dir + "\\" + trimedFileName + ext;
            return new FileNameResult
            {
                AbsolutePathForSave = absolutePath,
                RelativePathForWeb = absolutePath.Replace(root, "").Replace("\\", "/")
            };
        }

        public static void CreateThumbnail(this HttpPostedFileBase file, string inputPath, string outputPath)
        {
            var width = 125;
            var height = 125;

            Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            var inputImage = new Bitmap(inputPath);

            var outputImage = inputImage.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
            outputImage.Save(outputPath, ImageFormat.Jpeg);
        }

        private static bool ThumbnailCallback()
        {
            return false;
        }

        #region MergeAttributes
        private static object GetValue(IGrouping<string, KeyValuePair<string, object>> source, bool appendCssClass)
        {
            if (appendCssClass)
                return source.Key == "class" ? string.Join(" ", source.Select(p => p.Value)) : source.First().Value;
            return source.First().Value;
        }

        public static Dictionary<string, object> MergeAttributes(object primaryAttributes, Dictionary<string, object> secondaryAttributes, bool appendCssClass = true) //not replace css class
        {
            if (primaryAttributes is Dictionary<string, object> primary)
                return MergeAttributes(primary, secondaryAttributes, appendCssClass);

            return new RouteValueDictionary(primaryAttributes).Concat(secondaryAttributes).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => GetValue(d, appendCssClass));
        }

        public static Dictionary<string, object> MergeAttributes(object primaryAttributes, object secondaryAttributes, bool appendCssClass = true) //not replace css class
        {
            var primary = primaryAttributes as Dictionary<string, object>;
            var secondary = secondaryAttributes as Dictionary<string, object>;

            if (primary != null && secondary != null)
                return MergeAttributes(primary, secondary, appendCssClass);
            if (primary != null)
                return MergeAttributes(primary, secondaryAttributes, appendCssClass);
            if (secondary != null)
                return MergeAttributes(primaryAttributes, secondary, appendCssClass);

            var attributes = new RouteValueDictionary(primaryAttributes).Concat(new RouteValueDictionary(secondaryAttributes)).GroupBy(d => d.Key)
                .ToDictionary(d => d.Key.Replace('_', '-'), d => GetValue(d, appendCssClass));
            return attributes;
        }

        public static Dictionary<string, object> MergeAttributes(this Dictionary<string, object> primaryAttributes, Dictionary<string, object> secondaryAttributes, bool appendCssClass = true) //not replace css class
        {
            return primaryAttributes.Concat(secondaryAttributes).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => GetValue(d, appendCssClass));
        }

        public static Dictionary<string, object> MergeAttributes(this Dictionary<string, object> primaryAttributes, object secondaryAttributes, bool appendCssClass = true) //not replace css class
        {
            if (secondaryAttributes is Dictionary<string, object> secondary)
                return MergeAttributes(primaryAttributes, secondary, appendCssClass);

            return primaryAttributes.Concat(new RouteValueDictionary(secondaryAttributes)).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => GetValue(d, appendCssClass));
        }
        #endregion

        public static string FormatFileName(string filename, string format = "{0}\\{1}.{2}") //  GetDirectoryName\FileName.Extension
        {
            return string.Format(format, Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename), Path.GetExtension(filename).TrimStart('.'));
        }

        public static string ToStaticUrl(this string url, string subdomain = "", string prefix = "", string cdnOverwrite = "")
        {
            if (string.IsNullOrEmpty(url))
                return "";

            url = "/" + url.TrimStart('~').TrimStart('/');
            var urlEncoded = VirtualPathUtility.ToAbsolute(url).UrlPathEncode().TrimStart('/');

            cdnOverwrite = cdnOverwrite.TrimEnd('/');
            prefix = prefix.Trim('/');

            if (cdnOverwrite != "")
                return $"{cdnOverwrite}/{prefix}/{urlEncoded}";

            var baseUrl = HttpContext.Current.GetBaseUrl(true);
            if (subdomain != "")
                baseUrl = baseUrl.Insert(baseUrl.IndexOf("://") + 3, subdomain.TrimEnd('.') + ".");

            return $"{baseUrl}/{prefix}/{urlEncoded}";
        }

        public static string StripHtml(this string inputString)
        {
            foreach (Match m in Regex.Matches(inputString, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                inputString = inputString.Replace(m.Groups[1].Value, m.Groups[1].Value.ToStaticUrl());
            }
            return inputString;
        }

        public static string TrimFileForSave(string filename)
        {
            var arr = filename.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var result = arr.Join("-")
                .ToLower()
                //.Replace("/", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("!", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("*", "")
                .Replace("?", "")
                .Replace("\"", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "")
                .Replace("%", "")
                .Replace("~", "")
                .Replace("!", "")
                .Replace("@", "")
                .Replace("#", "")
                .Replace("$", "")
                .Replace("^", "")
                .Replace("&", "")
                .Replace("+", "")
                .Replace("=", "")
                .Replace(",", "")
                .Replace(";", "")
                .Replace("`", "")
                .Replace("'", "");
            return result;
        }

        public static string GetLink(string text, string url)
        {
            return "<a href=\"" + HttpContext.Current.GetBaseUrl() + url.TrimStart('/') + "\">" + text + "</a>";
        }

        public static string GetVirtualPath(this UrlHelper url, string absolutePath)
        {
            return url.Content(absolutePath.Replace(url.RequestContext.HttpContext.Request.PhysicalApplicationPath, "~/"));
        }

        public static string FolderPath(this string folderNames)
        {
            return FolderPath(folderNames.Trim('\\').Split('\\'));
        }

        public static string FolderPath(params string[] folderNames)
        {
            var path = HttpContext.Current.Server.MapPath("~").TrimEnd('\\');

            foreach (var folderName in folderNames)
            {
                path += "\\" + folderName;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string ToUrl(this object picName, string noPicture, params string[] folderNames)
        {
            var path = "../";

            foreach (var folderName in folderNames)
            {
                path += folderName + "/";
            }
            path += (picName == null ? noPicture : picName.ToString());
            return path;
        }

        public static bool DeleteFile(this string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";

            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomDigit(int length)
        {
            const string chars = "0123456789";

            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        public static string FixUrl(this string url)
        {
            var encoded = url.EmptyIfNull()
                .RemoveStr("%", ".", ":", "~", "/", "!", "@", "#", "$", "^", "&", "*", "(", ")", "'", "+", "=", "[", "]", "{", "}", ",", ";", "/", "|", "`", "<", ">", "\"", "\\");
            //    .Replace("%", "%".UrlEncode())
            //    .Replace(".", ".".UrlEncode())
            //    .Replace(":", ":".UrlEncode())
            //    .Replace("~", "~".UrlEncode())
            //    .Replace("/", "/".UrlEncode())
            //    .Replace("!", "!".UrlEncode())
            //    .Replace("@", "@".UrlEncode())
            //    .Replace("#", "#".UrlEncode())
            //    .Replace("$", "$".UrlEncode())
            //    .Replace("^", "^".UrlEncode())
            //    .Replace("&", "&".UrlEncode())
            //    .Replace("*", "*".UrlEncode())
            //    .Replace("(", "(".UrlEncode())
            //    .Replace(")", ")".UrlEncode())
            //    .Replace("'", "'".UrlEncode())
            //    .Replace("+", "+".UrlEncode())
            //    .Replace("=", "=".UrlEncode())
            //    .Replace("[", "[".UrlEncode())
            //    .Replace("]", "]".UrlEncode())
            //    .Replace("{", "{".UrlEncode())
            //    .Replace("}", "}".UrlEncode())
            //    .Replace(",", ",".UrlEncode())
            //    .Replace(";", ";".UrlEncode())
            //    .Replace("/", "/".UrlEncode())
            //    .Replace("|", "|".UrlEncode())
            //    .Replace("`", "`".UrlEncode())
            //    .Replace("<", "<".UrlEncode())
            //    .Replace(">", ">".UrlEncode())
            //    .Replace("\"", "\"".UrlEncode())
            //    .Replace("\\", "\\".UrlEncode());

            var arr = encoded.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            return arr.Join("-");
        }


        public static string SuggestUrl(this string url)
        {
            var str = url
                .Replace("%", "-")
                .Replace(".", "-")
                .Replace(":", "-")
                .Replace("~", "-")
                .Replace("/", "-")
                .Replace("!", "-")
                .Replace("@", "-")
                .Replace("#", "-")
                .Replace("$", "-")
                .Replace("^", "-")
                .Replace("&", "-")
                .Replace("*", "-")
                .Replace("(", "-")
                .Replace(")", "-")
                .Replace("'", "-")
                .Replace("+", "-")
                .Replace("=", "-")
                .Replace("[", "-")
                .Replace("]", "-")
                .Replace("{", "-")
                .Replace("}", "-")
                .Replace(",", "-")
                .Replace(";", "-")
                .Replace("`", "-")
                .Replace("<", "-")
                .Replace(">", "-")
                .Replace("|", "-")
                .Replace("/", "-")
                .Replace("\\", "-")
                .Replace("\"", "-")
                .Replace("-", " ");

            var arr = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            return arr.Join("-").Left(60);
        }

        public static string SuggestUrlForFileName(this string url)
        {
            var str = url
                .Replace("%", "-")
                //.Replace(".", "-")
                .Replace(":", "-")
                .Replace("~", "-")
                .Replace("/", "-")
                .Replace("!", "-")
                .Replace("@", "-")
                .Replace("#", "-")
                .Replace("$", "-")
                .Replace("^", "-")
                .Replace("&", "-")
                .Replace("*", "-")
                //.Replace("(", "-")
                //.Replace(")", "-")
                .Replace("'", "-")
                .Replace("+", "-")
                .Replace("=", "-")
                .Replace("[", "-")
                .Replace("]", "-")
                .Replace("{", "-")
                .Replace("}", "-")
                .Replace(",", "-")
                .Replace(";", "-")
                .Replace("`", "-")
                .Replace("<", "-")
                .Replace(">", "-")
                .Replace("|", "-")
                .Replace("/", "-")
                .Replace("\\", "-")
                .Replace("\"", "-")
                .Replace("-", " ");

            var arr = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            return arr.Join("-").Left(60);
        }

        public static string BreadCrumbs(params object[] items)
        {
            var lst = new List<string>();

            foreach (var item in items)
            {
                var li = item.ToString();

                if (li != "")
                    lst.Add(li);
            }
            return string.Join(" > ", lst);
        }

        public static Uri ToTiny(this Uri longUri)
        {
            var url = longUri.ToString().UrlEncode();
            var request = WebRequest.Create($"http://tinyurl.com/api-create.php?url={url}");

            var response = request.GetResponse();

            Uri returnUri = null;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                returnUri = new Uri(reader.ReadToEnd());
            }
            return returnUri;
        }

        public static bool Not(this bool b)
        {
            return !b;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static T SingleOrDefaultValue<T>(this IEnumerable<T> entities) where T : new()
        {
            var entity = entities.SingleOrDefault();
            if (entity == null)
                return new T(); //default(T);
            return entity;
        }

        public static T FirstOrDefaultValue<T>(this IEnumerable<T> entities) where T : new()
        {
            var entity = entities.FirstOrDefault();
            if (entity == null)
                return new T(); //default(T);
            return entity;
        }

        public static T If<T>(this bool predicate, T trueValue, T falseValue)
        {
            if (predicate)
                return trueValue;
            return falseValue;
        }

        public static T NewDefaultValue<T>(this T enitty) where T : class
        {
            enitty.GetType().GetProperties().ToList().ForEach(p =>
            {
                if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                    p.SetValue(enitty, false, null);
                if (p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?))
                    p.SetValue(enitty, Convert.ToByte(0), null);
                if (p.PropertyType == typeof(Int16) || p.PropertyType == typeof(Int16?))
                    p.SetValue(enitty, Convert.ToInt16(0), null);
                if (p.PropertyType == typeof(Int32) || p.PropertyType == typeof(Int32?))
                    p.SetValue(enitty, Convert.ToInt32(0), null);
                if (p.PropertyType == typeof(Int64) || p.PropertyType == typeof(Int64?))
                    p.SetValue(enitty, Convert.ToInt64(0), null);
                if (p.PropertyType == typeof(Decimal) || p.PropertyType == typeof(Decimal?))
                    p.SetValue(enitty, Convert.ToDecimal(0), null);
                if (p.PropertyType == typeof(float) || p.PropertyType == typeof(float?))
                    p.SetValue(enitty, float.Parse("0"), null);
                if (p.PropertyType == typeof(Double) || p.PropertyType == typeof(Double?))
                    p.SetValue(enitty, Convert.ToDouble(0), null);
                if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                    p.SetValue(enitty, new DateTime(), null);
                if (p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?))
                    p.SetValue(enitty, new DateTimeOffset(), null);
                if (p.PropertyType == typeof(string))
                    p.SetValue(enitty, "a", null);
                if (p.PropertyType == typeof(byte[]))
                    p.SetValue(enitty, new byte[] { }, null);
            });
            return enitty;
        }

        public static string ReplaceByFirstMatch(string pattern, string input, string output)
        {
            var regex = new Regex(pattern);
            if (regex.IsMatch(input))
                return string.Format(output, regex.Match(input).Value);
            return input;
        }

        /// <summary>
        /// Compare two arrasy
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="a1">Array 1</param>
        /// <param name="a2">Array 2</param>
        /// <returns>Result</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            return o == null ? failureValue : evaluator(o);
        }

        public static Bundle AddStyleBundle(this Bundle bundle, params string[] paths)
        {
            foreach (var path in paths)
            {
                bundle = bundle.Include(path, new CssRewriteUrlTransform());
            }
            return bundle;
        }
    }
}