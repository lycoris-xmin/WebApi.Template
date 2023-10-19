using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lycoris.Api.EntityFrameworkCore.Common.Impl
{
    internal class SqlSensitiveConverter : ValueConverter<string, string>
    {
        private const string KEY = "2B9E8A3F";
        private const string IV = "20220101";

        public SqlSensitiveConverter() : base(x => Encrypt(x), x => Decrypt(x))
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Encrypt(string value)
        {
            if (value.IsNullOrEmpty())
                return value;

            return SecretHelper.DESEncrypt(value, KEY, IV);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Decrypt(string value)
        {
            if (value.IsNullOrEmpty())
                return value;

            return SecretHelper.DESDecrypt(value, KEY, IV) ?? "";
        }
    }
}
