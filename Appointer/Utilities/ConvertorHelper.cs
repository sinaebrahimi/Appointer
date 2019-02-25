using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Appointer.Utility
{
    public static class ConvertorHelper
    {
        public static string ConvertToString(IEnumerable<ValueType> value)
        {
            return string.Join("-", value);
        }
        public static string ConvertToString(DateTime value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        public static string ConvertToString(TimeSpan value, bool includeNow = false)
        {
            return value.ToString("c", CultureInfo.InvariantCulture);
        }


        public static T ConvertTo<T>(string value) where T : new()
        {
            var type = ReflectionHelper.GetBaseType<T>();
            if (ValidationHelper.IsNumeric<T>())
            {
                value.Split('-').Select(p => p.ConvertTo<T>());
            }
            return new T();
        }

        public static int TryToInt(this object obj)
        {
            //int result2 = 0;
            //int.TryParse(obj.ToString(), out result2);
            //return result2;

            var result = 0;
            try
            {
                result = Convert.ToInt32(obj);
            }
            catch
            {
            }
            return result;
        }

        public static object ToDBNull(this object obj)
        {
            return DBNull.Value.Equals(obj) ? null : obj;
        }

        public static byte ToByte(this object obj)
        {
            return Convert.ToByte(obj);
        }

        public static bool ToBoolean(this object obj)
        {
            return Convert.ToBoolean(obj);
        }

        public static decimal ToDecimal(this object obj)
        {
            return Convert.ToDecimal(obj);
        }

        public static int ToInt(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static int? ToNullableInt(this object obj)
        {
            int? result = null;
            if (obj != null && obj.ToString() != "")
                result = Convert.ToInt32(obj);
            return result;
        }

        public static T CastTo<T>(this object obj)
        {
            return ((T)obj);
        }

        public static T ConvertTo<T>(this object obj)
        {
            return Convert.ChangeType(obj, typeof(T)).CastTo<T>();
        }
        public static object ConvertTo(this object obj, Type type)
        {
            return Convert.ChangeType(obj, type);
        }

        public static char ToASCIIchar(this int code)
        {
            return Convert.ToChar(code);
        }

        public static int ToASCIIcode(this char ch)
        {
            return Convert.ToInt32(ch);
        }

        public static string ToText(this int digit)
        {
            var txt = digit.ToString();

            var length = txt.Length;

            var a1 = new string[10] { "-", "یک", "دو", "سه", "چهار", "پنح", "شش", "هفت", "هشت", "نه" };

            var a2 = new string[10] { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };

            var a3 = new string[10] { "-", "ده", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };

            var a4 = new string[10] { "-", "یک صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفصد", "هشصد", "نهصد" };

            var result = "";

            var isDahegan = false;

            for (var i = 0; i < length; i++)
            {
                var character = txt[i].ToString();

                switch (length - i)
                {
                    case 7: //میلیون
                        if (character != "0")
                        {
                            result += a1[Convert.ToInt32(character)] + " میلیون و ";
                        }
                        else
                        {
                            result = result.TrimEnd('و', ' ');
                        }
                        break;
                    case 6: //صدهزار
                        if (character != "0")
                        {
                            result += a4[Convert.ToInt32(character)] + " و ";
                        }
                        else
                        {
                            result = result.TrimEnd('و', ' ');
                        }
                        break;
                    case 5: //ده هزار
                        if (character == "1")
                        {
                            isDahegan = true;
                        }
                        else if (character != "0")
                        {
                            result += a3[Convert.ToInt32(character)] + " و ";
                        }
                        break;
                    case 4: //هزار
                        if (isDahegan)
                        {
                            result += a2[Convert.ToInt32(character)] + " هزار و ";
                            isDahegan = false;
                        }
                        else
                        {
                            if (character != "0")
                            {
                                result += a1[Convert.ToInt32(character)] + " هزار و ";
                            }
                            else
                            {
                                result = result.TrimEnd('و', ' ');
                            }
                        }
                        break;
                    case 3: //صد
                        if (character != "0")
                        {
                            result += a4[Convert.ToInt32(character)] + " و ";
                        }
                        break;
                    case 2: //ده
                        if (character == "1")
                        {
                            isDahegan = true;
                        }
                        else if (character != "0")
                        {
                            result += a3[Convert.ToInt32(character)] + " و ";
                        }
                        break;
                    case 1: //یک
                        if (isDahegan)
                        {
                            result += a2[Convert.ToInt32(character)];
                            isDahegan = false;
                        }
                        else
                        {
                            if (character != "0") result += a1[Convert.ToInt32(character)];
                            else result = result.TrimEnd('و', ' ');
                        }
                        break;
                }
            }
            return result;
        }

        public static string ToPrice(this object dec)
        {
            var Src = dec.ToString();
            Src = Src.Replace(".0000", "");
            if (!Src.Contains("."))
            {
                Src = Src + ".00";
            }
            var price = Src.Split('.');

            if (price[1].Length >= 2)
            {
                price[1] = price[1].Substring(0, 2);
                price[1] = price[1].Replace("00", "");
            }

            string Temp = null;

            var i = 0;

            if ((price[0].Length % 3) >= 1)
            {
                Temp = price[0].Substring(0, (price[0].Length % 3));
                for (i = 0; i <= (price[0].Length / 3) - 1; i++)
                {
                    Temp += "," + price[0].Substring((price[0].Length % 3) + (i * 3), 3);
                }
            }
            else
            {
                for (i = 0; i <= (price[0].Length / 3) - 1; i++)
                {
                    Temp += price[0].Substring((price[0].Length % 3) + (i * 3), 3) + ",";
                }
                Temp = Temp.Substring(0, Temp.Length - 1);
                // Temp = price(0)
                //If price(1).Length > 0 Then
                //    Return price(0) + "." + price(1)
                //End If
            }
            if (price[1].Length > 0)
            {
                return Temp + "." + price[1];
            }
            return Temp;
        }
        public static string ToPriceString(this object obj)
        {
            return obj.ToString().Trim('0').Trim('.');
        }

        /// <summary>
        /// Extend any collection implementing IList to return a DataView.
        /// </summary>
        /// <param name="list">IList (Could be List<Type>)</param>
        /// <returns>DataView</returns>
        public static DataView ToDataView(this IList list)
        {
            // Validate Source
            if (list.Count < 1)
                return null;

            // Initialize DataTable and get all properties from the first Item in the List.
            var table = new DataTable(list.GetType().Name);

            var properties = list[0].GetType().GetProperties();

            // Build all columns from properties found. (Custom attributes could be added later)
            foreach (var info in properties)
            {
                try
                {
                    table.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                }
                catch (NotSupportedException)
                {
                    // DataTable does not support Nullable types, we want to keep underlying type.
                    table.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType)));
                }
                catch (Exception)
                {
                    table.Columns.Add(new DataColumn(info.Name, typeof(object)));
                }
            }

            // Add all rows
            for (var index = 0; index < list.Count; index++)
            {
                var row = new object[properties.Length];

                for (var i = 0; i < row.Length; i++)
                {
                    row[i] = properties[i].GetValue(list[index], null); // Get the value for each items property
                }

                table.Rows.Add(row);
            }

            return new DataView(table);
        }

        public static DataTable ExcelToTable(this string filePath, bool hasHeader)
        {
            try
            {
                var Con = new OleDbConnection();

                if (Path.GetExtension(filePath).ToLower() == ".xlsx")
                {
                    Con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath +
                                           ";Extended Properties=\"Excel 12.0 Xml;HDR=" + hasHeader + "\";";
                }
                if (Path.GetExtension(filePath).ToLower() == ".xls")
                {
                    Con.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath +
                                           ";Extended Properties=\"Excel 8.0;HDR=" + hasHeader + "\";";
                }
                var Cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", Con);

                var DT = new DataTable();
                Con.Open();
                DT.Load(Cmd.ExecuteReader());
                Con.Close();
                Con.Dispose();
                Cmd.Dispose();
                return DT;
            }
            catch
            {
                return null;
            }
        }

        public static string ToCSV<T>(this IEnumerable<T> instance)
        {
            StringBuilder csv;

            if (instance != null)
            {
                csv = new StringBuilder();
                instance.ForEach(v => csv.AppendFormat("{0},", v));
                return csv.ToString(0, csv.Length - 1);
            }
            return null;
        }

        public static string ToCSV<T>(this IEnumerable<T> instance, char separator)
        {
            StringBuilder csv;

            if (instance != null)
            {
                csv = new StringBuilder();
                instance.ForEach(value => csv.AppendFormat("{0}{1}", value, separator));
                return csv.ToString(0, csv.Length - 1);
            }
            return null;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist)
        {
            var dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;
            foreach (var rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = rec.GetType().GetProperties();
                    foreach (var pi in oProps)
                    {
                        var colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                var dr = dtReturn.NewRow();

                foreach (var pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null
                        ? DBNull.Value
                        : pi.GetValue
                            (rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        public static string ConvertUrlsToLinks(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            string regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";

            var html = new Regex(regex, RegexOptions.IgnoreCase).Replace(str, "<a href=\"$1\" target=\"&#95;blank\">$1</a><br />").Replace("href=\"www", "href=\"http://www");

            return html;
        }

        //add "System.Runtime.Serialization.dll" to references
        //add namespace "System.Runtime.Serialization.Json"
        //example:
        //JsonConvertor.Serialize1(myPerson) or myPerson.Serialize1()
        /// <summary>
        /// serialize object to json and return string
        /// </summary>
        /// <typeparam name="T">type of your class</typeparam>
        /// <param name="item">the object to serialize to json</param>
        /// <returns>string of serialize json</returns>
        public static string Serialize1<T>(this T item) where T : class
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, item);
                var json = Encoding.UTF8.GetString((stream.ToArray()));
                return json;
            }
        }

        //add "System.Web.Extensions.dll" to references
        //add namespace "System.Web.Script.Serialization"
        //example:
        //JsonConvertor.Serialize2(myPerson) or myPerson.Serialize2()
        /// <summary>
        /// serialize object to json and return string
        /// </summary>
        /// <typeparam name="T">type of your class</typeparam>
        /// <param name="obj">the object to serialize to json</param>
        /// <returns>string of serialize json</returns>
        public static string Serialize2<T>(this T obj) where T : class
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        //add "Newtonsoft.Json.dll" to references
        //add namespace "Newtonsoft.Json"
        //example:
        //JsonConvertor.Serialize(myPerson) or myPerson.Serialize()
        /// <summary>
        /// serialize object to json and return string
        /// </summary>
        /// <typeparam name="T">type of your class</typeparam>
        /// <param name="obj">the object to serialize to json</param>
        /// <returns>string of serialize json</returns>
        public static string Serialize<T>(this T obj) where T : class
        {
            var str = JsonConvert.SerializeObject(obj);
            return str;
        }

        //add "System.Xml.dll" to references
        //add namespace "System.Xml.Serialization"
        //example:
        //JsonConvertor.SerializeXml(myPerson) or myPerson.SerializeXml()
        /// <summary>
        /// serialize object to xml and return string
        /// </summary>
        /// <typeparam name="T">type of your class</typeparam>
        /// <param name="input">the object to serialize to json</param>
        /// <returns>string of serialize json</returns>
        public static string SerializeXml<T>(this T input) where T : class
        {
            using (var writer = new StringWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(writer, input);
                return writer.ToString();
            }
        }

        //add "System.Runtime.Serialization.dll" to references
        //add namespace "System.Runtime.Serialization.Json"
        //example:
        //JsonConvertor.Deserialize1<Person>("string json") or "string json".Deserialize1<Person>()
        /// <summary>
        /// deserialize json string to object of type T
        /// </summary>
        /// <typeparam name="T">type of your class</typeparam>
        /// <param name="json">string of json</param>
        /// <returns>object of type T that deserialize from string json</returns>
        public static T Deserialize1<T>(this string json)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));

                var obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }

        //add "System.Web.Extensions.dll" to references
        //add namespace "System.Web.Script.Serialization"
        //example:
        //JsonConvertor.Deserialize1<Person>("string json") or "string json".Deserialize1<Person>()
        /// <summary>
        /// deserialize json string to object of type T
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="json">string of json</param>
        /// <returns>object of type T that deserialize from string json</returns>
        public static T Deserialize2<T>(this string json)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        //add "Newtonsoft.Json.dll" to references
        //add namespace "Newtonsoft.Json"
        //example:
        //JsonConvertor.Deserialize<Person>("string json") or "string json".Deserialize<Person>()
        /// <summary>
        /// deserialize json string to object of type T
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="json">string of json</param>
        /// <returns>object of type T that deserialize from string json</returns>
        public static T Deserialize<T>(this string json)
        {
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public static string Deserialize(this string json)
        {
            var obj = JsonConvert.DeserializeObject<string>(json);
            return obj;
        }
        //add "System.Xml.dll" to references
        //add namespace "System.Xml.Serialization"
        //example:
        //JsonConvertor.DeserializeXml<Person>("string xml") or "string xml".DeserializeXml<Person>()
        /// <summary>
        /// deserialize xml string to object of type T
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="xml">string of xml</param>
        /// <returns>object of type T that deserialize from string xml</returns>
        public static T DeserializeXml<T>(this string xml)
        {
            using (var reader = new StringReader(xml))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }

        //example:
        //JsonConvertor.ToStringBuffer(myImage) or myImage.ToStringBuffer()
        /// <summary>
        /// convert image to string for save as string into a text file
        /// </summary>
        /// <param name="img">the image to convert to string</param>
        /// <returns>string of image</returns>
        public static string ToStringBuffer(this Image img)
        {
            var stream = new MemoryStream();
            img.Save(stream, ImageFormat.Png);
            return Convert.ToBase64String(stream.ToArray());
        }

        //example:
        //JsonConvertor.ToStringBuffer(myObject) or myObject.ToStringBuffer()
        /// <summary>
        /// convert object type of T to string for save as string into a text file
        /// </summary>
        /// <param name="obj">the image to convert to string</param>
        /// <returns>string of object of type T</returns>
        public static string ToStringBuffer<T>(this T obj) where T : class
        {
            string str = obj.ToBytes().ToStringBuffer();
            return str;
        }

        //example:
        //JsonConvertor.ToStringBuffer(myBytes) or myBytes.ToStringBuffer()
        /// <summary>
        /// convert bytesArray to string for save as string into a text file
        /// </summary>
        /// <param name="bytes">the bytesArray to convert to string</param>
        /// <returns>string bytesArray</returns>
        public static string ToStringBuffer(this byte[] bytes)
        {
            var str = Convert.ToBase64String(bytes);
            return str;
        }

        //example:
        //JsonConvertor.ToImage("string of image") or "string of image".ToImage()
        /// <summary>
        /// convert string of image to an image
        /// </summary>
        /// <param name="stringBuffer">the string of image to convert to image</param>
        /// <returns>string of image</returns>
        public static Image ToImage(this string stringBuffer)
        {
            var arr = Convert.FromBase64String(stringBuffer);

            var stream = new MemoryStream(arr);

            Image img = new Bitmap(stream);
            return img;
        }

        //example:
        //JsonConvertor.ToImage(bytesArray) or bytesArray.ToImage()
        /// <summary>
        /// convert bytesArray of image to an image
        /// </summary>
        /// <param name="bytes">the bytesArray to convert to image</param>
        /// <returns>image of bytesArray</returns>
        public static Image ToImage(this byte[] bytes)
        {
            var stream = new MemoryStream(bytes);

            Image img = new Bitmap(stream);
            return img;
        }

        public static byte[] ToBytes<T>(this T obj) where T : class
        {
            var bf = new BinaryFormatter();

            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static byte[] ToBytes(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte[] ToBytes(this Image img, ImageFormat format)
        {
            var ms = new MemoryStream();
            img.Save(ms, format);
            var arr = ms.GetBuffer();
            ms.Close();
            return arr;
        }

        public static T ToObject<T>(this byte[] byteArr)
        {
            var memStream = new MemoryStream();

            var binForm = new BinaryFormatter();
            memStream.Write(byteArr, 0, byteArr.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return (T)obj;
        }

    }
}