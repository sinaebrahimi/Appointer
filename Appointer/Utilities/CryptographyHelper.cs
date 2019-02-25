using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Appointer.Utility
{
    public static class CryptographyHelper
    {
        public static byte[] GetBytes(this string input)
        {
            //return Convert.FromBase64String(input);
            //return TextEncodings.Base64Url.Decode(input);
            return Encoding.UTF8.GetBytes(input);
        }

        public static string ToMd5Hash(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                var originalBytes = Encoding.Default.GetBytes(str);

                var encodedBytes = md5.ComputeHash(originalBytes);
                return BitConverter.ToString(encodedBytes).Replace("-", string.Empty);
            }
        }

        public static byte[] ToMd5HashByte(this string str)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                var hashData = new byte[16];
                hashData = md5.ComputeHash(new UTF8Encoding().GetBytes(str));
                return hashData;
            }
        }

        /// <summary>
        /// Calculates the SHA1 hash of the supplied string and returns a base 64 string.
        /// </summary>
        /// <param name="str">String that must be hashed.</param>
        /// <returns>The hashed string or null if hashing failed.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToHash or key is null or empty.</exception>
        public static string ToSHA1Hash(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("An empty string value cannot be hashed.");

            //using (var sha1 = new SHA1Managed())
            //{
            //    var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
            //    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
            //}

            using (var hashAlgorithm = new SHA1CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(str);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

        public static string ToSHA256Hash(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("An empty string value cannot be hashed.");

            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(str);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

        /// <summary>
        /// Supported hash algorithms
        /// </summary>
        public enum HashType
        {
            HMAC,
            HMACMD5,
            HMACSHA1,
            HMACSHA256,
            HMACSHA384,
            HMACSHA512,
            MACTripleDES,
            MD5,
            RIPEMD160,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        /// <summary>
        /// Computes the hash of the string using a specified hash algorithm
        /// </summary>
        /// <param name="str">The string to hash</param>
        /// <param name="hashType">The hash algorithm to use</param>
        /// <returns>The resulting hash or an empty string on error</returns>
        public static string ToHashCode(this string str, HashType hashType)
        {
            try
            {
                byte[] hash = null;

                var bytes = Encoding.ASCII.GetBytes(str);

                switch (hashType)
                {
                    case HashType.HMAC:
                        hash = HMAC.Create().ComputeHash(bytes);
                        break;
                    case HashType.HMACMD5:
                        hash = HMACMD5.Create().ComputeHash(bytes);
                        break;
                    case HashType.HMACSHA1:
                        hash = HMACSHA1.Create().ComputeHash(bytes);
                        break;
                    case HashType.HMACSHA256:
                        hash = HMACSHA256.Create().ComputeHash(bytes);
                        break;
                    case HashType.HMACSHA384:
                        hash = HMACSHA384.Create().ComputeHash(bytes);
                        break;
                    case HashType.HMACSHA512:
                        hash = HMACSHA512.Create().ComputeHash(bytes);
                        break;
                    case HashType.MACTripleDES:
                        hash = MACTripleDES.Create().ComputeHash(bytes);
                        break;
                    case HashType.MD5:
                        hash = MD5.Create().ComputeHash(bytes);
                        break;
                    case HashType.RIPEMD160:
                        hash = RIPEMD160.Create().ComputeHash(bytes);
                        break;
                    case HashType.SHA1:
                        hash = SHA1.Create().ComputeHash(bytes);
                        break;
                    case HashType.SHA256:
                        hash = SHA256.Create().ComputeHash(bytes);
                        break;
                    case HashType.SHA384:
                        hash = SHA384.Create().ComputeHash(bytes);
                        break;
                    case HashType.SHA512:
                        hash = SHA512.Create().ComputeHash(bytes);
                        break;
                }

                var sb = new StringBuilder();

                for (var i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        #region "Reflected from System.Web.HttpUtility"

        private static string UrlEncode2(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncode2(str, Encoding.UTF8);
        }

        private static string UrlEncode2(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        private static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            var bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count,
            bool alwaysCreateReturnValue)
        {
            var num = 0;

            var num2 = 0;

            for (var i = 0; i < count; i++)
            {
                var ch = (char)bytes[offset + i];

                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsSafe(ch))
                {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0))
            {
                return bytes;
            }
            var buffer = new byte[count + (num2 * 2)];

            var num4 = 0;

            for (var j = 0; j < count; j++)
            {
                var num6 = bytes[offset + j];

                var ch2 = (char)num6;

                if (IsSafe(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        internal static bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }

        internal static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }

        #endregion

        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "tu89geji340t89u2";
        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;

        public static string Encrypt(this string plainText, string passPhrase, string RijndaelManaged = "")
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var password = new PasswordDeriveBytes(passPhrase, null);

            var keyBytes = password.GetBytes(keysize / 8);

            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            var memoryStream = new MemoryStream();

            var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            var cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(this string cipherText, string passPhrase, string RijndaelManaged = "")
        {
            var initVectorBytes = Encoding.ASCII.GetBytes(initVector);

            var cipherTextBytes = Convert.FromBase64String(cipherText);

            var password = new PasswordDeriveBytes(passPhrase, null);

            var keyBytes = password.GetBytes(keysize / 8);

            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            var memoryStream = new MemoryStream(cipherTextBytes);

            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            var plainTextBytes = new byte[cipherTextBytes.Length];

            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string Encrypt(this string str)
        {
            var encData_byte = new byte[str.Length];
            encData_byte = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encData_byte);
        }

        public static string Decrypt(this string str)
        {
            var encoder = new UTF8Encoding();

            var utf8Decode = encoder.GetDecoder();

            var todecode_byte = Convert.FromBase64String(str);

            var charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);

            var decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            return new string(decoded_char);
        }

        /// <summary>
        /// Encrypt(strText, "&%#@?,:*");
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(this string str, string key)
        {
            byte[] byKey;

            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };

            try
            {
                byKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                var des = new DESCryptoServiceProvider();

                var inputByteArray = Encoding.UTF8.GetBytes(str);

                var ms = new MemoryStream();

                var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Decrypt(strText, "&%#@?,:*");
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(this string str, string key)
        {
            byte[] byKey;

            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };

            byte[] inputByteArray;
            // inputByteArray.Length = strText.Length;
            try
            {
                byKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(str);
                var ms = new MemoryStream();

                var cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                var encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
