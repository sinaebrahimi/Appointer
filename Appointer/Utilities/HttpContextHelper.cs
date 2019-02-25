using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Appointer.Utility
{
    public static class HttpContextHelper
    {
        public static bool DownloadFile(string url, string path)
        {
            Stream stream = null;
            bool success;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                var response = req.GetResponse();
                stream = response.GetResponseStream();
                if (response.Headers.AllKeys.Contains("Content-Encoding")
                    && response.Headers["Content-Encoding"].Contains("gzip"))
                {
                    stream = new GZipStream(stream, CompressionMode.Decompress);
                }
                using (var fileStream = File.Create(path))
                {
                    stream.CopyTo(fileStream);
                }
                success = true;
            }
            catch
            {
                success = false;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
            return success;
        }

        public static string IpAddress(HttpContextBase context = null)
        {
            var ip = context?.Request.ServerVariables["REMOTE_ADDR"] ?? context?.Request.UserHostAddress ??
                HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? HttpContext.Current.Request.UserHostAddress;

            if (ip == "localhost" || ip == "::1")
                ip = "127.0.0.1";
            return ip;
        }

        public static string HttpForwarded(HttpContextBase context = null)
        {
            var ip = context?.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ip == "localhost" || ip == "::1")
                ip = "127.0.0.1";
            return ip;
        }

        public static string UrlReferrer()
        {
            var reuest = HttpContext.Current.Request;
            return reuest.UrlReferrer == null ? null : reuest.UrlReferrer.AbsoluteUri;
        }

        public static string GetUrl(string path)
        {
            var newPath = path;
            if (string.IsNullOrEmpty(newPath) || newPath.Trim() == "/")
                newPath = Path.Combine("/", GetCurrentNeutralCulture());
            return newPath;
        }

        public static string GetCurrentNeutralCulture()
        {
            string name = Thread.CurrentThread.CurrentCulture.Name;

            if (!name.Contains("-")) return name;

            return name.Split('-')[0];
        }

        public static string GetCurrentPageUrl(this HttpRequest request)
        {
            return request.Url.AbsoluteUri.Replace(request.Url.PathAndQuery, "") + request.RawUrl;
        }

        public static HttpContextBase HttpContextBase(this HttpContext context)
        {
            return new HttpContextWrapper(context) as HttpContextBase;
        }

        public static string GetBaseUrl(this HttpContext context, bool excludeSubdomain = false)
        {
            return context.HttpContextBase().GetBaseUrl(excludeSubdomain);
        }

        public static string GetBaseUrl(this HttpContextBase context, bool excludeSubdomain = false)
        {
            var scheme = "http";
            var host = "Appointer.com";
            var port = "";
            if (context?.Request?.Url != null)
            {
                var uri = context.Request.Url;
                port = uri.Port != 80 ? (":" + uri.Port) : "";
                scheme = uri.Scheme;
                host = excludeSubdomain ? GetHostWithoutSubdomain(uri.Host) : uri.Host;
            }
            return $"{scheme}://{host}{port}";
        }

        public static string GetHostWithoutSubdomain(string host)
        {
            return string.Join(".", host.Split('.').Reverse().Take(2).Reverse());
        }

        public static string HtmlEncode(this string data)
        {
            return HttpUtility.HtmlEncode(data);
        }

        public static string HtmlDecode(this string data)
        {
            return HttpUtility.HtmlDecode(data);
        }

        public static NameValueCollection ParseQueryString(this string query)
        {
            return HttpUtility.ParseQueryString(query);
        }

        public static List<NameValueObject> ToList(this NameValueCollection collection)
        {
            var lst = new List<NameValueObject>();

            for (var i = 0; i < collection.Count; i++)
            {
                lst.Add(new NameValueObject(collection.Keys[i], collection[i]));
            }
            return lst;
        }

        public static IEnumerable<HttpCookie> Where(this HttpCookieCollection collection,
            Func<HttpCookie, bool> predicate)
        {
            var lst = new List<HttpCookie>();

            for (var i = 0; i < collection.Count; i++)
            {
                if (predicate(collection[i]))
                    lst.Add(collection[i]);
            }
            return lst.AsEnumerable();
        }

        public class NameValueObject
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public NameValueObject(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }

        public static string UrlEncode(this string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public static string UrlDecode(this string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        public static string UrlPathEncode(this string url)
        {
            return HttpUtility.UrlPathEncode(url);
        }

        public static string MapPath(string path)
        {
            return HostingEnvironment.MapPath(path);
        }

        public static void DownloadToClient(this string fileName, HttpResponse response, bool inline = false)
        {
            response.Clear();
            response.ClearHeaders();
            response.ClearContent();
            response.BufferOutput = false;  // for large files
            response.ContentType = Path.GetExtension(fileName).TrimStart('.').GetMimeType();
            response.AddHeader("Content-Disposition", inline ? "inline;" : "attachment;" + "filename=" + Path.GetFileName(fileName));
            response.AddHeader("Content-Length", new FileInfo(fileName).Length.ToString());
            //response.BinaryWrite(File.ReadAllBytes(filepath));
            //response.WriteFile(filepath);
            response.TransmitFile(fileName);
            response.Flush();
            response.End();
            response.Close();
        }

        public static void DownloadToClient(this Image img, HttpResponse response, ImageFormat format, string fileName = null)
        {
            response.Clear();
            response.ClearHeaders();
            response.ClearContent();
            response.BufferOutput = false;  // for large files
            response.ContentType = format.ToString().GetMimeType();
            response.AddHeader("Content-Disposition", "inline;" + (string.IsNullOrEmpty(fileName) ? "" : "filename=" + fileName));
            response.BinaryWrite(img.ToBytes(format));
            response.Flush();
            response.End();
            response.Close();
        }

        public static bool IsEmbeddedIntoAnotherDomain(this HttpRequestBase Request)
        {
            return Request.UrlReferrer != null && !Request.Url.Host.Equals(Request.UrlReferrer.Host, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
