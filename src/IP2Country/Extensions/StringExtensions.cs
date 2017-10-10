using System;
using System.Security.Cryptography;
using System.Text;

namespace IP2Country.Extensions
{
    public static class StringExtensions
    {
        public static string GetMd5(string str)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var data = Encoding.UTF8.GetBytes(str);
                data = md5.ComputeHash(data);
                return BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
            }
        }

        public static Guid AsGuid(this string str)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var data = Encoding.UTF8.GetBytes(str);
                var hash = md5.ComputeHash(data);
                return new Guid(hash);
            }
        }
    }
}