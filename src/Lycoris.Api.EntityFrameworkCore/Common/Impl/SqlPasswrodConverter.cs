using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Cryptography;
using System.Text;

namespace Lycoris.Api.EntityFrameworkCore.Common.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlPasswrodConverter : ValueConverter<string, string>
    {
        private const string SALT = "Yt,6JhDEoaTZVFAm";

        /// <summary>
        /// 
        /// </summary>
        public SqlPasswrodConverter() : base(x => Encrypt(x), x => x)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encrypt(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            var pwdAndSalt = Encoding.UTF8.GetBytes(value + SALT);
            var hashBytes = SHA256.Create().ComputeHash(pwdAndSalt);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
