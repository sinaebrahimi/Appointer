using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appointer.Utility
{
    public static class Http
    {
        public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";
        public static string accept = "text/html,application/xhtml+xml,application/xml";

        #region Sync Methods
        public static string Post(string url, NameValueCollection values)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept", accept);
                client.Headers.Add("User-Agent", userAgent);

                client.Encoding = Encoding.UTF8;
                var response = client.UploadValues(url, values);

                var responseString = Encoding.UTF8.GetString(response);
                return responseString;
            }
        }

        public static string Get(string url)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept", accept);
                client.Headers.Add("User-Agent", userAgent);
                //client.Headers.Add("Charset", "UTF-8");
                //client.Headers.Add("Accept-Encoding", "gzip, deflate");
                //client.Headers.Add("Accept-Charset", "ISO-8859-1");
                //client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                //client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                client.Encoding = Encoding.UTF8;
                var responseString = client.DownloadString(url);
                return responseString;
            }
        }

        /// <summary>
        /// Sending POST request.
        /// </summary>
        /// <param name="Url">Request Url.</param>
        /// <param name="Data">Data for request.</param>
        /// <returns>Response body.</returns>
        public static string Post(string Url, string Data)
        {
            string Out = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(Url);
            try
            {
                req.Method = "POST";
                req.Timeout = 100000;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = userAgent;
                //req.Accept = accept;

                byte[] sentData = Encoding.UTF8.GetBytes(Data);
                req.ContentLength = sentData.Length;
                using (var sendStream = req.GetRequestStream())
                {
                    sendStream.Write(sentData, 0, sentData.Length);
                    sendStream.Close();
                }
                var res = req.GetResponse();
                var ReceiveStream = res.GetResponseStream();
                Out = new StreamReader(ReceiveStream, Encoding.UTF8).ReadToEnd();
                //using (StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8))
                //{
                //    Char[] read = new Char[256];
                //    int count = sr.Read(read, 0, 256);
                //    while (count > 0)
                //    {
                //        String str = new String(read, 0, count);
                //        Out += str;
                //        count = sr.Read(read, 0, 256);
                //    }
                //}
            }
            catch (ArgumentException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {ex.Message}";
            }
            catch (WebException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: WebException raised! :: {ex.Message}";
            }
            catch (Exception ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: Exception raised! :: {ex.Message}";
            }

            return Out;
        }

        /// <summary>
        /// Sending GET request.
        /// </summary>
        /// <param name="Url">Request Url.</param>
        /// <param name="Data">Data for request.</param>
        /// <returns>Response body.</returns>
        public static string Get(string Url, string Data)
        {
            string Out = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(Url + (string.IsNullOrEmpty(Data) ? "" : "?" + Data));
            try
            {
                req.UserAgent = userAgent;
                //req.Accept = accept;
                req.AllowAutoRedirect = true;
                //req.MaximumAutomaticRedirections = 1;

                var resp = req.GetResponse();
                using (var stream = resp.GetResponseStream())
                {
                    Out = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                }
            }
            catch (ArgumentException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {ex.Message}";
            }
            catch (WebException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: WebException raised! :: {ex.Message}";
            }
            catch (Exception ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: Exception raised! :: {ex.Message}";
            }

            return Out;
        }
        #endregion

        #region Async Methods
        public static async Task<string> GetAsync(string url)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept", accept);
                client.Headers.Add("User-Agent", userAgent);
                //client.Headers.Add("Charset", "UTF-8");
                //client.Headers.Add("Accept-Encoding", "gzip, deflate");
                //client.Headers.Add("Accept-Charset", "ISO-8859-1");
                //client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                //client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                client.Encoding = Encoding.UTF8;
                var responseString = await client.DownloadStringTaskAsync(url);
                return responseString;
            }
        }

        public static async Task<string> PostAsync(string url, NameValueCollection values)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept", accept);
                client.Headers.Add("User-Agent", userAgent);

                //var values = new NameValueCollection();
                //values["thing1"] = "hello";
                //values["thing2"] = "world";

                //var values2 = new NameValueCollection()
                //{
                //   { "home", "Cosby" },
                //   { "favorite+flavor", "flies" }
                //};

                client.Encoding = Encoding.UTF8;
                var response = await client.UploadValuesTaskAsync(url, values);

                var responseString = Encoding.UTF8.GetString(response);
                return responseString;
            }
        }

        /// <summary>
        /// Sending POST request.
        /// </summary>
        /// <param name="Url">Request Url.</param>
        /// <param name="Data">Data for request.</param>
        /// <returns>Response body.</returns>
        public static async Task<string> PostAsync(string Url, string Data)
        {
            string Out = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(Url);
            try
            {
                req.Method = "POST";
                req.Timeout = 100000;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = userAgent;
                //req.Accept = accept;

                byte[] sentData = Encoding.UTF8.GetBytes(Data);
                req.ContentLength = sentData.Length;
                using (var sendStream = await req.GetRequestStreamAsync())
                {
                    await sendStream.WriteAsync(sentData, 0, sentData.Length);
                    sendStream.Close();
                }
                var res = await req.GetResponseAsync();
                var ReceiveStream = res.GetResponseStream();
                Out = await new StreamReader(ReceiveStream, Encoding.UTF8).ReadToEndAsync();
                //using (StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8))
                //{
                //    Char[] read = new Char[256];
                //    int count = sr.Read(read, 0, 256);
                //    while (count > 0)
                //    {
                //        String str = new String(read, 0, count);
                //        Out += str;
                //        count = sr.Read(read, 0, 256);
                //    }
                //}
            }
            catch (ArgumentException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {ex.Message}";
            }
            catch (WebException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: WebException raised! :: {ex.Message}";
            }
            catch (Exception ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: Exception raised! :: {ex.Message}";
            }

            return Out;
        }

        /// <summary>
        /// Sending GET request.
        /// </summary>
        /// <param name="Url">Request Url.</param>
        /// <param name="Data">Data for request.</param>
        /// <returns>Response body.</returns>
        public static async Task<string> GetAsync(string Url, string Data)
        {
            string Out = string.Empty;
            var req = (HttpWebRequest)WebRequest.Create(Url + (string.IsNullOrEmpty(Data) ? "" : "?" + Data));
            try
            {
                req.UserAgent = userAgent;
                //req.Accept = accept;
                req.AllowAutoRedirect = true;
                //req.MaximumAutomaticRedirections = 1;

                var resp = await req.GetResponseAsync();
                using (var stream = resp.GetResponseStream())
                {
                    Out = await new StreamReader(stream, Encoding.UTF8).ReadToEndAsync();
                }
            }
            catch (ArgumentException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {ex.Message}";
            }
            catch (WebException ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: WebException raised! :: {ex.Message}";
            }
            catch (Exception ex)
            {
                ex.LogError();
                Out = $"HTTP_ERROR :: Exception raised! :: {ex.Message}";
            }

            return Out;
        }
        #endregion
    }
}
