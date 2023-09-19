using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Api.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 登录令牌表
    /// </summary>
    [Table("LoginToken")]
    public class LoginToken : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        [TableColumn(StringLength = 255, DefaultValue = "", Required = true)]
        public string Token { get; set; } = "";

        /// <summary>
        /// 访问令牌过期时间
        /// </summary>
        [TableColumn(Required = true)]
        public DateTime TokenExpireTime { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [TableColumn(StringLength = 255, DefaultValue = "", Required = true)]
        public string RefreshToken { get; set; } = "";

        /// <summary>
        /// 刷新令牌过期时间
        /// </summary>
        [TableColumn(Required = true)]
        public DateTime RefreshTokenExpireTime { get; set; }

        /// <summary>
        /// 刷新令牌额外辅助
        /// </summary>
        [TableColumn(StringLength = 255, DefaultValue = "")]
        public string RefreshRemark { get; set; } = "";
    }
}
