using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CIYW.Kernel.Extensions;

    public static class StringExtension
    {
        public static string FirstCharUpper(this string str)
        {
            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        public static string CleanPath(this string str)
        {
            str = str.Replace('\\', '/');
            return str.Replace("//", "/");
        }

        public static string MD5Hash(this string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, source);
                return hash;
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool NotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool NullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string TrimWhiteSpaces(this string str)
        {
            return Regex.Replace(str, @"\s+", "");
        }

        public static string NormalizeString(this string str)
        {
            return str.TrimWhiteSpaces().ToUpper();
        }

        public static Guid ToGuid(this string str)
        {
            if (Guid.TryParse(str, out var id))
            {
                return id;
            }

            return Guid.Empty;
        }

        public static Guid? ToGuidNullable(this string str)
        {
            if (Guid.TryParse(str, out var id))
            {
                return id;
            }

            return null;
        }

        public static string GetStringTwoDigitsMore(this double val)
        {
            return val > 9 ? $"{val}" : $"0{val}";
        }
        public static string GetStringTwoDigitsMore(this int val)
        {
            return val > 9 ? $"{val}" : $"0{val}";
        }

        public static string ClearResponseContent(this string content)
        {
            try
            {
                return content
                    .Replace("{", "")
                    .Replace("}", "")
                    .Replace("\r\n", "")
                    .Replace("  ", "")
                    .Replace("\"", "");
            }
            catch (Exception)
            {
                return content;
            }
        }
    }